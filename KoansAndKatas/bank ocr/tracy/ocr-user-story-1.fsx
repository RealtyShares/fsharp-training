open System

// Solution to User Story 1: http://codingdojo.org/kata/BankOCR/

module Ocr = 
    let digitReference = 
       " _     _  _     _  _  _  _  _ \n"
     + "| |  | _| _||_||_ |_   ||_||_|\n"
     + "|_|  ||_  _|  | _||_|  ||_| _|\n"

    /// splits 3 lines of text containing 7-segment encoded digits into a sequence of 3x3 7-segment digits
    let toOcrSeq (rawText:string) = 
        // parse the raw input a rectangular 2d array of 3 char strings for easy lookup
        let parsedDigits =
            rawText
            |> fun s -> s.Split('\n')
            |> Seq.map (Seq.chunkBySize 3)
            |> Seq.map Seq.toArray
            |> Seq.toArray

        // the total number of digits in the input text
        let seqLen = parsedDigits.[0].Length

        // split the chunked input into a seqence of 3x3 chars, which represents an ascii-encoded 7-segment digit
        [0..(seqLen-1)]
        |> Seq.map (fun i ->
            [|parsedDigits.[0].[i]
              parsedDigits.[1].[i]
              parsedDigits.[2].[i]|])
        |> Seq.toArray  

    /// a "lookup function that accepts a 3x3 ocr reprentation of a digit and returns the corresponding decimal value"
    let lookupDigit (ocrDigit:char[][]) = 
        let digitMap = 
            toOcrSeq digitReference
            |> Seq.mapi (fun i digit -> digit,i)
            |> Map.ofSeq
        digitMap |> Map.tryFind ocrDigit

    /// helper function to print a seq of integers to a string
    let printAccountNumber (acctNumber:(int option) seq) = 
        acctNumber
        |> Seq.map (function
            | Some digit -> string digit
            | None -> "?")
        |> String.concat ""
    

open System.IO
// load sample file
let sampleFileLines = File.ReadAllLines (__SOURCE_DIRECTORY__ + "/../sample.txt")

// parse the file into individual account numbers, 3 lines of text each
let sampleAccountNumbers = 
    sampleFileLines
    |> Seq.chunkBySize 4
    |> Seq.map (Seq.take 3) // throw away every 4th line
    |> Seq.map (String.concat "\n")
    |> Seq.toArray

sampleAccountNumbers
// iterate through each 7-segment encoded account number in the sample file
|> Seq.map (fun accountNumberText ->
    // convert from 7-segment encoding to decimal
    let acctNumberDigits = 
        accountNumberText
        |> Ocr.toOcrSeq
        |> Seq.map (Ocr.lookupDigit)
        |> Seq.toArray
    // print the account number from a seq of ints
    Ocr.printAccountNumber acctNumberDigits)
// print the results
|> Seq.iteri (fun i accountNumber ->
    printfn "test case %d: %s" (i+1) accountNumber
    )
