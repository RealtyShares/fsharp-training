module Lights.State

open Elmish
open Lights.Types

let init () =
    [true; false; true; true; false; true], Cmd.none


let update msg model =
    match msg with
    | Toggle buttonIndex ->
        let newModel =
            model
            |> List.mapi (fun i buttonValue ->
                if abs(i - buttonIndex) < 2
                then not buttonValue
                else buttonValue)
        newModel, Cmd.none