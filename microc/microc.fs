
let fromFile = Parse.fromFile
let compileToFile = Comp.compileToFile

let argv = System.Environment.GetCommandLineArgs();;

let args = Array.filter ((<>) "-g") argv

let _ = printfn "Micro-C Stack VM compiler v 1.1.0 of 2021-3-22";;

let _ = 
   if args.Length > 1 then
      let source = args.[1]
      let stem = 
          if source.EndsWith(".c")
             then source.Substring(0,source.Length-2) 
             else source
      let target = stem + ".out"

      printfn "Compiling %s to %s" source target
      
      try  (let instrs = compileToFile (fromFile source) target;
            // printf "StackVM code:\n%A\n" instrs;
            printfn "Numeric code saved in file:\n\t%s\nPlease run with VM." target;
            )
      with Failure msg -> printfn "ERROR: %s" msg
   else
      printfn "Usage: microc.exe <source file>";
