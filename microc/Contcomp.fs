module Contcomp

open System.IO
open System
open Absyn
open Machine

type bstmtordec = 
    | BDec of instr list  //局部变量的声明
    | BStmt of stmt    //一个声明

//执行局部优化的代码生成函数
//INCSP m 的意思是increase stack top by m
//RET m 的意思是pop m and return to s[sp]
//Label lab 是符号标签；伪指令。
//大致的功能就是实现栈的空间操作
let rec addINCSP m1 C : instr list = 
    match C with
    | INCSP m2              :: C1   -> addINCSP (m1+m2) C1
    | RET m2                :: C1   -> RET (m2-m1) :: C1
    | Label lab :: RET m2   :: _    -> RET (m2-m1) :: C
    | _                             -> if m1=0 then C else INCSP m1 :: C 

//条件跳转到C 
let addLabel C : label * instr list =
    match C with
    | Label lab :: _ -> (lab, C)
    | GOTO lab  :: _ -> (lab, C)
    | _              -> let lab = newLabel()
                        (lab, Label lab :: C)
                        
//无条件转跳C 
let makeJump C : instr * instr list = 
    match C with
    | RET m                 :: _ -> (RET m, C)
    | Label lab :: RET m    :: _ -> (RET m, C)
    | Label lab             :: _ -> (GOTO lab, C)
    | GOTO lab              :: _ -> (GOTO lab, C)
    | _                          -> let lab = newLabel()
                                    (GOTO lab, Label lab :: C)
// TCALL  move m args down n, jump                            
let makeCall m lab C : instr list = 
    match C with
    | RET n             :: C1 -> TCALL(m, n, lab) :: C1
    | Label _ :: RET n  :: _  -> TCALL(m, n, lab) :: C
    | _                       -> CALL(m, lab) :: C

//作用应该是去除C中的第一个Label及之前的所有的代码
let rec deadcode C =
    match C with
    | []                -> []
    | Label lab :: _    -> C
    | _         :: C1   -> deadcode C1

//添加NOT（s[sp] != 0 ）
let addNOT C = 
    match C with
    | NOT        :: C1 -> C1
    | IFZERO lab :: C1 -> IFNZRO lab :: C1
    | IFNZRO lab :: C1 -> IFZERO lab :: C1
    | _                -> NOT :: C

//jump is GOTO or RET 把jump通过判断添加进去
let addJump jump C =
    let C1 = deadcode C
    match (jump, C1) with
    | (GOTO lab1, Label lab2 :: _)  -> if lab1=lab2 then C1
                                       else GOTO lab1 :: C1
    | _                             -> jump :: C1

//调用addjump函数，把goto加入进去
let addGOTO lab C =
    addJump (GOTO lab) C

//添加操作符
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

//添加float
let rec addCSTF i C =
    match (i, C) with
    | _                     -> (CSTF (System.BitConverter.ToInt32(System.BitConverter.GetBytes(float32(i)), 0))) :: C

//添加char
let rec addCSTC i C =
    match (i, C) with
    | _                     -> (CSTC ((int32)(System.BitConverter.ToInt16(System.BitConverter.GetBytes(char(i)), 0)))) :: C

let rec addCSTS (s:string) C =
    match (s, C) with
    | _                     -> let mutable list =[];
                               for i = 0 to s.Length do
                                   list <- (int (s.Chars(i)))::list
                               (CSTS list) :: C

// 多态类型 Env 
// 环境Env 是 元组 ("name",data)的列表 
// 值 data可以是任意类型
type 'data Env = (string * 'data) list

//环境查找函数 
//在环境env上查找名称为x的值
let rec lookup env x = 
    match env with
    | []            -> failwith(x + " not found")
    | (y, v)::yr    -> if x=y then v else lookup yr x

//在环境env上查找名称为x的结构体
let rec structLookup env x =
    match env with
    | []                            -> failwith(x + " not found")
    | (name, arglist, size)::rhs    -> if x = name then (name, arglist, size) else structLookup rhs x


