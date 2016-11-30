(*
TODO list
- convert expression to F# function: float -> float
- convert string to expression
- recursion
? Unary, Binary, Ternary model for command
*)

// expression
// ADD, MUL
//type Command = 
//    | Add
//    | Mul
//
//type Expression =
//    | Constant of float
//    | X
//    | SOME of Command * Expression * Expression 
//
//let foo = SOME(Add,Constant(10.0),SOME(Mul,X,Constant(42.0)))
//
//type Expr =
//    | Const of float
//    | X
//    | Add of Expr * Expr 
//    | Mul of Expr * Expr 
//
//let bar = Add(Const(10.0),Mul(X,Const(42.0)))
//
//"ADD(10.0,MUL(42,X))"
//
//// take an expression, return a function float -> float
//
//let rec Compute (expression: Expr) = 
//    match expression with 
//    | Const c -> (fun el -> c) 
//    | X -> (fun el -> el)
//    | Add (exp1,exp2) -> 
//        let f1 = Compute exp1
//        let f2 = Compute exp2
//        fun x -> f1 x + f2 x
//    | Mul (exp1,exp2) -> 
//        let f1 = Compute exp1
//        let f2 = Compute exp2
//        fun x -> f1 x * f2 x
//         
//
//let fooBar = fun x -> x * 2.0
//
//let f = Compute bar
//f 1.0 = 52.0
//
//let g = Compute (Const(42.0))
//g 1.0 = 42.0
//
//let h = Compute(X)
//h 1.0 = 1.0
//
//
//type Program () =
//
//    member this.Calculate (x: float) =
//        42.0 * x + 10.0
//    
//    member this.SmartCalculate (formula: string) (x: float) =
//       
//        0.0 
//
//let program = Program ()
////program.Calculate(1.0)
//let result = program.SmartCalculate "ADD(10.0,MUL(42,X))" 1.0
//result = 52.0
//
//let testCase = SOME(Add, SOME( Mul, X, Constant(42.0)), Constant(10.0))
//
//// end goal: be able to replace 42.0 * x + 10.0 at run time
// with "any" formula using x, from a text file.
// "any" formula will use our own language, for instance
// "ADD(10.0,MUL(42,X))
// text file doesn't matter, pass a string and execute 

// Support ADD, SUB, MUL, DIV, arbitrary complex expressions
// Handle floats, we need X and Constants
//
//#I "../packages"
//#r @"FParsec.1.0.2\lib\net40-client\FParsecCS.dll"
//#r @"FParsec.1.0.2\lib\net40-client\FParsec.dll"
//
//open FParsec
//
//let test p str =
//    match run p str with
//    | Success(result, _, _)   -> printfn "Success: %A" result
//    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg
//
//let customParseFloat = pfloat
//
//test customParseFloat "123.4"
//test customParseFloat "NOPE"
//
//let parseAdd = pstring "ADD"
//
//test parseAdd "ADD 1 2"
//test parseAdd "1 ADD 1 2"

// http://brandewinder.com/2016/02/20/converting-dsl-to-fsharp-code-part-1/

//let combined = customParseFloat <|> parseAdd
type Expression =
    | X
    | Constant of float
    | Add of Expression * Expression
    | Mul of Expression * Expression

let rec interpret (ex:Expression) =
    match ex with
    | X -> fun (x:float) -> x
    | Constant(value) -> fun (x:float) -> value
    | Add(leftExpression,rightExpression) ->
        let left = interpret leftExpression
        let right = interpret rightExpression
        fun (x:float) -> left x + right x
    | Mul(leftExpression,rightExpression) ->
        let left = interpret leftExpression
        let right = interpret rightExpression
        fun (x:float) -> left x * right x

#I @"../packages/"
#r @"FParsec.1.0.2\lib\net40-client\FParsecCS.dll"
#r @"FParsec.1.0.2\lib\net40-client\FParsec.dll"
open FParsec

let test parser text =
    match (run parser text) with
    | Success(result,_,_) -> printfn "Success: %A" result
    | Failure(_,error,_) -> printfn "Error: %A" error

let parseConstant = pfloat |>> Constant

test parseConstant "123.45"
test parseConstant "nope"

let parseVariable = stringReturn "x" X

test parseVariable "x"
test parseVariable "nope"

let parseExpression = parseVariable <|> parseConstant

test parseExpression "123.45"
test parseExpression "x"
test parseExpression "nope"

let parseExpressionsPair =
    between 
        (pstring "(")
        (pstring ")")
        (tuple2 
            (parseExpression .>> pstring ",") 
            parseExpression)

let parseAddition =
    pstring "add" >>.
    parseExpressionsPair
    |>> Add

let parseMultiplication =
    pstring "mul" >>.
    parseExpressionsPair
    |>> Mul

test parseAddition "add(1,x)"

let fullParser = parseVariable <|> parseConstant <|> parseAddition <|> parseMultiplication

type Program () =

    member this.Run (x:float,code:string) = 
        match (run fullParser code) with
        | Failure(message,_,_) -> 
            printfn "Malformed code: %s" message
        | Success(expression,_,_) ->
            let f = interpret expression
            let result = f x
            printfn "Result: %.2f" result

let program = Program()

let code = "add(x,42)"
program.Run(10.0,code)

let code2 = "mul(x,x)"
program.Run(10.0,code2)