module Lights.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Lights.Types
open Lights.State

let showButton dispatcher indexOfButton valueOfButton =
  div
    [ classList
        [ "lightbutton", true
          "on", valueOfButton]
      OnClick (fun _e -> Toggle indexOfButton |> dispatcher) ]
    []


let root model dispatch =
   model
  |> List.mapi (showButton dispatch)
  |> div [] 