type Var = 
    | Glovar of int    //栈内绝对地址
    | Locvar of int         //相对地址
    | StructMemberLoc of int     //结构成员变量的相对地址

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




//变量环境列表
//变量环境跟踪全局和局部变量，以及跟踪局部变量的下一个可用偏移量
type VarEnv = (Var * typ) Env * int

//结构体类型环境跟踪结构体类型
type StructTypeEnv = (string * (Var * typ) Env * int) list 

//函数参数例子:
//void func (int a , int *p)
// 参数声明列表为: [(TypI,"a");(TypP(TypI) ,"p")]
type Paramdecs = (typ * string) list

(* 函数环境列表  
    ”函数名“ ”返回类型“ “参数列表”
*)
type FunEnv = (label * typ option * Paramdecs) Env

//代码指令列表
type LabEnv = label list


//绑定varEnv中声明的变量并生成代码来分配它
let allocate (kind : int -> Var) (typ, x) (varEnv : VarEnv) (structEnv : StructTypeEnv): VarEnv *  instr list =
    let (env, fdepth) = varEnv
    match typ with
    | TypA (TypA _, _)    -> failwith "Warning: allocate-arrays of arrays not permitted" 
     //如果是数组，就分配i个空间，由于fdepth是指向下一个的，所以要加i+1
    | TypA (t, Some i)         ->
        let newEnv = ((x, (kind (fdepth+i), typ)) :: env, fdepth+i+1)
        let code = [INCSP i; GETSP; CSTI (i-1); SUB]
        (newEnv, code)
      //如果是结构体，先从结构体环境中查找结构体信息，然后分配 size + 1个空间
    | TypS         ->
        let newEnv = ((x, (kind (fdepth+128), typ)) :: env, fdepth+128+1)
        let code = [INCSP 128; GETSP; CSTI (128-1); SUB]
        (newEnv, code)
      //如果是结构体，先从结构体环境中查找结构体信息，然后分配 size + 1个空间
    | TypeStruct structName     ->
        let (name, argslist, size) = structLookup structEnv structName
        let code = [INCSP (size + 1); GETSP; CSTI (size); SUB]
        let newEnvr = ((x, (kind (fdepth + size + 1), typ)) :: env, fdepth+size+1+1)
        (newEnvr, code)
    //如果是其他的类型，只要分配1个空间就可以了
    | _     ->
        let newEnv = ((x, (kind (fdepth), typ)) :: env, fdepth+1)
        let code = [INCSP 1]
        (newEnv, code)

//绑定单个声明的变量到变量环境列表中
let bindParam (env, fdepth) (typ, x) : VarEnv =
    ((x, (Locvar fdepth, typ)) :: env, fdepth+1);

//绑定多个声明的变量到变量环境列表中
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


