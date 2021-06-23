module Contcomp

open System.IO
open System
open Absyn
open Machine

type bstmtordec = 
    | BDec of instr list
    | BStmt of stmt

let rec addINCSP m1 C : instr list = 
    match C with
    | INCSP m2              :: C1   -> addINCSP (m1+m2) C1
    | RET m2                :: C1   -> RET (m2-m1) :: C1
    | Label lab :: RET m2   :: _    -> RET (m2-m1) :: C
    | _                             -> if m1=0 then C else INCSP m1 :: C 

let addLabel C : label * instr list =
    match C with
    | Label lab :: _ -> (lab, C)
    | GOTO lab  :: _ -> (lab, C)
    | _              -> let lab = newLabel()
                        (lab, Label lab :: C)
                        

let makeJump C : instr * instr list = 
    match C with
    | RET m                 :: _ -> (RET m, C)
    | Label lab :: RET m    :: _ -> (RET m, C)
    | Label lab             :: _ -> (GOTO lab, C)
    | GOTO lab              :: _ -> (GOTO lab, C)
    | _                          -> let lab = newLabel()
                                    (GOTO lab, Label lab :: C)
                                   
let makeCall m lab C : instr list = 
    match C with
    | RET n             :: C1 -> TCALL(m, n, lab) :: C1
    | Label _ :: RET n  :: _  -> TCALL(m, n, lab) :: C
    | _                       -> CALL(m, lab) :: C

let rec deadcode C =
    match C with
    | []                -> []
    | Label lab :: _    -> C
    | _         :: C1   -> deadcode C1

let addNOT C = 
    match C with
    | NOT        :: C1 -> C1
    | IFZERO lab :: C1 -> IFNZRO lab :: C1
    | IFNZRO lab :: C1 -> IFZERO lab :: C1
    | _                -> NOT :: C

let addJump jump C =
    let C1 = deadcode C
    match (jump, C1) with
    | (GOTO lab1, Label lab2 :: _)  -> if lab1=lab2 then C1
                                       else GOTO lab1 :: C1
    | _                             -> jump :: C1


let addGOTO lab C =
    addJump (GOTO lab) C

let rec addCST i C = 
    match (i, C) with
    | (0, ADD       :: C1) -> C1
    | (0, SUB       :: C1) -> C1
    | (0, NOT       :: C1) -> addCST 1 C1
    | (_, NOT       :: C1) -> addCST 1 C1
    | (1, MUL       :: C1) -> C1
    | (1, DIV       :: C1) -> C1
    | (0, EQ        :: C1) -> addNOT C1
    | (_, INCSP m   :: C1) -> if m < 0 then addINCSP (m+1) C1
                              else CSTI i :: C 
    | (0, IFZERO lab :: C1) -> addGOTO lab C1
    | (_, IFZERO lab :: C1) -> C1
    | (0, IFNZRO lab :: C1) -> C1
    | (_, IFNZRO lab :: C1) -> addGOTO lab C1
    | _                     -> CSTI i :: C

let rec addCSTF i C =
    match (i, C) with
    | _                     -> (CSTF (System.BitConverter.ToInt32(System.BitConverter.GetBytes(float32(i)), 0))) :: C

let rec addCSTC i C =
    match (i, C) with
    | _                     -> (CSTC ((int32)(System.BitConverter.ToInt16(System.BitConverter.GetBytes(char(i)), 0)))) :: C

type 'data Env = (string * 'data) list

let rec lookup env x = 
    match env with
    | []            -> failwith(x + " not found")
    | (y, v)::yr    -> if x=y then v else lookup yr x

let rec structLookup env x =
    match env with
    | []                            -> failwith(x + " not found")
    | (name, arglist, size)::rhs    -> if x = name then (name, arglist, size) else structLookup rhs x


type Var = 
    | Glovar of int
    | Locvar of int
    | StructMemberLoc of int

