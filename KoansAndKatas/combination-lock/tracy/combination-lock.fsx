open System

module ComboLock = 
    // a type representing a combo-lock state transition event
    type StateTransitionEvent = 
        | Reset
        | Unlocked
        | ResetFailed
        
    // a function that produces a new combo-lock and an event source that clients can listen to
    let makeComboLock (unlockCombo: int list) (resetCombo:int list) = 
        // the event source
        let stateTransition = Event<_>()

        // an "agent" to process our combo lock input
        let agent = 
            MailboxProcessor<int>.Start(fun mq ->
                // next state logic for "accepting input" state
                let rec acceptingInput (acc) = 
                    async {
                        let! input = mq.Receive()
                        match acc with
                        | head::[] when input = head -> 
                            stateTransition.Trigger Unlocked
                            return! unlocked resetCombo
                        | head::tail when input = head -> 
                            return! acceptingInput tail
                        | _ -> 
                            stateTransition.Trigger Reset
                            return! reset()
                    }        
                // next state logic for "unlocked" state
                and unlocked acc = 
                    async {
                        let! input = mq.Receive()
                        match acc with
                        | head::[] when head = input -> 
                            stateTransition.Trigger Reset
                            return! reset()
                        | head::tail when head = input -> 
                            return! unlocked tail
                        | _ -> 
                            stateTransition.Trigger ResetFailed
                            return! resetFailed()
                    }
                and reset() = acceptingInput unlockCombo
                and resetFailed() = unlocked resetCombo

                // initialize the combo-lock in the "unlocked" state
                unlocked resetCombo)
        // trigger a reset event            
        stateTransition.Trigger Rese
        // return the agent and the event source
        agent,stateTransition.Publish

// create new combolock and event source
let (comboLock,stateTransitionEventSource) = ComboLock.makeComboLock [4;3;2] [0;0;0]

// listen for events
function
| ComboLock.Reset -> printfn "Lock has been reset!"
| ComboLock.Unlocked -> printfn "Lock has been unlocked!"
| ComboLock.ResetFailed -> printfn "Reset failed! Please reenter reset combination"
|> stateTransitionEventSource.Subscribe 

/// TEST IT OUT!

// reset the lock
comboLock.Post(0)
comboLock.Post(0)
comboLock.Post(0)

// test happy-path
comboLock.Post(4)
comboLock.Post(3)
comboLock.Post(2)

// test error case
[0;0;0;4;4;4] |> List.iter comboLock.Post

// test reset error case
[0;0;0;4;3;2;0;0;1] |> List.iter comboLock.Post