(*  编译micro-C语句：
    * stmt是要编译的语句
    * varEnv是局部和全局变量环境
    * funEnv是全局函数环境
    * lablist 是伪指令列表
    * structEnv是全局结构体环境
    * C是stmt代码编译后的代码
*)
let rec cStmt stmt (varEnv : VarEnv) (funEnv : FunEnv) (lablist : LabEnv) (structEnv : StructTypeEnv) (C : instr list) : instr list = 
    match stmt with
     //如果要编译的stmt是if语句
    | If(e, stmt1, stmt2) ->
        let (jumpend, C1) = makeJump C
        let (labelse, C2) = addLabel (cStmt stmt2 varEnv funEnv lablist structEnv C1)
        cExpr e varEnv funEnv lablist structEnv (IFZERO labelse :: cStmt stmt1 varEnv funEnv lablist structEnv (addJump jumpend C2))
     //如果要编译的stmt是while语句
    | While(e, body) ->
        let labbegin = newLabel()
        let (labend,Cend)   = addLabel C
        let lablist = labend :: labbegin :: lablist
        let (jumptest, C1) = 
            makeJump (cExpr e varEnv funEnv lablist structEnv (IFNZRO labbegin :: Cend))
        addJump jumptest (Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1)
     //如果要编译的stmt是DoUntil语句
    | DoUntil(body,e) ->
        let labbegin = newLabel()
        let C1 = 
            cExpr e varEnv funEnv lablist structEnv (IFZERO labbegin :: C)
        Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1
     //如果要编译的stmt是MatchItem语句
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
      //如果要编译的stmt是Switch语句
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
     //如果要编译的stmt是Case语句
    | Case(cond,body)  ->
        C
    | Pattern(cond,body) ->
        C
    | MatchAll( body ) ->
        C
    //如果要编译的stmt是DoWhile语句
    | DoWhile(body, e) ->
        let labbegin = newLabel()
        let C1 = 
            cExpr e varEnv funEnv lablist structEnv (IFNZRO labbegin :: C)
        Label labbegin :: cStmt body varEnv funEnv lablist structEnv C1 //先执行body
    //如果要编译的stmt是For语句
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
    //如果要编译的stmt是Expr表达式
    | Expr e ->
        cExpr e varEnv funEnv lablist structEnv (addINCSP -1 C)
    | Forin(dec,i1,i2,body) ->
        let rec tmp stat =
                    match stat with
                    | Access (c) -> c 
        let rec idx stat =
                    match stat with
                    | AccIndex (acc,idx) -> idx
        let rec address stat =
                    match stat with
                    | AccIndex (acc,idx) -> acc
        match i1 with 
        | Access acc  -> 
           
            let labend   = newLabel()                       //结束label
            let labbegin = newLabel()                       //设置label 
            let labope   = newLabel()                       //设置 for(,,opera) 的label
            let lablist = labend :: labope :: lablist
            
            
            let Cend =  Label labend ::  C 
            let C1 = Label labope ::  cExpr (Assign ( AccVar("tI"), idx acc )) varEnv funEnv lablist structEnv (addINCSP -1 Cend)
            let (jumptest, C2) =                                                
                makeJump (cExpr (Prim2("!=", Access (AccVar("tI")) ,idx (tmp i2) )) varEnv funEnv lablist structEnv (IFNZRO labbegin :: C1)) 
            
            let C4 = cStmt body varEnv funEnv lablist structEnv C2
            let C5  = Label labope :: cExpr ( Assign ( AccVar("tI"),  Prim2("+", Access (AccVar("tI")) ,CstI 1) )  ) varEnv funEnv lablist structEnv (addINCSP -1 C4)
            let C6 = Label labope :: cExpr ( Assign ( dec, Access( AccIndex(  acc , Access( AccVar("tI") ) ))  )  ) varEnv funEnv lablist structEnv (addINCSP -1 C5)
            
          
            cExpr (Assign ( dec, i1 )) varEnv funEnv lablist structEnv (addINCSP -1 (addJump jumptest  (Label labbegin :: C6) ) )
        | _  -> 
            let ass = Assign ( dec,i1)
            let judge =  Prim2("<",Access dec,i2)  
            let opera = Assign ( dec, Prim2("+",Access dec,CstI 1))
            cStmt (For (ass,judge,opera,body))    varEnv funEnv lablist structEnv C
     //如果要编译的stmt是Block
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
    //如果要编译的stmt是返回空
    | Return None ->
        RET (snd varEnv - 1) :: deadcode C
    //如果要编译的stmt是Return
    | Return (Some e) ->
        cExpr e varEnv funEnv lablist structEnv (RET (snd varEnv) :: deadcode C)
    //如果要编译的stmt是Break
    | Break ->
        let labend = headlab lablist
        addGOTO labend C
     //如果要编译的stmt是Continue
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

 //e是要编译的表达式
 //varEnv是编译时变量环境
 //funEnv是编译时环境
 //lablist是编译的伪代码列表
 //structEnv是编译的结构体环境
 //C是这个表达式的代码后面的代码
