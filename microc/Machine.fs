module Machine

type label = string

type instr =
    | Label of label
    | CSTI of int32
    | CSTF of int32
    | CSTC of int32
    | CSTS of int32 list
    | ADD
    | SUB
    | MUL
    | DIV
    | MOD
    | EQ
    | LT
    | NOT
    | DUP
    | SWAP
    | LDI
    | STI
    | GETBP
    | GETSP
    | INCSP of int
    | GOTO of label
    | IFZERO of label
    | IFNZRO of label
    | CALL of int * label
    | TCALL of int * int * label
    | RET of int
    | PRINTI
    | PRINTC
    | LDARGS
    | STOP
    | THROW of int
    | PUSHHDLR of int * label
    | POPHDLR
    | PRINTF

let (resetLabels, newLabel) =
    let lastlab = ref -1
    ((fun () -> lastlab := 0), (fun () -> (lastlab := 1 + !lastlab; "L" + (!lastlab).ToString())))

type 'data env = (string * 'data) list

let rec lookup env x = 
    match env with
    | []            -> failwith(x + "not found")
    | (y, v)::yr    -> if x=y then v else lookup yr x

[<Literal>]
let CODECSTI    = 0

[<Literal>]
let CODEADD     = 1

[<Literal>]
let CODESUB     = 2

[<Literal>]
let CODEMUL     = 3

[<Literal>]
let CODEDIV     = 4

[<Literal>]
let CODEMOD     = 5

[<Literal>]
let CODEEQ      = 6

[<Literal>]
let CODELT      = 7

[<Literal>]
let CODENOT     = 8

[<Literal>]
let CODEDUP     = 9

[<Literal>]
let CODESWAP    = 10

[<Literal>]
let CODELDI     = 11

[<Literal>]
let CODESTI     = 12

[<Literal>]
let CODEGETBP   = 13

[<Literal>]
let CODEGETSP   = 14

[<Literal>]
let CODEINCSP   = 15

[<Literal>]
let CODEGOTO    = 16

[<Literal>]
let CODEIFZERO  = 17

[<Literal>]
let CODEIFNZRO  = 18

[<Literal>]
let CODECALL    = 19

[<Literal>]
let CODETCALL   = 20

[<Literal>]
let CODERET     = 21

[<Literal>]
let CODEPRINTI  = 22

[<Literal>]
let CODEPRINTC  = 23

[<Literal>]
let CODELDARGS  = 24

[<Literal>]
let CODESTOP    = 25;


[<Literal>]
let CODECSTF    = 26;

[<Literal>]
let CODECSTC    = 27;

[<Literal>]
let CODETHROW   = 28;

[<Literal>]
let CODEPUSHHR  = 29;


[<Literal>]
let CODEPOPHR   = 30;

[<Literal>]
let CODEPRINTF  = 31

[<Literal>]
let CODECSTS    = 32;

let makelabenv (addr, labenv) instr = 
    match instr with
    | Label lab         -> (addr, (lab, addr) :: labenv)
    | CSTI i            -> (addr+2, labenv)
    | CSTS str          -> (addr+1+str.Length, labenv)
    | CSTF i            -> (addr+2, labenv)
    | CSTC i            -> (addr+2, labenv)
    | ADD               -> (addr+1, labenv)
    | SUB               -> (addr+1, labenv)
    | MUL               -> (addr+1, labenv)
    | DIV               -> (addr+1, labenv)
    | MOD               -> (addr+1, labenv)
    | EQ                -> (addr+1, labenv)
    | LT                -> (addr+1, labenv)
    | NOT               -> (addr+1, labenv)
    | DUP               -> (addr+1, labenv)
    | SWAP              -> (addr+1, labenv)
    | LDI               -> (addr+1, labenv)
    | STI               -> (addr+1, labenv)
    | GETBP             -> (addr+1, labenv)
    | GETSP             -> (addr+1, labenv)
    | INCSP m           -> (addr+2, labenv)
    | GOTO lab          -> (addr+2, labenv)
    | IFZERO lab        -> (addr+2, labenv)
    | IFNZRO lab        -> (addr+2, labenv)
    | CALL(m, lab)      -> (addr+3, labenv)
    | TCALL(m, n, lab)  -> (addr+4, labenv)
    | RET m             -> (addr+2, labenv)
    | PRINTI            -> (addr+1, labenv)
    | PRINTC            -> (addr+1, labenv)
    | PRINTF            -> (addr+1, labenv)
    | LDARGS            -> (addr+1, labenv)
    | STOP              -> (addr+1, labenv)
    | THROW i           -> (addr+2, labenv)
    | PUSHHDLR (exn ,lab) -> (addr+3, labenv)
    | POPHDLR           -> (addr+1, labenv)


