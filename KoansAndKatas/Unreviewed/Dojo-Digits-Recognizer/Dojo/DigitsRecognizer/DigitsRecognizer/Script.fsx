
open System
open System.IO

let makeAppRelative fileName = System.IO.Path.Combine( __SOURCE_DIRECTORY__, fileName)
let readLines path = File.ReadAllLines(makeAppRelative path)

let training = readLines "trainingsample.csv"
let validation = readLines "validationsample.csv"
 
let train = training |> Array.map (fun (s:string) -> s.Split(','))
let valid = validation |> Array.map (fun (s:string) -> s.Split(','))
 
let trainData = train.[1..]
let validData = valid.[1..]

let mapStringToInt doubleArray =
    doubleArray 
    |> Array.map (fun (array:string[]) -> array |> Array.map (fun (s:string) -> Convert.ToInt32(s)))
 
let trainDataInts = mapStringToInt trainData
let validDataInts = mapStringToInt validData

type DigitRecord = { Label:int; Pixels:int[] }
[<Measure>] type percentage

let mapArrayToRecord (doubleArray : int[][]) = 
    doubleArray 
    |> Array.map (fun (array:int[]) -> { Label = array.[0]; Pixels = array.[1..] })
 
let trainRecords = mapArrayToRecord trainDataInts
let validRecords = mapArrayToRecord validDataInts

let distance (P1: int[]) (P2: int[]) =
    let squared = Array.map2 (fun p1 p2 -> (p1-p2)*(p1-p2)) P1 P2
    let sum = Array.sum squared
    sqrt (double sum)

let classify (unknown:int[]) =
    let distances = trainRecords |> Array.map (fun (record:DigitRecord) -> (record.Label, distance record.Pixels unknown))
    distances 
    |> Array.minBy(fun x -> snd x) 
    |> fst

let verify (valid: DigitRecord[]) =
   let sum = valid |> Array.map (fun record -> System.Convert.ToInt32((classify record.Pixels) = record.Label)) |> Array.sum
   let percentage = sum * 100<percentage> / Array.length valid
   percentage

verify validRecords