and cExpr (e : expr) (varEnv : VarEnv) (funEnv : FunEnv) (lablist : LabEnv) (structEnv : StructTypeEnv) (C : instr list) : instr list =
    match e with
    //声明
    | Access acc        -> cAccess acc varEnv funEnv lablist structEnv (LDI :: C)
     //赋值
    | Assign(acc, e)    -> cAccess acc varEnv funEnv lablist structEnv (cExpr e varEnv funEnv lablist structEnv (STI :: C))
    | ToInt e           -> match e with
                           |  ConstFloat f -> addCST (int f) C
                           |  ConstChar c -> addCST (int c - int '0') C
    | ToChar e          -> match e with
                           |  CstI i -> addCSTC (i + int '0') C                        
    | ToFloat e         -> match e with
                           |  CstI i -> addCSTF (float32 i) C                     
    | ConstFloat i      -> addCSTF i C     //浮点数
    | ConstString s     -> addCSTS s C     //字符串
    | ConstChar i       -> addCSTC (int i) C   //字符
    | CstI i            -> addCST i C   //整数
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
                let C1 = cExpr ass varEnv funEnv lablist structEnv C
                CSTI 1 :: ADD :: (addINCSP -1 C1)
            | "-B" ->
                let ass = Assign (acc,Prim2("-",Access acc, e))
                let C1 = cExpr ass varEnv funEnv lablist structEnv C
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
    //输出
    | Print(ope,e1)  ->
         cExpr e1 varEnv funEnv lablist structEnv
            (match ope with
            | "%d"  -> PRINTI :: C
            | "%c"  -> PRINTC :: C
            | "%f"  -> PRINTF :: C
            )
    | PrintHex(hex,e1)  -> failwith("Error")
    | Println acc -> failwith("Error")
    //单目运算
    | Prim1(ope, e1) ->
        let rec tmp stat =
                    match stat with
                    | Access (c) -> c               //get IAccess
        cExpr e1 varEnv funEnv lablist structEnv
            (match ope with
            | "!"       -> addNOT C
            | _         -> failwith "Error: unknown unary operator")
    //双目运算
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
    //三目运算
    | Prim3(cond, e1, e2)    ->
        let (jumpend, C1) = makeJump C
        let (labelse, C2) = addLabel (cExpr e2 varEnv funEnv lablist structEnv C1)
        cExpr cond varEnv funEnv lablist structEnv (IFZERO labelse :: cExpr e1 varEnv funEnv lablist structEnv (addJump jumpend C2))
     //与
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
     //或
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
    //函数 F（es）
    | Call(f, es)   -> callfun f es varEnv funEnv lablist (structEnv : StructTypeEnv) C

//给结构体分配环境空间
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


//给结构体创建环境（调用分配空间函数）
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
    
//创建全局环境 变量环境VarEnv 函数环境FunEnv 结构体类型环境StructTypeEnv 虚拟机操作list
and makeGlobalEnvs(topdecs : topdec list) : VarEnv * FunEnv * StructTypeEnv * instr list =
    //绑定添加函数
    let rec addv decs varEnv funEnv structTypEnv =
        match decs with
         //如果语句为空，则返回环境
        | [] -> (varEnv, funEnv, structTypEnv, [])
         //如果语句列表不为空，则把list的head也就是第一个选取出来进行模式配对
        | dec::decr ->
            match dec with
            //如果匹配到Vardec（也就是我们定义的变量声明 typ*string <类型+名称>）
            | Vardec (typ, x) -> 
                //allocate函数对变量绑定分配,返回变量环境varEnv1和虚拟机操作code1
                let (varEnv1, code1) = allocate Glovar (typ, x) varEnv structTypEnv
                //递归更新
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv1 funEnv structTypEnv
                 //将每次的虚拟机操作添加到code1上，再返回全局环境
                (varEnvr, funEnvr, structTypEnvr, code1 @ coder)
             //如果匹配到的是VariableDeclareAndAssign（也就是变量声明和赋值 typ * string * expr）
            | VariableDeclareAndAssign (typ, x, e) -> 
                //allocate函数对变量绑定分配,返回变量环境varEnv1和虚拟机操作code1
                let (varEnv1, code1) = allocate Glovar (typ, x) varEnv structTypEnv
                 //递归更新
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv1 funEnv structTypEnv
                //添加 访问变量x的代码
                (varEnvr, funEnvr, structTypEnvr, code1 @ (cAccess (AccVar(x)) varEnvr funEnvr [] structTypEnv (cExpr e varEnvr funEnvr [] structTypEnv (STI :: (addINCSP -1 coder)))))
             //如果匹配的是Fundec（ typ option * string * (typ * string) list * stmt）
            | Fundec (tyOpt, f, xs, body) ->
                addv decr varEnv ((f, (newLabel(), tyOpt, xs)) :: funEnv) structTypEnv
             //如果匹配的是结构体
            |Structdec (typName, typEntry) -> 
                let structTypEnv1 = makeStructEnvs typName typEntry structTypEnv
                let (varEnvr, funEnvr, structTypEnvr, coder) = addv decr varEnv funEnv structTypEnv1
                (varEnvr, funEnvr, structTypEnvr, coder)
                
    addv topdecs ([], 0) [] []

