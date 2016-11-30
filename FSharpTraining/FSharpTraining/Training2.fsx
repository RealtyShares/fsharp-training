// what to do to implement Seq.countBy

// F# lists -> rec and pattern matching
// Seq.fold -> same as IEnumerable.Aggregate<..>

(*
3 collections: Seq, Array, List
List <> List<T> C#, LinkedList
if you want List<T> : ResizeArray<T>
list = awesome for linear pass, awful for random index access
*)

let xs = [ 1; 2; 3; 4; 5 ]

xs |> List.map (fun x -> x + 1)
xs 
|> Seq.map (fun x -> x + 1) 
|> Seq.toList 
|> List.map (fun x -> x - 1)

// lists and pattern matching

// Seq is lazy, list is eager
#time "on"
seq { 1 .. 1000000 }

// list is immutable, head + tail
// cons operator ::
let head :: tail = xs
let ys = 42 :: xs

let workWith (xs:int list) =
    match xs with
    | [] -> "empty"
    | hd::tl -> "head and tail"

let deeper (xs:int list) =
    match xs with
    | [] -> 0
    | first::second::rest -> 2
    | head::tail -> 1

let rec max (xs: int list) (accum:int) =
    match xs with
    | [] -> accum
    | head::tail -> 
//        let res = max tail accum
//        if (head > res) 
//        then head 
//        else res
        let acc = 
            if head > accum
            then head
            else accum
        max tail acc

type Result =
    | NotDefined
    | Value of int

let wrappedmax (xs:int list) =

    let rec search (xs:int list) accum =
        match xs with
        | [] -> accum
        | head::tail -> 
            let acc = 
                if head > accum
                then head
                else accum
            search tail acc
    
    match xs with
    | [] -> 
        NotDefined
        // failwith("The input sequence was empty")  
    | _ ->
        let maxValue = search xs (System.Int32.MinValue)
        Value(maxValue)

(List.empty<int>) |> wrappedmax 

wrappedmax [ -1; -2 ] = Value(-1)
wrappedmax [] = NotDefined

let handleMax (xs:int list) =
    let res = wrappedmax xs
    match res with
    | NotDefined -> "what should I do"
    | Value(x) -> "Max"

// Result ~ Option
type Option<'T> =
    | None
    | Some of 'T

type MyList =
    | Empty 
    | Full of int * MyList

type Tree =
    | Leaf of int
    | Branch of Tree * Tree

let tree =
    Branch(
        Leaf(1),
        Branch(
            Leaf(2),
            Leaf(3)))

// "homework"
// write List.rev: reverse a list
// write sum of Tree


// Discriminated Union

type Boolean =
    | True
    | False



// SICP: Structure and Interpretation of COmputer Programs

// Ploeh / Mark Seeman, short variable names in FP
// xs as a variable name?

// match list with | head::tail
// match with | x :: xs


//let rec max2 (xs: int list) =
//    match xs with
//    | [] -> accum
//    | head::tail -> 
//        let res = max tail 
//        if (head > res) 
//        then head 
//        else res

let test = [ 1; 42; 5; 3 ]
max test 100 = 42
max test 0 = 42

max [] 0

// wrapper! often the case for recursive functions
