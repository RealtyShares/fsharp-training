// sets the working directory in the REPL to the dame directory as this file
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ 

#load "../../.paket/load/net461/fsharpx.extras.fsx"

open FSharpx

// A helpful "Result" module with some type aliases for Choice<'a,b'>
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

// Step 1) implement isEven : int -> Result<int,string>
let isEven num =
    match num % 2 = 0 with
    | true -> Success num
    | false -> Failure <| sprintf "%d is not even" num

// test it!
isEven 0
isEven 3

// Step 2) implement isGreaterThan : int -> int -> Result<int,string>
let isGreaterThan min num = 
    match num > min with
    | true -> Success num
    | false -> Failure <| sprintf "%d is not greater than %d" num min

// test it!
12 |> isGreaterThan 10
9 |> isGreaterThan 10

// Step 3) implement isLessThan : int -> int -> Result<int,string>
let isLessThan max num = 
    match num < max with
    | true -> Success num
    | false -> Failure <| sprintf "%d is not less than %d" num max

// test it!
9 |> isLessThan 10
12 |> isLessThan 10

// Step 3a) implement isEvenAndGreaterThan : int -> int -> Result<int,string>
let isEvenAndGreaterThan min num =
    match isEven num with
    | Success num ->
        match num |> isGreaterThan min with
        | Success num -> Success num
        | Failure _ ->
            Failure <| sprintf "%d is not greater than %d" num min
    | Failure _ ->
        Failure <| sprintf "%d is not even" num

// test it!
12 |> isEvenAndGreaterThan 10
13 |> isEvenAndGreaterThan 10
8 |> isEvenAndGreaterThan 10

// Step 4) implement Choice.bind : Choice<'a,'error> -> ('a -> Choice<'b,'error>) -> Choice<'b,'error>
module Choice =
    let bind fmap m =
        match m with
        | Success value -> fmap value
        | Failure msg -> Failure msg

// test Choice.bind
let ok : Result<int,string> = Success 1
ok |> Choice.bind isEven

// test Choice.bind
let ok2 : Result<int,string> = Success 2
ok2 |> Choice.bind isEven

// Step 4a) implement isEvenAndGreaterThan' int -> int -> Result<int,string>
let isEvenAndGreaterThan' min num =
    num
    |> isEven
    |> Choice.bind (isGreaterThan min)

// test it!
12 |> isEvenAndGreaterThan' 10
13 |> isEvenAndGreaterThan' 10
8 |> isEvenAndGreaterThan' 10

// Step 4b) implement Modadic bind operator (>>=)
let (>>=) l r =  l |> Choice.bind r

// test it!
(12 |> isEven) >>= isGreaterThan 10
(Failure "something bad happend") >>= isEven >>= isGreaterThan 10

// Step 5) implement Kliesli composition a.k.a. the fish operator: (>=>)
let (>=>) (fn1:'a -> Result<'b,'error>) (fn2:'b -> Result<'c,'error>) : ('a -> Choice<'c,'error>) =
    fn1 >> Choice.bind fn2

// Step 5) implement isEvenAndGreaterThan'' using the fish operator
let isEvenAndGreaterThan'' min = 
    isEven
    >=> isGreaterThan min

// test it!
12 |> isEvenAndGreaterThan'' 10
13 |> isEvenAndGreaterThan'' 10
8 |> isEvenAndGreaterThan'' 10

// freestyle with the fish operator  test a "range" of values
14 |> (isEven >=> isGreaterThan 10 >=> isLessThan 13)

// refactor the range test into a separate function called, isBetween
let isBetween (min,max) = isGreaterThan min >=> isLessThan max

// Same output?
12 |> (isEven >=> isBetween (10,13))

// Step 6) implement "choice" computation expression
[<AutoOpen>]
module ChoiceBuilder =
    let choice = FSharpx.Choice.EitherBuilder()

// Step 5) implement isEvenAndGreaterThan''' using a choice computation expression
let isEvenAndGreaterThan''' min num = 
    choice {
        let! _ = num |> isEven
        let! _ = num |> isGreaterThan min
        return num
    }

// test it!
12 |> isEvenAndGreaterThan''' 10
13 |> isEvenAndGreaterThan''' 10
8 |> isEvenAndGreaterThan''' 10