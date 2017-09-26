module Global

type Page =
  | Home
  | Counter
  | About
  | Lights

let toHash page =
  match page with
  | About -> "#about"
  | Counter -> "#counter"
  | Home -> "#home"
  | Lights -> "#lights"
