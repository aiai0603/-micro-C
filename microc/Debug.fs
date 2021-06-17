module Debug

let argv = System.Environment.GetCommandLineArgs()
let mutable debug = false
try
    let _ = Array.find ((=) "-g") argv
    debug <- true
with :? System.Exception as ex -> ()

let info action = 
  if debug then action () else ()