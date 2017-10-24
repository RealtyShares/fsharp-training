open System

// a hackerrank utils library with a "mock" version of System.Console
#load "utils.fsx"

open Utils

// copy/pasted from utils.fsx
let rec readLines() = 
    seq {
        match Console.ReadLine() with
        | null -> ()
        | line -> 
            yield line
            yield! readLines()
    }

let input = 
    [
        //copy sample input from hackerrank here. you may have to tab a few times.
        1
        3
        4
        5
    ]

// loads our "mock" console with the above input
Utils.resetConsole(input)

let readALine = Console.ReadLine() |> int

readLines()
// .. do some stuff
|> Seq.iter (printfn "%s")