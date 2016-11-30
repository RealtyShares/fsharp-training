// Async + tail recursion

type State =
    | Ping
    | Pong

let rec timer (value:State) = 
    async {
        printfn "%A" value
        do! Async.Sleep 1000

        let newValue = 
            match value with
            | Ping -> Pong
            | Pong -> Ping

        return! timer newValue
    } 

timer Ping |> Async.Start


