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

// Our validation result type that either succeeds or is a list of errors
type ValidationResult<'a,'b> = Result<'a,'b list>

// A "Person" record type that will serve as our "domain" model. 
// Notice some fields are "optional".
type Person = 
    { firstName : string
      lastName : string
      address1 : string
      address2 : string option
      city : string
      state : string
      zip: string
      phone : string option 
      email : string option }

// Some unstructured data in the form of a Map<string,string>
// Sort of looks like the bddy of a form post
let data = 
   ["firstName", "Tracy"
    "lastName", "Gonzalez"
    "address1", "525 Market Street"
    "address2", "Suite 2800"
    "city", "San Francisco"
    "state", "CA"
    "zip", "94107"
    "phone", "1-800-REA-LTYS"
    "email", "contact@realtyshares.com"
    ] |> Map.ofList

// Step 1) a naive "validation" function that looks at the unstructured data dictionary
// and returns a ValidationResult<Person,string>
let validatePerson (data:Map<string,string>) : ValidationResult<Person,string list>= 
    { firstName = data.["firstName"] // this is really bad. What if the key isn't in the data dictionary?
      lastName = data.["lastName"]
      address1 = data.["address1"]
      address2 = Some data.["address2"]
      city = data.["city"]
      state = data.["state"]
      zip = data.["zip"]
      phone = Some data.["phone"] 
      email = Some data.["email"] }
    |> Success

// let's test it
validatePerson data

// now let's try passing in some bad data
let badData = Map.ofList ["firstName","Tracy"; "nothing", "else"]
validatePerson badData // <-- we're going to get a runtime exception

// Step 2) a better  "validation" function that at least checks to that firstName and lastName exist
// will still fail if any of the other data keys are missing
let validatePerson' (data:Map<string,string>) : ValidationResult<Person,string>= 
    let firstName = data |> Map.tryFind "firstName" |> Choice.ofOption "firstName is required"
    let lastName = data |> Map.tryFind "lastName" |> Choice.ofOption "lastName is required"
    match firstName,lastName with
    | Success firstName, Success lastName ->
        { firstName = firstName
          lastName = data.["lastName"]
          address1 = data.["address1"]
          address2 = Some data.["address2"]
          city = data.["city"]
          state = data.["state"]
          zip = data.["zip"]
          phone = Some data.["phone"] 
          email = Some data.["email"] }
        |> Success
    | Failure msg, (Success _) ->
        Failure [msg]
    | (Success _), Failure msg ->
        Failure [msg]
    | Failure msg1,Failure msg2 ->
        Failure [msg1; msg2]

// let's test it out
validatePerson' data
// now let's try passing in some bad data
let badData' = Map.ofList ["there should be more keys here", "right?"]
validatePerson' badData' // <-- notice we at least get a list of errors, now

let betterData = Map.ofList ["firstName", "Tracy"; "lastName", "Gonzalez"; "missing", "values..."]
validatePerson' betterData // <-- we're going to get a runtime exception because everything but first/last name is missing


// Step 3) let's try a different approach
// 1. We have some ValidationResult values that may or may not be Success or Failure
// 2. We want to pass the Success values into a new function that returns our domain model
// 3. We want to enumerate all the Failure messages into a nice list in case validation fails 

// maybe let's start with a simpler domain model
type PhoneNumber = | PhoneNumber of string

// a function that produces a phone number from a string
let createPhoneNumber num = PhoneNumber num

// can we validate this? It's already a guaranteed success!
let phoneStr : ValidationResult<string,string> = Success "1-800-REA-LTYS"

// let's do something crazy and write a function that "lifts" the createPhoneNumber function into a ValidationResult<_,_>
let validatePhoneNumber : ValidationResult<(string->PhoneNumber),string> = 
    Success createPhoneNumber

// let's try passing the stringly-typed phone number into our validation function
let validationResult = 
    match phoneStr,validatePhoneNumber with
    | Success phoneNumber, Success createPhoneNumber -> 
        // phoneNumber and createPhoneNumber function are "Success" values. 
        // It's OK to apply the phoneNumber to the createPhoneNumber function, and return a Success
        Success <| createPhoneNumber phoneNumber
    | Failure msgs, Success _ -> 
        // the phone number had some validation errors. Just pass them through.
        Failure msgs
    | Success _, Failure msgs -> 
        // the createPhoneNUmber function had some validation errors. Just pass them through.
        Failure msgs
    | Failure msgs1, Failure msgs2 ->
        // the data is completely fubar. Concatonate the two lists of errros
        Failure (List.append msgs1 msgs2)

// evaluate this in the repl and be amazed
match validationResult with
| Success (PhoneNumber phoneNumber) ->
    printfn "OMG. It actually worked!"