let rec structLookupVar env x lastloc =
    match env with
    | []                            -> failwith(x + " not found")
    | (name, (loc, typ))::rhs         -> 
        if x = name then 
            match typ with
            | TypA (_, _)  -> StructMemberLoc (lastloc+1)
            | _                 -> loc 
        else
        match loc with
        | StructMemberLoc lastloc1 -> structLookupVar rhs x lastloc1




type VarEnv = (Var * typ) Env * int

type StructTypeEnv = (string * (Var * typ) Env * int) list 

type Paramdecs = (typ * string) list

type FunEnv = (label * typ option * Paramdecs) Env

type LabEnv = label list


let allocate (kind : int -> Var) (typ, x) (varEnv : VarEnv) (structEnv : StructTypeEnv): VarEnv *  instr list =
    let (env, fdepth) = varEnv
    match typ with
    | TypA (TypA _, _)    -> failwith "Warning: allocate-arrays of arrays not permitted" 
    | TypA (t, Some i)         ->
        let newEnv = ((x, (kind (fdepth+i), typ)) :: env, fdepth+i+1)
        let code = [INCSP i; GETSP; CSTI (i-1); SUB]
        (newEnv, code)
    | TypeStruct structName     ->
        let (name, argslist, size) = structLookup structEnv structName
        // let rec traverse args envir = 
        //     match args with
        //     | []        ->      envir
        //     | (varName, (_, structTyp)):: rhs   ->  
        //         match structTyp with
        //         | TypA (TypA _, _)    -> failwith "Warning: allocate-arrays of arrays not permitted" 
        //         | TypA (t, Some i)         ->
        //             let newEnv = (x, (kind (fdepth+i), structTyp)) :: envir
        //             let newEnv1 = traverse rhs newEnv
        //             newEnv1
        //         | _     ->
        //             let newEnv = (x, (kind (fdepth), structTyp)) :: envir
        //             let newEnv1 = traverse rhs newEnv
        //             newEnv1

        let code = [INCSP (size + 1); GETSP; CSTI (size); SUB]
        // let newEnvr = traverse argslist env
        let newEnvr = ((x, (kind (fdepth + size + 1), typ)) :: env, fdepth+size+1+1)
        (newEnvr, code)
    | _     ->
        let newEnv = ((x, (kind (fdepth), typ)) :: env, fdepth+1)
        let code = [INCSP 1]
        (newEnv, code)

let bindParam (env, fdepth) (typ, x) : VarEnv =
    ((x, (Locvar fdepth, typ)) :: env, fdepth+1);

let bindParams paras (env, fdepth) : VarEnv = 
    List.fold bindParam (env, fdepth) paras;


let rec headlab labs = 
    match labs with
        | lab :: tr -> lab
        | []        -> failwith "Error: unknown break"
let rec dellab labs =
    match labs with
        | lab :: tr ->   tr
        | []        ->   []

