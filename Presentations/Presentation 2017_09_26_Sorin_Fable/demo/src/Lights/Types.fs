module Lights.Types
type ButtonState = bool
type ButtonId = int


// Business model
type Model = ButtonState list

type Msg = 
    | Toggle of ButtonId
