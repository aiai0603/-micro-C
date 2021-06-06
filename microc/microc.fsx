
#r "FsLexYacc.Runtime.dll";;
#load "Absyn.fs" "CPar.fs" "CLex.fs" "Parse.fs" "Machine.fs" "Comp.fs" ;;

let fromFile = Parse.fromFile
let compileToFile = Comp.compileToFile

let args = System.Environment.GetCommandLineArgs();;

let _ = printf "Micro-C Stack VM compiler v 1.0.0.1 of 2017-12-2\n";;

let _ = 
   if args.Length > 1 then
      let source = args.[1]
      let stem = 
          if source.EndsWith(".c")
             then source.Substring(0,source.Length-2) 
             else source
      let target = stem + ".out"

      printf "Compiling %s to %s\n" source target
      
      try  (let instrs = compileToFile (fromFile source) target;
            printf "StackVM code:\n%A\n" instrs;
            printf "Numeric code in file:\n\t%s\n Please run with VM.\n" target;
            )
      with Failure msg -> printf "ERROR: %s\n" msg
   else
      printf "Usage: microc.exe <source file> \n";