//生成代码以访问变量、取消引用指针或索引数组
and cAccess access varEnv funEnv lablist (structEnv : StructTypeEnv) C =
    match access with
    //若匹配的是可访问的变量x
    | AccVar x  ->
        match lookup (fst varEnv) x with
        | Glovar addr, _ -> addCST addr C
        | Locvar addr, _ -> GETBP :: addCST addr (ADD :: C)
    //若匹配的是可访问的结构体
    | AccStruct (AccVar stru, AccVar memb) ->
        let (loc, TypeStruct structname)   = lookup (fst varEnv) stru
        let (name, argslist, size) = structLookup structEnv structname
        match structLookupVar argslist memb 0 with
        | StructMemberLoc varLocate ->
            match lookup (fst varEnv) stru with
            | Glovar addr, _ -> addCST (addr - (size+1) + varLocate) C
            | Locvar addr, _ -> GETBP :: addCST (addr - (size+1) + varLocate) (ADD ::  C)
     //若匹配的是可访问的指针
    | AccDeref e ->
        cExpr e varEnv funEnv lablist structEnv C
     //若匹配的是可访问的数组
    | AccIndex(acc, idx)    ->
        match acc with
        |  AccStruct (AccVar stru, AccVar memb) ->
            cAccess acc varEnv funEnv lablist structEnv (cExpr idx varEnv funEnv lablist structEnv (ADD :: C))
        | _     ->   
            cAccess acc varEnv funEnv lablist structEnv (LDI :: cExpr idx varEnv funEnv lablist structEnv (ADD :: C))

//cExprs用来处理多个需要编译的表达式
and cExprs es varEnv funEnv lablist (structEnv : StructTypeEnv) C = 
    match es with
    | []        -> C
    | e1::er    -> cExpr e1 varEnv funEnv lablist structEnv (cExprs er varEnv funEnv lablist structEnv C)

//函数 F（es）
and callfun f es varEnv funEnv lablist (structEnv : StructTypeEnv) C : instr list =
    let (labf, tyOpt, paramdecs) = lookup funEnv f
    let argc = List.length es
    if argc = List.length paramdecs then
        cExprs es varEnv funEnv lablist structEnv (makeCall argc labf C)
    else
        failwith (f + ": parameter/argument mismatch")

//编译一个完整的micro-C程序：globals，调用main，函数
let cProgram (Prog topdecs) : instr list = 
    let _ = resetLabels ()
    //先处理环境
    let ((globalVarEnv, _), funEnv, structEnv, globalInit) = makeGlobalEnvs topdecs
    //编译函数
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
    //先找到main函数，然后找到main函数的参数
    let (mainlab, _, mainparams) = lookup funEnv "main"
    let argc = List.length mainparams
    globalInit
    @ [LDARGS; CALL(argc, mainlab); STOP]
    @ List.concat functions


//编译程序（抽象语法）并将其写入文件名称；另外，以指令列表的形式返回程序。

let intsToFile (inss : int list) (fname : string) =
    File.WriteAllText(fname, String.concat " " (List.map string inss))


let contCompileToFile program fname =
    let instrs      = cProgram program
    let bytecode    = code2ints instrs
    intsToFile bytecode fname; instrs