let rec emitints getlab instr ints = 
    match instr with
    | Label lab         -> ints
    | CSTI i            -> CODECSTI     :: i            :: ints
    | CSTF i            -> CODECSTF     :: i            :: ints
    | CSTC i            -> CODECSTC     :: i            :: ints
    | CSTS str          -> CODECSTS     :: str         @ ints
    | ADD               -> CODEADD      :: ints
    | SUB               -> CODESUB      :: ints
    | MUL               -> CODEMUL      :: ints
    | DIV               -> CODEDIV      :: ints
    | MOD               -> CODEMOD      :: ints
    | EQ                -> CODEEQ       :: ints
    | LT                -> CODELT       :: ints
    | NOT               -> CODENOT      :: ints
    | DUP               -> CODEDUP      :: ints
    | SWAP              -> CODESWAP     :: ints
    | LDI               -> CODELDI      :: ints
    | STI               -> CODESTI      :: ints
    | GETBP             -> CODEGETBP    :: ints
    | GETSP             -> CODEGETSP    :: ints
    | INCSP m           -> CODEINCSP    :: m            :: ints
    | GOTO lab          -> CODEGOTO     :: getlab lab   :: ints
    | IFZERO lab        -> CODEIFZERO   :: getlab lab   :: ints
    | IFNZRO lab        -> CODEIFNZRO   :: getlab lab   :: ints
    | CALL(m, lab)      -> CODECALL     :: m            :: getlab lab   :: ints
    | TCALL(m, n, lab)  -> CODETCALL    :: m            :: n            :: getlab lab   :: ints
    | RET m             -> CODERET      :: m            :: ints
    | PRINTI            -> CODEPRINTI   :: ints
    | PRINTC            -> CODEPRINTC   :: ints
    | PRINTF            -> CODEPRINTF   :: ints
    | LDARGS            -> CODELDARGS   :: ints
    | STOP              -> CODESTOP     :: ints
    | THROW i           -> CODETHROW    :: i            :: ints
    | PUSHHDLR (exn, lab) -> CODEPUSHHR :: exn          :: getlab lab   :: ints
    | POPHDLR           -> CODEPOPHR    :: ints

let code2ints (code : instr list) : int list = 
    let (_, labenv) = List.fold makelabenv (0, []) code
    let getlab lab = lookup labenv lab
    List.foldBack (emitints getlab) code []


let ntolabel (n:int) :label =
    string(n)


let rec decomp ints : instr list = 

    // printf "%A" ints

    match ints with
    | []                                            ->   []
    | CODEADD :: ints_rest                          ->   ADD            :: decomp ints_rest
    | CODESUB    :: ints_rest                       ->   SUB            :: decomp ints_rest
    | CODEMUL    :: ints_rest                       ->   MUL            :: decomp ints_rest
    | CODEDIV    :: ints_rest                       ->   DIV            :: decomp ints_rest
    | CODEMOD    :: ints_rest                       ->   MOD            :: decomp ints_rest
    | CODEEQ     :: ints_rest                       ->   EQ             :: decomp ints_rest
    | CODELT     :: ints_rest                       ->   LT             :: decomp ints_rest
    | CODENOT    :: ints_rest                       ->   NOT            :: decomp ints_rest
    | CODEDUP    :: ints_rest                       ->   DUP            :: decomp ints_rest
    | CODESWAP   :: ints_rest                       ->   SWAP           :: decomp ints_rest
    | CODELDI    :: ints_rest                       ->   LDI            :: decomp ints_rest
    | CODESTI    :: ints_rest                       ->   STI            :: decomp ints_rest
    | CODEGETBP  :: ints_rest                       ->   GETBP          :: decomp ints_rest
    | CODEGETSP  :: ints_rest                       ->   GETSP          :: decomp ints_rest
    | CODEINCSP  :: m :: ints_rest                  ->   INCSP m        :: decomp ints_rest
    | CODEGOTO   :: lab :: ints_rest                ->   GOTO (ntolabel lab)        :: decomp ints_rest
    | CODEIFZERO :: lab :: ints_rest                ->   IFZERO (ntolabel lab)      :: decomp ints_rest
    | CODEIFNZRO :: lab :: ints_rest                ->   IFNZRO (ntolabel lab)      :: decomp ints_rest
    | CODECALL   :: m :: lab :: ints_rest           ->   CALL(m, ntolabel lab)      :: decomp ints_rest
    | CODETCALL  :: m :: n ::  lab :: ints_rest     ->   TCALL(m,n,ntolabel lab)    :: decomp ints_rest
    | CODERET    :: m :: ints_rest                  ->   RET m          :: decomp ints_rest
    | CODEPRINTI :: ints_rest                       ->   PRINTI         :: decomp ints_rest
    | CODEPRINTC :: ints_rest                       ->   PRINTC         :: decomp ints_rest
    | CODEPRINTF :: ints_rest                       ->   PRINTF         :: decomp ints_rest
    | CODELDARGS :: ints_rest                       ->   LDARGS         :: decomp ints_rest
    | CODESTOP   :: ints_rest                       ->   STOP           :: decomp ints_rest
    | CODECSTI   :: i :: ints_rest                  ->   CSTI i         :: decomp ints_rest       
    | CODECSTF   :: i :: ints_rest                  ->   CSTF i         :: decomp ints_rest    
    | CODECSTC   :: i :: ints_rest                  ->   CSTC i         :: decomp ints_rest      
    | _                                             ->    printf "%A" ints; failwith "unknow code"