let rec cStmt stmt (varEnv : VarEnv) (funEnv : FunEnv) (lablist : LabEnv) (structEnv : StructTypeEnv) (C : instr list) : instr list = 
    match stmt with
    | If(e, stmt1, stmt2) ->
        let (jumpend, C1) = makeJump C
        let (labelse, C2) = addLabel (cStmt stmt2 varEnv funEnv lablist structEnv C1)
        cExpr e varEnv funEnv lablist structEnv (IFZERO labelse :: cStmt stmt1 varEnv funEnv lablist structEnv (addJump jumpend C2))
    | While(e, body) ->
        let labbegin = newLabel()
        let (labend,Cend)   = addLabel C
        let lablist = labend :: labbegin :: lablist
        let (jumptest, C1) = 
            makeJump (cExpr e varEnv funEnv lablist structEnv (IFNZRO labbegin :: Cend))
        addJump jumptest (Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1)
    | DoUntil(body,e) ->
        let labbegin = newLabel()
        let C1 = 
            cExpr e varEnv funEnv lablist structEnv (IFZERO labbegin :: C)
        Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1
    | MatchItem(e,cases) ->  
        let (labend, C1) = addLabel C
        let lablist = labend :: lablist
        let rec everycase c  = 
            match c with
            | Pattern(cond,body) :: tr->
                let (labnextbody,labnext,C2) = everycase tr
                let (label, C3) = addLabel(cStmt body varEnv funEnv lablist structEnv (addGOTO labnextbody C2))
                let (label2, C4) = addLabel( cExpr (Prim2 ("==",e,cond)) varEnv funEnv lablist structEnv (IFZERO labnext :: C3))
                (label,label2,C4)
            | MatchAll( body ) :: tr -> 
                let (labnextbody,labnext,C2) = everycase tr
                let (label, C3) = addLabel(cStmt body varEnv funEnv lablist structEnv (addGOTO labnextbody C2))
                let (label2, C4) = addLabel( cExpr (Prim2 ("==",e,e)) varEnv funEnv lablist structEnv (IFZERO labnext :: C3))
                (label,label2,C4)
            | [] -> (labend, labend,C1)
        let (label,label2,C2) = everycase cases
        C2
    | Switch(e,cases)   ->
        let (labend, C1) = addLabel C
        let lablist = labend :: lablist
        let rec everycase c  = 
            match c with
            | Case(cond,body) :: tr ->
                let (labnextbody,labnext,C2) = everycase tr
                let (label, C3) = addLabel(cStmt body varEnv funEnv lablist structEnv (addGOTO labnextbody C2))
                let (label2, C4) = addLabel( cExpr (Prim2 ("==",e,cond)) varEnv funEnv lablist structEnv (IFZERO labnext :: C3))
                (label,label2,C4)
            | Default( body ) :: tr -> 
                let (labnextbody,labnext,C2) = everycase tr
                let (label, C3) = addLabel(cStmt body varEnv funEnv lablist structEnv (addGOTO labnextbody C2))
                let (label2, C4) = addLabel(cExpr (Prim2 ("==",e,e)) varEnv funEnv lablist structEnv (IFZERO labnext :: C3))
                (label,label2,C4)
            | [] -> (labend, labend,C1)
        let (label,label2,C2) = everycase cases
        C2
    | Case(cond,body)  ->
        C
    | Pattern(cond,body) ->
        C
    | MatchAll( body ) ->
        C
    | DoWhile(body, e) ->
        let labbegin = newLabel()
        let C1 = 
            cExpr e varEnv funEnv lablist structEnv (IFNZRO labbegin :: C)
        Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1 //先执行body
    | For(dec, e, opera,body) ->
        let labend   = newLabel()                       //结束label
        let labbegin = newLabel()                       //设置label 
        let labope   = newLabel()                       //设置 for(,,opera) 的label
        let lablist = labend :: labope :: lablist
        let Cend = Label labend :: C
        let (jumptest, C2) =                                                
            makeJump (cExpr e varEnv funEnv lablist structEnv (IFNZRO labbegin :: Cend)) 
        let C3 = Label labope :: cExpr opera varEnv funEnv lablist structEnv (addINCSP -1 C2)
        let C4 = cStmt body varEnv funEnv lablist structEnv C3    
        cExpr dec varEnv funEnv lablist structEnv (addINCSP -1 (addJump jumptest  (Label labbegin :: C4) ) ) //dec Label: body  opera  testjumpToBegin 指令的顺序  
    | Expr e ->
        cExpr e varEnv funEnv lablist structEnv (addINCSP -1 C)
    | Block stmts ->
        let rec pass1 stmts ((_, fdepth) as varEnv) = 
            match stmts with
            | []        -> ([], fdepth)
            | s1::sr    ->
                let (_, varEnv1) as res1 = bStmtordec s1 varEnv structEnv
                let (resr, fdepthr) = pass1 sr varEnv1
                (res1 :: resr, fdepthr)
        let (stmtsback, fdepthend) = pass1 stmts varEnv
        let rec pass2 pairs C =
            match pairs with
            | [] -> C            
            | (BDec code, varEnv)  :: sr -> code @ pass2 sr C
            | (BStmt stmt, varEnv) :: sr -> cStmt stmt varEnv funEnv lablist structEnv (pass2 sr C)
        pass2 stmtsback (addINCSP(snd varEnv - fdepthend) C)
    | Return None ->
        RET (snd varEnv - 1) :: deadcode C
    | Return (Some e) ->
        cExpr e varEnv funEnv lablist structEnv (RET (snd varEnv) :: deadcode C)
    | Break ->
        let labend = headlab lablist
        addGOTO labend C
    | Continue ->
        let lablist   = dellab lablist
        let labbegin = headlab lablist
        addGOTO labbegin C
