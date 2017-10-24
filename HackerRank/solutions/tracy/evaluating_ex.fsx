open System

// a hackerrank utils library with a "mock" version of System.Console
#load "../../utils.fsx"

open Utils

// copy/pasted from utils.fsx
let rec readLines () = 
    match Console.ReadLine() with
    | null -> []
    | line -> line::readLines()

let rec fac (n:int) =
    match n with
    | n when n > 1 -> n * fac(n-1)
    | _ -> 1

let expansion (x:float) =
    [0..9]
    |> List.map (fun i ->
        let numerator = Math.Pow(x,float i)
        let denominator = float <| fac i
        numerator / denominator)
    |> List.sum    


let input = 
   [4.0
    20.0000
    5.0000
    0.5000
    -0.5000]

// loads our "mock" console with the above input
Utils.resetConsole(input)

let iterations = Console.ReadLine() |> int

readLines()
|> List.take iterations
|> List.map (float >> expansion)
|> List.iter (printfn "%f")