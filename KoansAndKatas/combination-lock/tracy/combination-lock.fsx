open System

module ComboLock = 
    type StateTransitionEvent = 
        | Reset
        | Unlocked
        | ResetFailed
        
    let makeComboLock (combo: int list) (resetCombo:int list) = 
        let stateTransition = Event<_>()
        let agent = 
            MailboxProcessor<int>.Start(fun mq ->
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
                and reset() = acceptingInput combo
                and resetFailed() = unlocked resetCombo

                unlocked resetCombo)
        stateTransition.Trigger Reset
        agent,stateTransition.Publish

let (comboLock,stateTransitionEventSource) = ComboLock.makeComboLock [4;3;2] [0;0;0]

function
| ComboLock.Reset -> printfn "Lock has been reset!"
| ComboLock.Unlocked -> printfn "Lock has been unlocked!"
| ComboLock.ResetFailed -> printfn "Reset failed! Please reenter reset combination"
|> stateTransitionEventSource.Subscribe 

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
