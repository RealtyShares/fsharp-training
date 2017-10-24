module Utils

open System

type MockConsole = 
    { ReadLine : unit -> string }

let mutable Console = Unchecked.defaultof<MockConsole>

let private makeConsole (inputSeq: _ seq) = 
    let enumerator = inputSeq.GetEnumerator()
    { ReadLine = fun () ->
        let hasMore = enumerator.MoveNext()
        if hasMore 
            then enumerator.Current |> box |> string
            else Unchecked.defaultof<String> }

let resetConsole inputSeq = 
    Console <- makeConsole inputSeq

let rec readLines () = 
    match Console.ReadLine() with
    | null -> []
    | line -> line::readLines()    