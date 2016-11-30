// printfn, printf -> Console.Out n has a newline

printfn "Hello"
 
1 + 2
let foo xs = xs |> Seq.sort

// tuples, not tuples in functions

let add x y = x + y

let oldSchoolAdd (x,y) = x + y

let add42 = add 42

add42 1

let add3things a b c = a + b + c

let f = add3things 1 

1 |> add 2 |> add 8 |> add 1

open System.IO

// File.WriteAllText(path,contents)

let saveTo path content = File.WriteAllText(path,content)

"my contents" |> saveTo "myPath"

let g (xs:string seq) = xs |> Seq.maxBy (fun x -> x.Length)

["Foe";"Food";"D"] |> g

let h (x:string) = x.ToUpperInvariant()

let doStuff data =
    data
    |> g    
    |> h

// don't do this :)
let doStuffBackwards data =
    h <| (g <| data)

// don't do this!
(h << g) ["Foe";"Food";"D"]

doStuff ["Foe";"Food";"D"]

// |> = "pipe-forward"

// composition
(g >> h) ["Foe";"Food";"D"]

let xuz = (g >> h)
xuz ["Foe";"Food";"D"]


// https://github.com/fsprojects/FSharp.Control.Reactive/blob/master/src/FSharp.Control.Reactive/Observable.fs#L200-L201

// DSL = domain specific languages
// http://martinfowler.com/books/dsl.html

type Customer = {
    Name:string
    Age:int
    }

let customer1 = { Name = "Joe"; Age = 42 }
let customer2 = { customer1 with Name = "Jack" }
let customer3 = { customer1 with Name = "Sasha" }
let customer4 = { customer1 with Name = "Sasha" }

// default: value-wise comparison
customer3 = customer4

let x = (1,2)
let y = (1,2)
x = y

//[<CustomEquality>]
//[<CustomComparison>]
//type Baz = 
//    {
//        Name:string
//    }
//    member this.Capitalize = this.Name.ToUpperInvariant()
//    override this.Equals(other) = false
//    override this.GetHashCode() = 123


// possible, mark mutable
// customer1.Name <- "HASFASHF"

let customers = 
    [   
        customer1
        customer2
        customer3
        customer4
    ]

customers |> List.sort

let olderThan age cust = cust.Age > age

let olderThan42 = olderThan 42

customers |> List.filter (fun c -> olderThan 35 c)

customers |> List.filter olderThan42

customers |> List.filter (olderThan 42)

let (|||>) x f = f x
1 |||> add 1


// can you rely

type Foo = { Bar : int [] }

let data = [| 1 .. 5 |]
let foo = { Bar = data }

data.[0] <- 42
//foo.Bar <- [| 1 |]

type Point2D =
   struct
      val X: float
      val Y: float
      new(x: float, y: float) = { X = x; Y = y }
   end

// type inference

// top to bottom, left to right

let mult x y = x * y

mult 1 2

// file ordering!

let xs = [| 1 .. 10 |]
xs.[4..]
xs.[..3]

[ 'a' .. 'z' ]

let sillyExample () = 
    
    Seq.map (fun x -> x.Length) ["A";"AB";"ABC"]

let fixedExample () = 
    
    ["A";"AB";"ABC"] |> Seq.map (fun x -> x.Length) 



//
//Given our discussion on functions, pipelines and composition, I think this would be good reading material:
//
//https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/
//
//It's a long series, but it's probably worth it :) In general, this website is a gold mine for F#.
//
//Have a great weekend,
//
//Mathias