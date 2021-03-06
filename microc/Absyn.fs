(* File MicroC/Absyn.fs
   Abstract syntax of micro-C, an imperative language.
   sestoft@itu.dk 2009-09-25

   Must precede Interp.fs, Comp.fs and Contcomp.fs in Solution Explorer
 *)

module Absyn

type typ =
  | TypB 
  | TypF
  | TypI                             (* Type int                    *)
  | TypC                             (* Type char                   *)
  | TypA of typ * int option         (* Array type                  *)
  | TypS                             (* Type string                *)
  | TypP of typ                      (* Pointer type                *)
  | TypeStruct of string
  | Lambda of  typ option * string * (typ * string) list * stmt
                                                                   
and expr =    
  | CreateI of string * int
  | CstI of int   (*constant int*)
  | ConstBool of bool
  | ConstString of string (*constant string*)
  | ConstChar of char (*constant char*) 
  | ConstFloat of float32
  | ConstNull                                                     
  | Access of access                 (* x    or  *p    or  a[e]     *)
  | Assign of access * expr          (* x=e  or  *p=e  or  a[e]=e   *)
  | Self of  access * string * expr
  | ToInt of expr
  | ToChar of expr
  | ToFloat of expr
  | Print of string * expr
  | Println of access
  | Addr of access                   (* &x   or  &*p   or  &a[e]    *)
  | Prim1 of string * expr           (* Unary primitive operator    *)
  | Prim2 of string * expr * expr    (* Binary primitive operator   *)
  | Prim3 of expr * expr * expr      
  | Andalso of expr * expr           (* Sequential and              *)
  | Orelse of expr * expr            (* Sequential or               *)
  | Call of string * expr list       (* Function call f(...)        *)
  | PrintHex of int * expr
  | Typeof of expr
  | Sizeof of expr  
  
  
  
                                                                   
and access =                                                       
  | AccVar of string                 (* Variable access        x    *) 
  | AccDeref of expr                 (* Pointer dereferencing  *p   *)
  | AccIndex of access * expr        (* Array indexing         a[e] *)
  | AccStruct of access * access
                                                                   
and stmt =                                                         
  | If of expr * stmt * stmt         (* Conditional                 *)
  | While of expr * stmt             (* While loop                  *)
  | DoWhile of  stmt * expr          (* DoWhile loop                *)
  | Expr of expr                     (* Expression statement   e;   *)
  | Return of expr option            (* Return from method          *)
  | Block of stmtordec list          (* Block: grouping and scope   *)
  | For of expr * expr  * expr * stmt
  | Forin of access * expr *expr * stmt
  | DoUntil of stmt * expr
  | Switch of expr * stmt list
  | Case of expr * stmt 
  | Default of stmt 
  | MatchItem of expr * stmt list 
  | Pattern of expr * stmt 
  | MatchAll of stmt
  | Break
  | Continue
  | Throw of Exception
  | Try of stmt * stmt list
  | Catch of expr * stmt
 
and Exception = 
  | Exception of string
                                                                   
and stmtordec =                                                    
  | Dec of typ * string              (* Local variable declaration  *)
  | Stmt of stmt                     (* A statement                 *)
  | DeclareAndAssign of typ * string * expr

and topdec = 
  | Fundec of typ option * string * (typ * string) list * stmt
  | Vardec of typ * string
  | Structdec of  string * (typ * string) list 
  | VariableDeclareAndAssign of typ * string * expr

and program = 
  | Prog of topdec list


