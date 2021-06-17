### 文件说明

#### interpreter  解释器

```sh
Absyn.fs micro-C abstract syntax                              抽象语法
grammar.txt informal micro-C grammar and parser specification 文法定义
CLex.fsl micro-C lexer specification                          fslex词法定义
CPar.fsy micro-C parser specification                         fsyacc语法定义
Parse.fs micro-C parser                                       语法解析器
Interp.fs micro-C interpreter                                 解释器
ex1.c-ex21.c micro-C example programs                         例子程序
interpc.fsproj                                                项目文件
```

#### compiler  编译器

```sh
Machine.fs definition of micro-C stack machine instructions  VM 指令定义
Machine.java micro-C stack machine in Java                   VM 实现 java
machine.c micro-C stack machine in C                         VM 实现 c 
machine.cs micro-C stack machine in C#                       VM 实现 c#
machine.csproj  machine project file                         VM 项目文件

Comp.fs compile micro-C to stack machine code                编译器
prog0 example stack machine program: print numbers           字节码 案例
prog1 example stack machine program: loop 20m times          字节码 案例
microc.fsproj                                                编译器项目文件
```

#### compiler  优化编译器

```sh
Contcomp.fs compile micro-C backwards                   优化编译器
microcc.fsproj                                          优化编译器项目文件
```

### A 解释器
#### A.1  解释器 interpc.exe 构建步骤

```sh
//编译解释器 interpc.exe 命令行程序 
dotnet restore  interpc.fsproj  //可选
dotnet clean  interpc.fsproj    //可选
dotnet build -v n interpc.fsproj //构建，-v n查看详细生成过程

//执行解释器
./bin/Debug/net5.0/interpc.exe ex1.c 8
dotnet run -p interpc.fsproj ex1.c 8
dotnet run -p interpc.fsproj -g ex1.c 8  //显示token AST 等调试信息
```

#### A.2 解释器命令行fsi中运行 

```sh
//生成扫描器
dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fslex/netcoreapp3.1\fslex.dll"  -o "CLex.fs" --module CLex --unicode CLex.fsl

//生成分析器
dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fsyacc/netcoreapp3.1\fsyacc.dll"  -o "CPar.fs" --module CPar CPar.fsy

//命令行运行程序
dotnet fsi 

//#r "./bin/Debug/net5.0/FsLexYacc.Runtime.dll ";;     
#r "nuget: FsLexYacc";;  //添加包引用
#load "Absyn.fs" "Debug.fs" "CPar.fs" "CLex.fs" "Parse.fs" "Interp.fs" "ParseAndRun.fs" ;; 

open ParseAndRun;;    //导入模块 ParseAndRun
fromFile "ex1.c";;    //显示 ex1.c的语法树
run (fromFile "ex1.c") [17];; //解释执行 ex1.c
run (fromFile "ex11.c") [8];; //解释执行 ex11.c
#q;;

```

解释器的主入口 是 interp.fs 中的 run 函数，具体看代码的注释

### B 编译器 
#### B.1 microc编译器构建步骤

```sh
//构建 microc.exe 编译器程序 
dotnet restore  microc.fsproj //可选
dotnet clean  microc.fsproj  //可选
dotnet build  microc.fsproj  //构建

dotnet run -p microc.fsproj ex1.c    // 执行编译器，编译 ex1.c，并输出  ex1.out 文件
dotnet run -p microc.fsproj -g ex1.c   // -g 查看调试信息

./bin/Debug/net5.0/microc.exe -g ex1.c  // 直接执行构建的.exe文件，同上效果
```

#### B.2 dotnet fsi 中运行编译器

```sh
//启动fsi
dotnet fsi -r ./bin/Debug/net5.0/FsLexYacc.Runtime.dll Debug.fs Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Comp.fs ParseAndComp.fs   

//fsi 中输入，运行编译器
open ParseAndComp;;
compileToFile (fromFile "ex11.c") "ex11.out";;  //生成机器码ex11.out
#q;;


// fsi 中运行
#time "on";;  // 打开时间跟踪
// 参考A. 中的命令 比较下解释执行解释执行 与 编译执行 ex11.c 的速度
```

###  C 优化编译器 
#### C.1  优化编译器 microcc.exe 构建步骤

```sh

dotnet restore  microcc.fsproj
dotnet clean  microcc.fsproj
dotnet build  microcc.fsproj  //构建编译器

dotnet run -p microcc.fsproj ex11.c //执行编译器
./bin/Debug/net5.0/microcc.exe ex11.c  //直接执行

```

#### C.2 dotnet fsi 中运行 backwards编译器  

```sh
dotnet fsi -r ./bin/Debug/net5.0/FsLexYacc.Runtime.dll Absyn.fs CPar.fs CLex.fs Parse.fs Machine.fs Contcomp.fs ParseAndContcomp.fs   

open ParseAndContcomp;;
contCompileToFile (fromFile "ex11.c") "ex11.out";;
#q;;
```

### D 虚拟机构建 Machine

- 运行下面的命令 查看 fac 0 , fac 3 的栈帧 
- 理解栈式虚拟机执行流程

#### D.1 dotnet
```sh
dotnet run -p machine.csproj ex9.out 3 // 运行虚拟机，执行 ex9.out 

./bin/Debug/net5.0/machine.exe ex9.out 3
./bin/Debug/net5.0/machine.exe -t ex9.out 0  // 运行虚拟机，执行 ex9.out ，-t 查看跟踪信息
./bin/Debug/net5.0/machine.exe -t ex9.out 3  // 运行虚拟机，执行 ex9.out ，-t 查看跟踪信息
```

#### D.2 Linux/C
```sh
cc machine.c -o machine

machine ex9.out 3
machine -trace ex9.out 0  // -trace  并查看跟踪信息
machine -trace ex9.out 3
```
#### D.3 Java
```sh
javac Machine.java
java Machine ex9.out 3

javac Machinetrace.java
java Machinetrace ex9.out 0
java Machinetrace ex9.out 3
```