and tryStmt tryBlock (varEnv : VarEnv) (funEnv : FunEnv) (lablist : LabEnv) (structEnv : StructTypeEnv) (C : instr list) : instr list * VarEnv = 
    match tryBlock with
    | Block stmts ->
        let rec pass1 stmts ((_, fdepth) as varEnv) = 
            match stmts with
            | []        -> ([], fdepth,varEnv)
            | s1::sr    ->
                let (_, varEnv1) as res1 = bStmtordec s1 varEnv structEnv
                let (resr, fdepthr,varEnv2) = pass1 sr varEnv1
                (res1 :: resr, fdepthr,varEnv2)
        let (stmtsback, fdepthend,varEnv1) = pass1 stmts varEnv
        let rec pass2 pairs C =
            match pairs with
            | [] -> C            
            | (BDec code, varEnv)  :: sr -> code @ pass2 sr C
            | (BStmt stmt, varEnv) :: sr -> cStmt stmt varEnv funEnv lablist structEnv (pass2 sr C)
        (pass2 stmtsback (addINCSP(snd varEnv - fdepthend) C),varEnv1)
and bStmtordec stmtOrDec varEnv (structEnv : StructTypeEnv): bstmtordec * VarEnv =

    match stmtOrDec with
    | Stmt stmt    ->
        (BStmt stmt, varEnv)
    | Dec (typ, x)  ->
        let (varEnv1, code) = allocate Locvar (typ, x) varEnv structEnv
        (BDec code, varEnv1)
    | DeclareAndAssign (typ, x, e) ->
        let (varEnv1, code) = allocate Locvar (typ, x) varEnv structEnv
        (BDec (cAccess (AccVar(x)) varEnv1 [] [] structEnv (cExpr e varEnv1 [] [] structEnv (STI :: (addINCSP -1 code)))), varEnv1)

