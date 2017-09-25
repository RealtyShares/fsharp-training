// sets the working directory in the REPL to the dame directory as this file
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ 

#load "../../.paket/load/net461/fsharpx.extras.fsx"

open FSharpx

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =
    type Result<'s,'e> = Choice<'s,'e>

    let inline Success v : Result<_,_> = Choice1Of2 v
    let inline Failure v : Result<_,_> = Choice2Of2 v

    let (|Success|Failure|) =
        function
        | Choice1Of2 a -> Success a
        | Choice2Of2 e -> Failure e

// Let's write a reusable "validator" function
let validator predicate errorMessage value : ValidationResult<_,_> =
    if predicate value 
        then Success value
        else Failure (List.singleton errorMessage)


// Some basic (applicative style) validation combinators
module Validation =
    open System
    open System.Text.RegularExpressions
    
    type ValidationResult<'a,'b> = Result<'a,'b list>

    module Applicative =
        let ap (x:ValidationResult<_,_>) (f:ValidationResult<_,_>) : ValidationResult<_,_> =
            match f,x with
            | Choice1Of2 f, Choice1Of2 x     -> Choice1Of2 (f x)
            | Choice2Of2 e, Choice1Of2 x     -> Choice2Of2 e
            | Choice1Of2 f, Choice2Of2 e     -> Choice2Of2 e
            | Choice2Of2 e1, Choice2Of2 e2 -> Choice2Of2 (List.append e1 e2)

        /// Sequential application
        let inline (<*>) f x = ap x f

        let inline (<!>) f (c:ValidationResult<_,string>) = Success f <*> c

        let inline (>>=) x f = Choice.bind f x

        /// Promote a function to a monad/applicative, scanning the monadic/applicative arguments from left to right.
        let inline lift2 f a b = Success f <*> a <*> b

        /// Sequence actions, discarding the value of the first argument.
        let inline ( *>) x y = lift2 (fun _ z -> z) x y

        /// Sequence actions, discarding the value of the second argument.
        let inline ( <*) x y = lift2 (fun z _ -> z) x y

        // Composes a choice type with a non choice type.
        let inline (<?>) a b = lift2 (fun _ z -> z) a (Success b)

        // Composes a non-choice type with a choice type.
        let inline (|?>) a b = lift2 (fun z _ -> z) (Success a) b

    let opt defaultVal c =
        match c with
        | Choice1Of2 v -> Success v
        | Choice2Of2 _ -> Success defaultVal

    let validator pred error value : ValidationResult<_,_> =
        if pred value then Success value
        else Failure (List.singleton error)

    let validatorM fmap error value : ValidationResult<_,_> =
        let value' : 'a option = fmap value
        if value'.IsSome
            then Success value'.Value
            else Failure (List.singleton error)

    let pass x = validator (fun _ -> true) "" x
    let fail msg x = validator (fun _ -> false) msg x
    let lt n msg v = validator ((>) n) msg v
    let lte n msg v = validator ((>=) n) msg v
    let gt n msg v = validator ((<) n) msg v
    let gte n msg v = validator ((<=) n) msg v
    let eq n msg v = validator ((=) n) msg v
    let neq n msg v = validator ((<>) n) msg v
    let notNull msg v = validator (fun o -> not <| System.Object.ReferenceEquals(o, null)) msg v
    let tryMap f errorMsg v = validatorM (fun v -> try Some(f v) with | _ -> None) errorMsg v
    let matchesRegex (regex:Regex) msg v = validator regex.IsMatch msg v
    let isEnum<'a when 'a : (new : unit ->  'a)
                          and 'a : struct
                          and 'a :> System.ValueType>
        name ignoreCase validationResult =
        validationResult
        |> Choice.bind (fun s ->
            tryMap
                (fun s -> System.Enum.Parse(typeof<'a>, s, ignoreCase))
                (sprintf "%s should be an enum of type %s" name typeof<'a>.Name) s)

    module String =
        let notEmpty msg v = validator (fun v -> not <| String.IsNullOrEmpty v) msg v
        let notNullOrEmpty msg v = validator (not << System.String.IsNullOrEmpty) msg v
        let notNullOrWhitespace msg v = validator (not << System.String.IsNullOrWhiteSpace) msg v

    module Collection =
        let notEmptyList msg v =
            notNull msg v
            |> Choice.bind (validator (fun lst -> List.length lst > 0) msg)

    module Set =
        let contains (values:_ Set) errMsg = validator (fun v -> values |> Set.contains v) errMsg

    let isSome (v:'a option) msg =
        match v with
        | Some value -> Success value
        | None -> Failure (List.singleton msg)

    let forall pred msg (v:'a seq) = validator (Seq.forall pred) msg v





// some helpful type aliases to constrain ValidationResult to a error type of string list
type V<'a> = ValidationResult<'a,string>
let Success : 'a -> V<'a> = RSKernel.Validation.Success
let Failure : string list -> V<'a> = RSKernel.Validation.Failure

// validate

let isOdd err a = validator (fun v -> v % 2 = 1) err a

let validateV v =
    neq 2 "the value must not be 2" v
    *> isOdd "the value must be odd" v
    *> lte 10 "the value must less than or equal to 10" v

validateV 2 // output: Failure ["the value must not be 2"; "the value must be odd"]
validateV 4 // output: Failure ["the value must be odd"]
validateV 14 // output: Failure ["the value must be odd"; "the value must less than or equal to 10"]
validateV 3 // output: Success 3

// An int parser that returns a ValidationResult
let tryParseInt txt = 
    match System.Int32.TryParse(txt) with
    | true, value -> Success value
    | false, _ -> Failure [sprintf "%s is not an integer!" txt]

let validateVString (vstr:string) = validate {
    let! v = vstr |> tryParseInt
    return! neq 2 "the value must not be 2" v
    *> isOdd "the value must be odd" v
    *> lte 10 "the value must less than or equal to 10" v
    }
validateVString "one" // output: Failure ["one is not an integer!"]
validateVString "2" // output: Failure ["the value must not be 2"; "the value must be odd"]
validateVString "4" // output: Failure ["the value must be odd"]
validateVString "14" // output: Failure ["the value must be odd"; "the value must less than or equal to 10"]
validateVString "3" // output: Success 3

let test1 : V<_> = 
    validate {
        let! x = Success 0.0
        let! y = Success 10.0
        return (x,y)
    }

test1

let test2 : V<int*int> = 
    validate {
        let! x = Failure ["fail1!"]
        let! y = Failure ["fail2!"]
        return (x,y)
    }

test2

open RSKernel.AsyncValidation

type AV<'a> = AsyncValidationResult<'a,string>

let asyncTest1 : AV<int*int> =
    asyncValidate {
        let! x = async.Return(Failure ["missing a x value"])
        let! y = async.Return(Failure ["missing a y value"])
        return (x,y)
    } 
    
asyncTest1 |> Async.RunSynchronously

//"Applicative Style"
// <*, *>, <?>

let something = Success 1 <* Success "one"

something

let something2 = Success(1) *> Success "one"

something2

let moreWeirdOperators = async.Return(Success 1) <?> "boom"

moreWeirdOperators |> Async.RunSynchronously


let doesItMakeSenseNow = async.Return(Failure ["failure!"]) <?> "boom"

doesItMakeSenseNow |> Async.RunSynchronously

// monadic style - Choice.bind(), remembers previous results as opposed to applicative

// "elevated world" explanation
//https://fsharpforfunandprofit.com/posts/elevated-world/
//https://fsharpforfunandprofit.com/posts/elevated-world-2/

// http://bugsquash.blogspot.com/2011/08/validating-with-applicative-functors-in.html


/// more complicated validation, with refering back to the previous arguments in scope
let point2d x y = 
    x |> Choice.bind (fun xval ->
        y |> Choice.bind (fun yval -> 
                 Choice1Of2(xval,yval)
    )
)

let uu<'a> = point2d (Choice1Of2 1) (Choice1Of2 2)

uu


let apply (f:V<'a ->'b>) (x:V<'a>) : V<'b> =
    match f,x with
    | Success f, Success x -> Success (f x)
    | Failure e, Success x     -> Failure e
    | Success f, Failure e     -> Failure e
    | Failure e1, Failure e2 -> Failure (List.append e1 e2)


let (<*>) = apply

let (<!>) l r = apply (Success l) r

let (<?>) (l:ValidationResult<'a,string>) (r:'b) : ValidationResult<'b,string> = 
    match l with
    | Success _ -> Success r
    | Failure errs -> Failure errs

let temp = Failure ["fuck!"] <?> 3

//TODO: to figure out
let x = Success 1
let y = Success 2

let sum = 
//  (fun xval yval -> xval, yval)      x     y
    (fun xval yval -> xval + yval) <!> x <*> y


let sum2 : V<int>  = 
//  (fun xval yval -> xval, yval)      x     y
    (Success (fun xval yval -> xval + yval)) <*> x <*> y

let a x y = (fun xval yval -> (xval,yval))
            <!> (x |> Choice.bind (fun xval -> 
                                    if xval > 0.0 
                                    then Success xval 
                                    else Failure ["x must be positive"]))
            <*> y

let k = a (Success(1.0)) (Success(1.0)) 


let k2 = a (Success(-1.0)) (Success(1.0)) 

let k3 = a (Failure(["Oh snap"])) (Success(1.0)) 

let k4<'a> = a (Success(1.0)) (Failure(["Oh snap"])) 

k4