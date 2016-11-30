//http://rosalind.info/problems/dna/
let i1 = "AGCTTTTCATTCTGACTGCAACGGGCAATATGTCTCTGTGTGGATTAAAAAAAGAGTGTCTGATAGCAGC" 

// first approximation
let countDna1 input =   
 
    let mutable acnt = 0
    let mutable gcnt = 0
    let mutable ccnt = 0
    let mutable tcnt = 0

    for a in input do
        match a with 
        | 'A'-> acnt <- (acnt + 1)   
        | 'G'-> gcnt <- (gcnt + 1)   
        | 'C'-> ccnt <- (ccnt + 1)   
        | 'T'-> tcnt <- (tcnt + 1)   
        | _ -> failwith "Unexpected char"

//    [acnt; ccnt; gcnt; tcnt] 
//    |> Seq.map string
//    |> String.concat " " 

    sprintf "%i %i %i %i" acnt ccnt gcnt tcnt

let expected = "20 12 17 21"
let actual = countDna1 i1
actual = expected

let areEqual expout functionality input =
    expout = functionality input

areEqual expected countDna1 i1

countDna1 "Hello"



// second approximation
let countDna2 input=    
     // group by char
    input 
    |> Seq.groupBy id
    // second param -> get length of the sequence
    |> Seq.map (fun (x,y) -> (x, Seq.length y) )


// third approximation
let countdna3 input =    
    Seq.countBy id input

//let countdna4 = Seq.countBy id

countdna3 i1


// http://rosalind.info/problems/rna/
let i2 = "GATGGAACTTGACTACGTAAATT" 

open System

let rnaToDna input =
    let sq = Seq.map (fun c -> match c with 
                               | 'T'-> 'U'
                               | _ -> c) (input |> Seq.toList)
    sq |> String.Concat

rnaToDna i2


// http://rosalind.info/problems/revc/
let i3 = "AAAACCCGGT" 

let reverseComplement input =
    let sq = Seq.map (fun c -> match c with 
                               | 'A'-> 'T'
                               | 'T' -> 'A'
                               | 'C' -> 'G'
                               | 'G' -> 'C')
                                (input |> Seq.toList)
    sq |> Seq.rev |> String.Concat

reverseComplement i3