and cExpr (e : expr) (varEnv : VarEnv) (funEnv : FunEnv) (lablist : LabEnv) (structEnv : StructTypeEnv) (C : instr list) : instr list =
    match e with
    | Access acc        -> cAccess acc varEnv funEnv lablist structEnv (LDI :: C)
    | Assign(acc, e)    -> cAccess acc varEnv funEnv lablist structEnv (cExpr e varEnv funEnv lablist structEnv (STI :: C))
    | ToInt e           -> match e with
                           |  ConstFloat f -> addCST (int f) C
                           |  ConstChar c -> addCST (int c - int '0') C
    | ToChar e          -> match e with
                           |  CstI i -> addCSTC (i + int '0') C                        
    | ToFloat e         -> match e with
                           |  CstI i -> addCSTF (float32 i) C                     
    | ConstFloat i      -> addCSTF i C
    | ConstChar i       -> addCSTC (int i) C
    | CstI i            -> addCST i C
    | Access acc        -> cAccess acc varEnv funEnv lablist structEnv C
    | CreateI(s,hex)    -> let mutable res = 0;
                           for i=0 to s.Length-1 do
                                if s.Chars(i)>='0' && s.Chars(i)<='9' then
                                    res <- res*hex + ( (int (s.Chars(i)))-(int '0') )
                                elif s.Chars(i)>='a' && s.Chars(i)<='f' then
                                    res <- res*hex + ( (int (s.Chars(i)))-(int 'a')+10 )
                                elif s.Chars(i)>='A' && s.Chars(i)<='F' then
                                    res <- res*hex + ( (int (s.Chars(i)))-(int 'A')+10 )
                                else 
                                    failwith("ERROR WORLD IN NUMBER")
                           addCST res C     
    | Self(acc,ope,e)->             
        cExpr e varEnv funEnv lablist structEnv
            (match ope with
            | "+" -> 
                let ass = Assign (acc,Prim2("+",Access acc, e))
                cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
            | "-" ->
                let ass = Assign (acc,Prim2("-",Access acc, e))
                cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
            | "+B" -> 
                let ass = Assign (acc,Prim2("+",Access acc, e))
                let C1 = cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
                CSTI 1 :: ADD :: (addINCSP -1 C1)
            | "-B" ->
                let ass = Assign (acc,Prim2("-",Access acc, e))
                let C1 = cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
                CSTI 1 :: SUB :: (addINCSP -1 C1)
            | "*" -> 
                let ass = Assign (acc,Prim2("*",Access acc, e))
                cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
            | "/" ->
                let ass = Assign (acc,Prim2("/",Access acc, e))
                cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
            | "%" ->
                let ass = Assign (acc,Prim2("%",Access acc, e))
                cExpr ass varEnv funEnv lablist structEnv (addINCSP -1 C)
            | _         -> failwith "Error: unknown unary operator")
    | Print(ope,e1)  ->
         cExpr e1 varEnv funEnv lablist structEnv
            (match ope with
            | "%d"  -> PRINTI :: C
            | "%c"  -> PRINTC :: C
            | "%f"  -> PRINTF :: C
            )
    | PrintHex(hex,e1)  -> failwith("Error")
    | Prim1(ope, e1) ->
        let rec tmp stat =
                    match stat with
                    | Access (c) -> c               //get IAccess
        cExpr e1 varEnv funEnv lablist structEnv
            (match ope with
            | "!"       -> addNOT C
            | _         -> failwith "Error: unknown unary operator")
    | Prim2(ope, e1, e2)    ->
        cExpr e1 varEnv funEnv lablist structEnv
            (cExpr e2 varEnv funEnv lablist structEnv
                (match ope with
                | "*"   -> MUL  ::  C
                | "+"   -> ADD  ::  C
                | "-"   -> SUB  ::  C
                | "/"   -> 
                    let head C1 =
                        match C1 with
                        | a :: tr -> a
                        | []-> failwith "Error: empty ins"
                    if head C = CSTI 0 then THROW 1 :: (addINCSP -1 C)
                    else   DIV  ::  C
                | "%"   -> MOD  ::  C
                | "=="  -> EQ   ::  C
                | "!="  -> EQ   ::  addNOT C
                | "<"   -> LT   ::  C
                | ">="  -> LT   ::  addNOT C
                | ">"   -> SWAP ::  LT  :: C
                | "<="  -> SWAP ::  LT  :: addNOT C
                | _     -> failwith "Error: unknown binary operator"))
    | Prim3(cond, e1, e2)    ->
        let (jumpend, C1) = makeJump C
        let (labelse, C2) = addLabel (cExpr e2 varEnv funEnv lablist structEnv C1)
        cExpr cond varEnv funEnv lablist structEnv (IFZERO labelse :: cExpr e1 varEnv funEnv lablist structEnv (addJump jumpend C2))
    | Andalso(e1, e2)   ->
        match C with
        | IFZERO lab :: _ ->
            cExpr e1 varEnv funEnv lablist structEnv (IFZERO lab :: cExpr e2 varEnv funEnv lablist structEnv C)
        | IFNZRO labthen :: C1 ->
            let (labelse, C2) = addLabel C1
            cExpr e1 varEnv funEnv lablist structEnv
                (IFZERO labelse 
                    :: cExpr e2 varEnv funEnv lablist structEnv (IFNZRO labthen :: C2))
        | _ ->
            let (jumpend, C1)   = makeJump C
            let (labfalse, C2)  = addLabel (addCST 0 C1)
            cExpr e1 varEnv funEnv lablist structEnv
                (IFZERO labfalse
                    :: cExpr e2 varEnv funEnv lablist structEnv (addJump jumpend C2))
    | Orelse(e1, e2)    ->
        match C with
        | IFNZRO lab :: _ ->
            cExpr e1 varEnv funEnv lablist structEnv (IFNZRO lab :: cExpr e2 varEnv funEnv lablist structEnv C)
        | IFZERO labthen :: C1 ->
            let(labelse, C2) = addLabel C1
            cExpr e1 varEnv funEnv lablist structEnv
                (IFNZRO labelse :: cExpr e2 varEnv funEnv lablist structEnv
                    (IFZERO labthen :: C2))
        | _ ->
            let (jumpend, C1) = makeJump C
            let (labtrue, C2) = addLabel(addCST 1 C1)
            cExpr e1 varEnv funEnv lablist structEnv
                (IFNZRO labtrue
                    :: cExpr e2 varEnv funEnv lablist structEnv (addJump jumpend C2))
    | Call(f, es)   -> callfun f es varEnv funEnv lablist (structEnv : StructTypeEnv) C

