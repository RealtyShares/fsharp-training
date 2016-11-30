// Seq.map, Seq.filter, Seq.fold
// homework: implement max and maxby using only these 3 functions (IMMUTABLE!)

// maxBy: take a sequence and an arbitrary function, 
// and order by the result of applying the function to items


let maxby comp xs = 
    match xs with
    | [] -> 
        None
    | _  -> let res = Seq.fold 
                        (fun acc elem -> comp acc elem)
                        System.Int32.MinValue xs 
            Some(res)


let maxby2 projection =
    Seq.fold (fun acc elem -> 
        match acc with
        | None -> Some(elem)
        | Some accValue -> 
            if (projection accValue > projection elem)
            then Some(accValue) 
            else Some(elem))
            None




let f (s:string) = s.Length

let uniqueElements (s:string) = 
    Seq.distinct s |> Seq.map string |> String.concat ""


let xs = [ "A"; "ABC"; "ABBBBBB" ]
maxby2 f xs = Some("ABC")
maxby2 (uniqueElements >> f) xs = Some("ABC")

// lexicographic
"ABC" > "AB"

"AB" > "BA"
"A" > "B"

"ABC" > "ABB"

let intComparison = (fun a b -> if a > b then a else b)



maxby intComparison [-4;5;8] = Some(8)

let max = 
    maxby intComparison

max [-4;5;4] = Some(5)




// homework
// write List.rev: reverse a list
let reverseWrapped xs =
    let rec reverse xs accum = 
        match xs with
        | head::tail -> reverse tail (head::accum)
        | [] -> accum
    reverse xs []

reverseWrapped [1;2;3] = [3;2;1]

// write sum of Tree

type Tree =
    | Leaf of int
    | Branch of Tree * Tree

let tree =
    Branch(
        Leaf(1),
        Branch(
            Leaf(2),
            Leaf(3)))

let rec treeSum tree = 
    match tree with
        | Leaf l -> l
        | Branch (b,c) -> (treeSum b) + (treeSum c)

treeSum tree = 6

