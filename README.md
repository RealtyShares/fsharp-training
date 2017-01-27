# About
This repo has some code exercises Alex worked on during mentoship session with Mathias. All relevant and somewhat relevant info is compiled below. The ultimate goal of this repo is to assist RealtyShares devs with learning F# so please feel free to modify the repo as much as you want

# Reading materials

Given our discussion on functions, pipelines and composition, I think this would be good reading material:
https://fsharpforfunandprofit.com/posts/thinking-functionally-intro/
It's a long series, but it's probably worth it :) In general, this website is a gold mine for F#.

1) the fancy font I am using is Fira Code: https://github.com/tonsky/FiraCode
2) you should watch / take a look at the 2 following references on ROP:

http://fsharpforfunandprofit.com/rop/
http://fsharpforfunandprofit.com/posts/railway-oriented-programming-carbonated/

The video in particular is a good starting point. The second post is more "bonus".

3) async: this post is a good complement to what we did today:
https://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/



# Contact
Mathias - mathias.brandewinder@gmail.com. Important - before reaching out to him please confirm with your manger


# Topics to look into 
(kudos to Tracy for coming up with the list): 
- Pattern matching (particularly over DU's, tuples, lists, and perhaps records). Active patterns are less important but would be good to touch on if time permits.
- Async computation expressions and the FSharp.Async core library. 
- Railway-oriented programming.
- Monadic style and how it applies to F# option types, seq, async, (or even asyncSeq) 
- Applicative style and how it applies to data validation
- Agents/MailboxProcessors
- DDD, ie. modeling a business domain using the F# type system
- Code organization using FSharp modules
- Exploratory programming with FSI and the scripting environment

* I understand monads/applicative functors are pretty abstract subjects. It's sufficient to understand how to use common ones like async, seq, maybe and either.

## Relevant libraries:

We make relatively heavy use of the following F# libraries. There's a conceptual learning curve associated with each, so it would be good to work through a few small Katas that incorporate them in some way:

- Suave (for HTTP/JSON APIs)
- FSharpx.Validation (for general data validation, particularly the applicative operators: <!> <*> <* *>)
- FParsec (for basic parsing tasks)