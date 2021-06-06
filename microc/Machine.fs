(* File MicroC/Machine.fs 

   Instructions and code emission for a stack-based
   abstract machine * sestoft@itu.dk 2009-09-23

   Implementations of the machine are found in file MicroC/Machine.java 
   and MicroC/machine.c.
   
   Must precede Comp.fs and Contcomp.fs in the VS Solution Explorer.
 *)

module Machine

type label = string
// 汇编指令
type instr =
  | Label of label                     (* symbolic label; pseudo-instruc. *)
  | CSTI of int                        (* constant                        *)
  | ADD                                (* addition                        *)
  | SUB                                (* subtraction                     *)
  | MUL                                (* multiplication                  *)
  | DIV                                (* division                        *)
  | MOD                                (* modulus                         *)
  | EQ                                 (* equality: s[sp-1] == s[sp]      *)
  | LT                                 (* less than: s[sp-1] < s[sp]      *)
  | NOT                                (* logical negation:  s[sp] != 0   *)
  | DUP                                (* duplicate stack top             *)
  | SWAP                               (* swap s[sp-1] and s[sp]          *)
  | LDI                                (* get s[s[sp]]                    *)
  | STI                                (* set s[s[sp-1]]                  *)
  | GETBP                              (* get bp                          *)
  | GETSP                              (* get sp                          *)
  | INCSP of int                       (* increase stack top by m         *)
  | GOTO of label                      (* go to label                     *)
  | IFZERO of label                    (* go to label if s[sp] == 0       *)
  | IFNZRO of label                    (* go to label if s[sp] != 0       *)
  | CALL of int * label                (* move m args up 1, push pc, jump *)
  | TCALL of int * int * label         (* move m args down n, jump        *)
  | RET of int                         (* pop m and return to s[sp]       *)
  | PRINTI                             (* print s[sp] as integer          *)
  | PRINTC                             (* print s[sp] as character        *)
  | LDARGS                             (* load command line args on stack *)
  | STOP                               (* halt the abstract machine       *)

(* Generate new distinct labels *)

// 返回两个函数 resetLabels , newLabel
let (resetLabels, newLabel) = 
    let lastlab = ref -1
    ((fun () -> lastlab := 0), (fun () -> (lastlab := 1 + !lastlab; "L" + (!lastlab).ToString())))

(* Simple environment operations *)

type 'data env = (string * 'data) list

let rec lookup env x = 
    match env with 
    | []         -> failwith (x + " not found")
    | (y, v)::yr -> if x=y then v else lookup yr x

(* An instruction list is emitted in two phases:
   * pass 1 builds an environment labenv mapping labels to addresses 
   * pass 2 emits the code to file, using the environment labenv to 
     resolve labels
 *)

(* These numeric instruction codes must agree with Machine.java: *)



//机器码

//[<Literal>] 属性可以让 
//该变量在模式匹配时候被匹配,否则匹配时只能用数值.不能用变量名



[<Literal>]
let CODECSTI   = 0 


[<Literal>]
let CODEADD    = 1 


[<Literal>]
let CODESUB    = 2 


[<Literal>]
let CODEMUL    = 3 

[<Literal>]
let CODEDIV    = 4 

[<Literal>]
let CODEMOD    = 5 

[<Literal>]
let CODEEQ     = 6 

[<Literal>]
let CODELT     = 7 

[<Literal>]
let CODENOT    = 8 

[<Literal>]
let CODEDUP    = 9 

[<Literal>]
let CODESWAP   = 10 

[<Literal>]
let CODELDI    = 11 


[<Literal>]
let CODESTI    = 12 


[<Literal>]
let CODEGETBP  = 13 


[<Literal>]
let CODEGETSP  = 14 


[<Literal>]
let CODEINCSP  = 15 


[<Literal>]
let CODEGOTO   = 16


[<Literal>]
let CODEIFZERO = 17

[<Literal>]
let CODEIFNZRO = 18 



[<Literal>]
let CODECALL   = 19


[<Literal>]
let CODETCALL  = 20

[<Literal>]
let CODERET    = 21


[<Literal>]
let CODEPRINTI = 22 

[<Literal>]
let CODEPRINTC = 23


[<Literal>]
let CODELDARGS = 24

[<Literal>]
let CODESTOP   = 25;



(* Bytecode emission, first pass: build environment that maps 
   each label to an integer address in the bytecode.
 *)
//获得标签在机器码中的地址
let makelabenv (addr, labenv) instr = 
    match instr with
    // 记录当前 (标签, 地址) ==> 到 labenv中
    | Label lab      -> (addr, (lab, addr) :: labenv)
    | CSTI i         -> (addr+2, labenv)
    | ADD            -> (addr+1, labenv)
    | SUB            -> (addr+1, labenv)
    | MUL            -> (addr+1, labenv)
    | DIV            -> (addr+1, labenv)
    | MOD            -> (addr+1, labenv)
    | EQ             -> (addr+1, labenv)
    | LT             -> (addr+1, labenv)
    | NOT            -> (addr+1, labenv)
    | DUP            -> (addr+1, labenv)
    | SWAP           -> (addr+1, labenv)
    | LDI            -> (addr+1, labenv)
    | STI            -> (addr+1, labenv)
    | GETBP          -> (addr+1, labenv)
    | GETSP          -> (addr+1, labenv)
    | INCSP m        -> (addr+2, labenv)
    | GOTO lab       -> (addr+2, labenv)
    | IFZERO lab     -> (addr+2, labenv)
    | IFNZRO lab     -> (addr+2, labenv)
    | CALL(m,lab)    -> (addr+3, labenv)
    | TCALL(m,n,lab) -> (addr+4, labenv)
    | RET m          -> (addr+2, labenv)
    | PRINTI         -> (addr+1, labenv)
    | PRINTC         -> (addr+1, labenv)
    | LDARGS         -> (addr+1, labenv)
    | STOP           -> (addr+1, labenv)

