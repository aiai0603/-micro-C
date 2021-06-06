(* File MicroC/ParseAndRun.fs *)

module ParseAndRun

let fromString = Parse.fromString

let fromFile = Parse.fromFile

// let tokens = Parse.tokens

let run = Interp.run