| Failure msgs ->
    printfn "OMG. It actually failed!:"
    // print the error messages
    msgs |> List.iteri (fun i err -> printfn "%d - %s" (i+1) err)


// Step 4) OMG. It actually worked! 
// Let's refactor that validationResult match expression into its own function

// for the lack of a better name, we'll call this function "apply"
let apply (result:ValidationResult<'a,'error>) (fmap:ValidationResult<('a->'b),'error>) : ValidationResult<'b,'error> = 
    match result,fmap with
    | Success result, Success fmap -> Success(fmap result)
    | Failure msgs, Success _ -> Failure msgs
    | Success _, Failure msgs -> Failure msgs
    | Failure msgs1, Failure msgs2 -> Failure (List.append msgs1 msgs2)

// now, let's use apply to validate a phone number
let validationResult' : ValidationResult<PhoneNumber,string> =
    Success createPhoneNumber 
    |> apply (Success "1-800-REA-LTYS")

// let's see if we can get it to fail
let failureResult : ValidationResult<PhoneNumber,string> =
    Success createPhoneNumber 
    |> apply (Failure ["Not a phone number!"])

// Step 4) Hmm.... does this work for more complex domain models?
type ContactInfo = 
    { phoneNumber : PhoneNumber 
      emailAddress: EmailAddress }
and EmailAddress = | EmailAddress of string  

// yes, but it's not very pretty
let contactInfoValidationResult : ValidationResult<_,string> = 
    (Success <| fun phoneNumber email -> 
        { phoneNumber = PhoneNumber phoneNumber
          emailAddress = EmailAddress email })
    |> apply (Success "1-800-REA-LTYS")
    |> apply (Success "contact@realtyshares.com")      
      
// let's make some custom operators to help us out
let (<*>) fn r = apply r fn
// smame as the apply operator (<*>), but it "lifts" the left argument into a choice
let (<!>) fn r = Success fn <*> r

// this is starting to shape up now
let contactInfoValidationResult' : ValidationResult<_,string> = 
    (fun phoneNumber email -> 
        { phoneNumber = PhoneNumber phoneNumber
          emailAddress = EmailAddress email })
    <!> Success "1-800-REA-LTYS"
    <*> Success "contact@realtyshares.com"

// this is exactly what we want
let contactInfoFailureResult : ValidationResult<_,string> = 
    (fun phoneNumber email -> 
        { phoneNumber = PhoneNumber phoneNumber
          emailAddress = EmailAddress email })
    <!> Failure ["phone number is required"]
    <*> Failure ["email is required"]

// Step 5) Now for the moment of truth. Can we validate a Person type?    
let validatePerson'' (data:Map<string,string>) =
    // let's create a function first to help with pulling data out of a Map<string,string> 
    let tryGetValue key =
        data 
        |> Map.tryFind key
        |> Choice.ofOption [sprintf "%s is required" key]

    // now, let's make a person record!
    (fun firstName lastName 
         address1 address2 city state zip
         phone email ->
        { firstName = firstName
          lastName = lastName
          address1 = address1
          address2 = Some address2
          city = city
          state = state
          zip = zip
          phone = Some phone 
          email = Some email })
    <!> tryGetValue "firstName"
    <*> tryGetValue "lastName"
    <*> tryGetValue "address1"
    <*> tryGetValue "address2"
    <*> tryGetValue "city"
    <*> tryGetValue "state"
    <*> tryGetValue "zip"
    <*> tryGetValue "phone"
    <*> tryGetValue "email"
    
// let's test it!
validatePerson'' data // <-- Success!
validatePerson'' badData // <-- Failure! A whole mess of errors
validatePerson'' betterData // <-- Failure! Fewer errors

// Step 5) We're almost there. We just need to go a little deeper down the rabbit hole. 
// Can we make "tryGetValue" more generic? 

// Let's write a reusable "validator" function, that takes a true or value predicate and 
// 1. returns a Success when the predicate is true
// 2. returns a Failure with an error message when the predicate is false
let validator predicate errorMessage value : ValidationResult<_,_> =
    if predicate value 
        then Success value
        else Failure (List.singleton errorMessage)

// Let's write another variant of "validator" function, that "maps" the value to an optional value of a different type 
// 1. returns a Success when the map function returns Some value
// 2. returns a Failure with an error message when the function maps to None
let validatorM fmap error value : ValidationResult<_,_> =
    let result = fmap value
    match result with
    | Some value -> Success value
    | None -> Failure (List.singleton error)

// A validator to pull a required field out of a data map
let requiredValue key data =
    data |> validatorM (Map.tryFind key) (sprintf "%s is required" key)

// A validator to pull and optional field out of a data map
let optionalValue key data =
    data |> validatorM (Map.tryFind key >> Some) ("")

open System.Text.RegularExpressions
// A validator that matches a regular expresion
let matchesRegex (regex:Regex) msg v = 
    validator regex.IsMatch msg v

// a validator that checks to make sure a phone number string is in the correct format
// notice the return type is a PhoneNumber rather than a string
let isPhoneNumber phoneNumber =
    let regex = Regex("^(1\-)?((\(\d{3}\) ?)|([\d\w]{3}-))?[\d\w]{3}-[\d\w]{4}$",RegexOptions.IgnoreCase)
    phoneNumber 
    |> matchesRegex regex "not a valid phone number"
    |> Choice.map PhoneNumber

// a validator that checks to make sure an email string is in the correct format
// notice the return type is a EmailAddress rather than a string
let isEmailAddress email =
    let regex = Regex("^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$",RegexOptions.IgnoreCase)
    email 
    |> matchesRegex regex "not a valid email address"
    |> Choice.map EmailAddress


requiredValue "firstName" data // <-- Choice1Of2 "Tracy"
requiredValue "firstName" badData' // <-- Choice2Of2 ["firstName is required"]
optionalValue "phone" data // <-- Choice1Of2 (Some "1-800-REA-LTYS")
optionalValue "notAKey" data // <-- Choicee1Of2 None
isPhoneNumber "1-800-REA-LTYS" // <-- Choice1Of2 (PhoneNumber "1-800-REA-LTYS")
isPhoneNumber "nope" // <-- Chioce2Of2 "not a valid phone number"
isEmailAddress "contact@realtyshares.com" // <-- Choice1Of2 (EmailAddress "contact@realtyshares.com")
isEmailAddress "nope" // <-- Chioce2Of2 "not a valid email address"


// can we combine optionalValue and isPhoneNumber? Yes, but eww...
let validatePhoneNumber' data =
    data 
    |> optionalValue "phone"
    |> Choice.bind (function 
        | Some phoneNumber ->
            match isPhoneNumber phoneNumber with
            | Success p -> Success (Some p)
            | Failure msg -> Failure msg
        | None -> 
            Success None)

validatePhoneNumber' data // <-- Choice1Of2 (Some (PhoneNumber "1-800-REA-LTYS"))
validatePhoneNumber' badData' // <-- Choice1Of2 None
validatePhoneNumber' (Map.ofList ["phone", "not a phone number"]) // <-- Choice2Of2 "not a valid phone number"

// how about a ChoiceOption monad?
module ChoiceOption =
    let bind fn =
        Choice.bind (function 
        | Some value -> fn value
        | None -> Success None)

// a little better    
let validatePhoneNumber'' data =
    data 
    |> optionalValue "phone"
    |> ChoiceOption.bind (isPhoneNumber >> Choice.map Some)

// try it out
validatePhoneNumber'' data // <-- Choice1Of2 (Some (PhoneNumber "1-800-REA-LTYS"))
validatePhoneNumber'' badData' // <-- Choice1Of2 None
validatePhoneNumber'' (Map.ofList ["phone", "not a phone number"]) // <-- Choice2Of2 "not a valid phone number"


// let's make a new combinator
let andAlso validate = ChoiceOption.bind (validate >> Choice.map Some)

// Third time's the charm. Good enough!    
let validatePhoneNumber''' data =
    data 
    |> optionalValue "phone"
    |> andAlso isPhoneNumber

// we can also use "point-free" style
let validateEmail''' =
    optionalValue "email"
    >> andAlso isEmailAddress

// now let's implement validatePerson one more time using a stronger domain model that uses PhoneNumbers and EmailAddresses
type Person' = 
    { firstName : string
      lastName : string
      address1 : string
      address2 : string option
      city : string
      state : string
      zip: string
      phone : PhoneNumber option 
      email : EmailAddress option }

let validatePerson''' (data:Map<string,string>) =
    (fun firstName lastName 
         address1 address2 city state zip
         phone email ->
        { firstName = firstName
          lastName = lastName
          address1 = address1
          address2 = address2 // <-- notice address2 is a string option this time
          city = city
          state = state
          zip = zip
          phone = phone // <-- notice phone is a PhoneNumber option, just like the domain model expects
          email = email // <-- notice email is an EmailAddress option, just like the domain model expects 
          })
    <!> requiredValue "firstName" data
    <*> requiredValue "lastName" data
    <*> requiredValue "address1" data
    <*> optionalValue "address2" data
    <*> requiredValue "city" data
    <*> requiredValue "state" data
    <*> requiredValue "zip" data
    <*> validatePhoneNumber''' data
    <*> validateEmail''' data
    
// let's test it!
validatePerson''' data // <-- Success! Now, with a more strongly-typed domain model!
validatePerson''' badData // <-- Failure! A whole mess of errors
validatePerson''' betterData // <-- Failure! Fewer errors

// Congratulations! You've learned applicative-style validation. Have a free monad!

// Next Step: An introduction to the standard Validation library in validation-part2.fsx


