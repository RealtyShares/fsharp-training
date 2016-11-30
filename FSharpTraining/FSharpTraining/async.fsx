//Hello sir,
//
//I attached the file I was working on today, for reference.
//
//Otherwise:
//
//1) the fancy font I am using is Fira Code: https://github.com/tonsky/FiraCode
//
//2) you should watch / take a look at the 2 following references on ROP:
//
//http://fsharpforfunandprofit.com/rop/
//http://fsharpforfunandprofit.com/posts/railway-oriented-programming-carbonated/
//
//The video in particular is a good starting point. The second post is more "bonus".
//
//3) async: this post is a good complement to what we did today:
//https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/
//
//I need to think of "homework"; I suspect you'll be busy with FParsec for now, so I'll sleep on it first. 
//
//Cheers, and hope the exercise we ended up with today was not too brutal!



// async uses a computation expression

// Font: Fira Code, using ligatures

let helloAsync = 
    async {
        do! Async.Sleep 1000
        return "Hello"
    }

helloAsync |> Async.RunSynchronously

let task = 
    async {
        printfn "Starting"
        let! hello = helloAsync
        printfn "%s" hello
        do! Async.Sleep 1000
        printfn "Finished"
    }

task |> Async.Start

let slowTask i = 
    async {
        printfn "Start %i" i
        do! Async.Sleep 1000
        printfn "Done %i" i
    }

[ 1 .. 10 ] 
|> List.map slowTask
|> Async.Parallel
|> Async.RunSynchronously

let longTask = 
    async { 
        do! Async.Sleep 5000
        return 42 
        }

let result = longTask |> Async.StartAsTask
result.IsCompleted
result.AsyncState
result.Result

// not on the thread pool, unlike Async.Start
Async.StartImmediate

let rec timer () = 
    async {
        printfn "Ping!"
        do! Async.Sleep 1000
        return! timer ()
    }

timer () |> Async.Start

42

type Msg = 
    | Ping
    | Pong

printfn "%A" Ping


let rec ping () = 
    async {
        printfn "Ping!"
        do! Async.Sleep 1000
        return! pong ()
    }
and pong () = 
    async {
        printfn "Pong!"
        do! Async.Sleep 1000
        return! ping ()
    }

ping () |> Async.Start

// check -> in the codebase
// error model

// crawler: as an exercise

(*
Mailbox: agent / actor based model
*)

type Message = 
    | Content of string
    | Kill

let mailbox = 
    new MailboxProcessor<Message>(fun inbox ->
        let rec loop () = 
            async {
                let! msg = inbox.Receive ()
                match msg with 
                | Content(txt) -> 
                    printfn "%s" txt
                    return! loop ()
                | Kill -> 
                    printfn "DONE"
                    ignore ()
        }
        loop ()
    )

mailbox.Start ()

mailbox.Post (Content "Hello")

// other processes: Akka.NET