(* Bytecode emission, second pass: output bytecode as integers *)

//getlab 是得到标签所在地址的函数
//let getlab lab = lookup labenv lab

let rec emitints getlab instr ints = 
    match instr with
    | Label lab      -> ints
    | CSTI i         -> CODECSTI   :: i :: ints
    | ADD            -> CODEADD    :: ints
    | SUB            -> CODESUB    :: ints
    | MUL            -> CODEMUL    :: ints
    | DIV            -> CODEDIV    :: ints
    | MOD            -> CODEMOD    :: ints
    | EQ             -> CODEEQ     :: ints
    | LT             -> CODELT     :: ints
    | NOT            -> CODENOT    :: ints
    | DUP            -> CODEDUP    :: ints
    | SWAP           -> CODESWAP   :: ints
    | LDI            -> CODELDI    :: ints
    | STI            -> CODESTI    :: ints
    | GETBP          -> CODEGETBP  :: ints
    | GETSP          -> CODEGETSP  :: ints
    | INCSP m        -> CODEINCSP  :: m :: ints
    | GOTO lab       -> CODEGOTO   :: getlab lab :: ints
    | IFZERO lab     -> CODEIFZERO :: getlab lab :: ints
    | IFNZRO lab     -> CODEIFNZRO :: getlab lab :: ints
    | CALL(m,lab)    -> CODECALL   :: m :: getlab lab :: ints
    | TCALL(m,n,lab) -> CODETCALL  :: m :: n :: getlab lab :: ints
    | RET m          -> CODERET    :: m :: ints
    | PRINTI         -> CODEPRINTI :: ints
    | PRINTC         -> CODEPRINTC :: ints
    | LDARGS         -> CODELDARGS :: ints
    | STOP           -> CODESTOP   :: ints


(* Convert instruction list to int list in two passes:
   Pass 1: build label environment
   Pass 2: output instructions using label environment
 *)
 
//通过对 code 的两次遍历,完成汇编指令到机器指令的转换
let code2ints (code : instr list) : int list =
    
    //从前往后遍历 `汇编指令序列 code: instr list`
    //得到 标签对应的地址,记录到 labenv中
    let (_, labenv) = List.fold makelabenv (0, []) code
    
    //getlab 是得到标签所在地址的函数
    let getlab lab = lookup labenv lab
    
    //从后往前 遍历 `汇编指令序列 code: instr list`
    List.foldBack (emitints getlab) code []
                    


let ntolabel (n:int) :label = 
    string(n)

//反编译
let rec decomp ints : instr list = 

    // printf "%A" ints

    match ints with
    | []                                              ->  []
    | CODEADD :: ints_rest                         ->   ADD           :: decomp ints_rest
    | CODESUB    :: ints_rest                         ->   SUB           :: decomp ints_rest
    | CODEMUL    :: ints_rest                         ->   MUL           :: decomp ints_rest
    | CODEDIV    :: ints_rest                         ->   DIV           :: decomp ints_rest
    | CODEMOD    :: ints_rest                         ->   MOD           :: decomp ints_rest
    | CODEEQ     :: ints_rest                         ->   EQ            :: decomp ints_rest
    | CODELT     :: ints_rest                         ->   LT            :: decomp ints_rest
    | CODENOT    :: ints_rest                         ->   NOT           :: decomp ints_rest
    | CODEDUP    :: ints_rest                         ->   DUP           :: decomp ints_rest
    | CODESWAP   :: ints_rest                         ->   SWAP          :: decomp ints_rest
    | CODELDI    :: ints_rest                         ->   LDI           :: decomp ints_rest
    | CODESTI    :: ints_rest                         ->   STI           :: decomp ints_rest
    | CODEGETBP  :: ints_rest                         ->   GETBP         :: decomp ints_rest
    | CODEGETSP  :: ints_rest                         ->   GETSP         :: decomp ints_rest
    | CODEINCSP  :: m :: ints_rest                    ->   INCSP m       :: decomp ints_rest
    | CODEGOTO   ::  lab :: ints_rest           ->   GOTO (ntolabel lab)      :: decomp ints_rest
    | CODEIFZERO ::  lab :: ints_rest           ->   IFZERO (ntolabel lab)    :: decomp ints_rest
    | CODEIFNZRO ::  lab :: ints_rest           ->   IFNZRO (ntolabel lab)    :: decomp ints_rest
    | CODECALL   :: m ::  lab :: ints_rest      ->   CALL(m, ntolabel lab)   :: decomp ints_rest
    | CODETCALL  :: m :: n ::  lab :: ints_rest ->   TCALL(m,n,ntolabel lab):: decomp ints_rest
    | CODERET    :: m :: ints_rest                    ->   RET m         :: decomp ints_rest
    | CODEPRINTI :: ints_rest                         ->   PRINTI        :: decomp ints_rest
    | CODEPRINTC :: ints_rest                         ->   PRINTC        :: decomp ints_rest
    | CODELDARGS :: ints_rest                         ->   LDARGS        :: decomp ints_rest
    | CODESTOP   :: ints_rest                         ->   STOP             :: decomp ints_rest
    | CODECSTI   :: i :: ints_rest                    ->   CSTI i :: decomp ints_rest       
    | _                                       ->    printf "%A" ints; failwith "unknow code"

