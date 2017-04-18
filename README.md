# F# Training

## About: Getting RealtyShares devs started with F#

This repo has:
- F# Training process
- Koans and Katas (Dojos) compiled by Mark.
- Some code exercises Alex worked on during mentorship sessions with Mathias (the fancy font I am using is Fira Code: https://github.com/tonsky/FiraCode). 

### Time estimate: 2 weeks.

1) Work through http://www.fsharpworkshop.com/

2) Main topics to learn and practice: 

- Funtional Programming paradigm

https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/

- Code organization using FSharp modules

https://fsharpforfunandprofit.com/series/a-recipe-for-a-functional-app.html

https://vimeo.com/100960317

- Exploratory programming with FSI and the scripting environment

http://brandewinder.com/2016/02/06/10-fsharp-scripting-tips/

- Pattern matching (particularly over DU's, tuples, lists, and perhaps records). Active patterns are less important but would be good to touch on if time permits.
- Async computation expressions and the FSharp.Async core library. 

https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/

http://tomasp.net/blog/csharp-async-gotchas.aspx/

http://tomasp.net/blog/async-csharp-differences.aspx/

- Railway-oriented programming.

http://fsharpforfunandprofit.com/rop/

http://fsharpforfunandprofit.com/posts/railway-oriented-programming-carbonated/

- Monadic style and how it applies to F# option types, seq, async, (or even asyncSeq) incl. Map, Bind, Apply

https://fsharpforfunandprofit.com/series/map-and-bind-and-apply-oh-my.html

- Recursive folds, catamorphisms (fold generalization)

https://fsharpforfunandprofit.com/series/recursive-types-and-folds.html

https://lorgonblog.wordpress.com/2008/04/05/catamorphisms-part-one/

- Applicative style and how it applies to data validation

- Agents/MailboxProcessors, Actors

https://www.youtube.com/watch?v=RiWXo_5CAvg

- DDD, ie. modeling a business domain using the F# type system

https://www.youtube.com/watch?v=970nkg60lHs

- Type Providers

https://vimeo.com/104896802

- Funcional Architecture

https://vimeo.com/161131920

https://www.youtube.com/watch?v=nxIRlf4AtcA


* I understand monads/applicative functors are pretty abstract subjects. It's sufficient to understand how to use common ones like async, seq, maybe and either.

3) Solve a few DoJos in https://github.com/RealtyShares/fsharp-training/blob/master/KoansAndKatas/koanwiki.md
## Other resources:
- [Fable](https://github.com/kunjee17/awesome-fable)

## Relevant libraries:

We make relatively heavy use of the following F# libraries. There's a conceptual learning curve associated with each, so it would be good to work through a few small Katas that incorporate them in some way:

- Suave (for HTTP/JSON APIs)

https://www.youtube.com/watch?v=ujxwW6fFXOc

https://vimeo.com/171704578

- FSharpx.Validation (for general data validation, particularly the applicative operators: <!> <*> <* *>)

- FParsec (for basic parsing tasks)

https://vimeo.com/171704565


## Chargeable options (Important - please confirm with your manger)
- Video classes: https://fsharp.tv/courses/  incl. free class: https://learn.decacoder.com/course/uid-01-begin-fsharp
- Mentor: Mathias - mathias.brandewinder@gmail.com. 
