open System


// a hackerrank utils library with a "mock" version of System.Console
#load "../../utils.fsx"

open Utils

let input = 
    [
        "3"
    ]

// loads our "mock" console with the above input
Utils.resetConsole(input)

let rec fibn n =
    match n with
    | 0 | 1 -> 0
    | 2 -> 1
    | n -> fibn (n-1) + fibn (n-2)

open System.Collections.Generic

let n = Console.ReadLine() |> int

printfn "%d" <| fibn n