and structAllocateDef(kind : int -> Var) (structName : string) (typ : typ) (varName : string) (structTypEnv : StructTypeEnv) : StructTypeEnv = 
    match structTypEnv with
    | lhs :: rhs ->
        let (name, env, depth) = lhs
        if name = structName 
        then 
            match typ with
            | TypA (TypA _, _)    -> failwith "Warning: allocate-arrays of arrays not permitted" 
            | TypA (t, Some i)         ->
                let newEnv = env @ [(varName, (kind (depth+i), typ))]
                (name, newEnv, depth + i) :: rhs
            | _ ->
                let newEnv = env @ [(varName, (kind (depth+1), typ))] 
                (name, newEnv, depth + 1) :: rhs
        else structAllocateDef kind structName typ varName rhs
    | [] -> 
        match typ with
            | TypA (TypA _, _)    -> failwith "Warning: allocate-arrays of arrays not permitted" 
            | TypA (t, Some i)         ->
                let newEnv = [(varName, (kind (i), typ))]
                (structName, newEnv, i) :: structTypEnv
            | _ ->
                let newEnv = [(varName, (kind (0), typ))]
                (structName, newEnv, 0) :: structTypEnv



and makeStructEnvs(structName : string) (structEntry :(typ * string) list ) (structTypEnv : StructTypeEnv) : StructTypeEnv = 
    let rec addm structName structEntry structTypEnv = 
        match structEntry with
        | [] -> structTypEnv
        | lhs::rhs ->
            match lhs with
            | (typ, name)   -> 
                let structTypEnv1 = structAllocateDef StructMemberLoc structName typ name structTypEnv
                let structTypEnvr = addm structName rhs structTypEnv1
                structTypEnvr

    addm structName structEntry structTypEnv
    
and makeGlobalEnvs(topdecs : topdec list) : VarEnv * FunEnv * StructTypeEnv * instr list =
    let rec addv decs varEnv funEnv structTypEnv =
        match decs with
        | [] -> (varEnv, funEnv, structTypEnv, [])
        | dec::decr ->
            match dec with
            | Vardec (typ, x) -> 
                let (varEnv1, code1) = allocate Glovar (typ, x) varEnv structTypEnv
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv1 funEnv structTypEnv
                (varEnvr, funEnvr, structTypEnvr, code1 @ coder)
            | VariableDeclareAndAssign (typ, x, e) -> 
                let (varEnv1, code1) = allocate Glovar (typ, x) varEnv structTypEnv
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv1 funEnv structTypEnv
                (varEnvr, funEnvr, structTypEnvr, code1 @ (cAccess (AccVar(x)) varEnvr funEnvr [] structTypEnv (cExpr e varEnvr funEnvr [] structTypEnv (STI :: (addINCSP -1 coder)))))
            | Fundec (tyOpt, f, xs, body) ->
                addv decr varEnv ((f, (newLabel(), tyOpt, xs)) :: funEnv) structTypEnv
            |Structdec (typName, typEntry) -> 
                let structTypEnv1 = makeStructEnvs typName typEntry structTypEnv
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv funEnv structTypEnv1
                (varEnvr, funEnvr, structTypEnvr, coder)
                
    addv topdecs ([], 0) [] []


and cAccess access varEnv funEnv lablist (structEnv : StructTypeEnv) C =
    match access with
    | AccVar x  ->
        match lookup (fst varEnv) x with
        | Glovar addr, _ -> addCST addr C
        | Locvar addr, _ -> GETBP :: addCST addr (ADD :: C)
    | AccStruct (AccVar stru, AccVar memb) ->
        let (loc, TypeStruct structname)   = lookup (fst varEnv) stru
        let (name, argslist, size) = structLookup structEnv structname
        match structLookupVar argslist memb 0 with
        | StructMemberLoc varLocate ->
            match lookup (fst varEnv) stru with
            | Glovar addr, _ -> addCST (addr - (size+1) + varLocate) C
            | Locvar addr, _ -> GETBP :: addCST (addr - (size+1) + varLocate) (ADD ::  C)
    | AccDeref e ->
        cExpr e varEnv funEnv lablist structEnv C
    | AccIndex(acc, idx)    ->
        match acc with
        |  AccStruct (AccVar stru, AccVar memb) ->
            cAccess acc varEnv funEnv lablist structEnv (cExpr idx varEnv funEnv lablist structEnv (ADD :: C))
        | _     ->   
            cAccess acc varEnv funEnv lablist structEnv (LDI :: cExpr idx varEnv funEnv lablist structEnv (ADD :: C))


and cExprs es varEnv funEnv lablist (structEnv : StructTypeEnv) C = 
    match es with
    | []        -> C
    | e1::er    -> cExpr e1 varEnv funEnv lablist structEnv (cExprs er varEnv funEnv lablist structEnv C)


and callfun f es varEnv funEnv lablist (structEnv : StructTypeEnv) C : instr list =
    let (labf, tyOpt, paramdecs) = lookup funEnv f
    let argc = List.length es
    if argc = List.length paramdecs then
        cExprs es varEnv funEnv lablist structEnv (makeCall argc labf C)
    else
        failwith (f + ": parameter/argument mismatch")


let cProgram (Prog topdecs) : instr list = 
    let _ = resetLabels ()
    let ((globalVarEnv, _), funEnv, structEnv, globalInit) = makeGlobalEnvs topdecs
    let compilefun (tyOpt, f, xs, body) = 
        let (labf, _, paras)    = lookup funEnv f
        let (envf, fdepthf)     = bindParams paras (globalVarEnv, 0)
        let C0                  = [RET (List.length paras-1)]
        let code                = cStmt body (envf, fdepthf) funEnv [] structEnv C0
        Label labf :: code
    let functions = 
        List.choose (function 
                        | Fundec (rTy, name, argTy, body)
                                            ->  Some (compilefun (rTy, name, argTy, body))
                        | Vardec _ -> None
                        | VariableDeclareAndAssign _ -> None
                        | Structdec _ -> None)
                        topdecs
    let (mainlab, _, mainparams) = lookup funEnv "main"
    let argc = List.length mainparams
    globalInit
    @ [LDARGS; CALL(argc, mainlab); STOP]
    @ List.concat functions



let intsToFile (inss : int list) (fname : string) =
    File.WriteAllText(fname, String.concat " " (List.map string inss))


let contCompileToFile program fname =
    let instrs      = cProgram program
    let bytecode    = code2ints instrs
    intsToFile bytecode fname; instrs
