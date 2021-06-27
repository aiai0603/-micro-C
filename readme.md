[2020-2021学年第2学期]

# [**实 验 报 告**]

![zucc](./img/zucc.png)

- 课程名称:编程语言原理与编译
- 实验项目:期末大作业
- 专业班级__计算机1803_
- 学生学号_31801150_ 31801066_
- 学生姓名 张帅  沈文燕
- 实验指导教师:郭鸣

| 姓名   | 学号     | 班级       | 任务                                                         | 权重 |
| ------ | -------- | ---------- | ------------------------------------------------------------ | ---- |
| 张帅   | 31801150 | 计算机1803 | 解释器（主要）、编译器（一部分）、Java虚拟机（修改）、测试、文档 | 1.0  |
| 沈文燕 | 31801066 | 计算机1801 | 编译器（主要）、测试、文档、注释                             | 0.9  |

成员代码提交日志

![image-20210627192044858](.\img\record.png)



1. 项目自评等级:(1-5) 请根据自己项目情况填写下表

   （完善程度：个人认为实现的完善程度，个人花费的工作量等
   	难度：实现的困难程度，工作量大小，老师定义的困难程度等

| 解释器                   | 完善程度 | 难度 | 备注                           |
| ------------------------ | -------- | ---- | ------------------------------ |
| 各类数组，数组检查       | 5        | 3    | 判定调用是否越界               |
| bool类型                 | 5        | 1    |                                |
| 数强制转型               | 5        | 1    |                                |
| 字符类型                 | 5        | 1    |                                |
| dountil循环              | 5        | 1    |                                |
| dowhile循环              | 5        | 1    |                                |
| float类型                | 5        | 1    |                                |
| for循环                  | 5        | 2    |                                |
| forin（支持数组和纯数字) | 5        | 4    |                                |
| 函数返回值               | 3        | 3    | 实现方式比较粗暴（见下)        |
| 按照进制创建整数         | 3        | 3    |                                |
| 数据初值定义             | 3        | 3    | 不支持数组和结构体的初值       |
| 模式匹配                 | 1        | 1    | 只能根据数值匹配（类似switch） |
| 三目运算                 | 5        | 1    |                                |
| += 等语法糖              | 5        | 1    |                                |
| 自增自减（++/--)         | 5        | 3    | 可以识别 ++i 和 i++            |
| 计算长度                 | 5        | 2    |                                |
| 字符串                   | 5        | 2    |                                |
| 结构体                   | 5        | 5    |                                |
| switch  case  default    | 5        | 2    |                                |
| 类型检查                 | 5        | 5    | 支持一定范围的自动转型（见下） |
| 变量名称检查             | 5        | 2    | 只能以小写英文字母开头         |

| 编译器                | 完善程度 | 难度 | 备注                                |
| --------------------- | -------- | ---- | ----------------------------------- |
| 各类数组，数组检查    | 5        | 3    | 判定调用是否越界                    |
| bool类型              | 5        | 1    |                                     |
| 数强制转型            | 5        | 1    |                                     |
| 字符类型              | 5        | 1    |                                     |
| dountil循环           | 5        | 1    |                                     |
| dowhile循环           | 5        | 1    |                                     |
| float类型             | 5        | 1    |                                     |
| for循环               | 5        | 3    |                                     |
| forin（支持纯数字)    | 5        | 3    |                                     |
| 函数返回值            | 3        | 1    |                                     |
| 按照进制创建整数      | 3        | 2    |                                     |
| 数据初值定义          | 3        | 3    | 不支持数组和结构体的初值            |
| 模式匹配              | 1        | 1    | 只能根据数值匹配（类似switch）      |
| 三目运算              | 5        | 1    |                                     |
| += 等语法糖           | 5        | 1    |                                     |
| 自增自减（++/--)      | 3        | 3    | 可以识别 ++i 和 i++，但是返回值不对 |
| 结构体                | 5        | 4    |                                     |
| switch  case  default | 5        | 2    |                                     |
| 变量名称检查          | 5        | 2    | 只能以小写英文字母开头              |
| trycatch              | 5        | 4    |                                     |
| 修改Java虚拟机        | 5        | 1    | 支出了一条新的指令输出浮点数        |

2. 项目说明

   - 整体文件架构

     src文件夹               Java虚拟机

     TestInterp文件夹  解释器测试集

     TestParse文件夹   编译器测试集

     Absyn.fs                 抽象语法

     CLex.fsl          		fslex词法定义

     CPar.fsy             	fsyacc语法定义

     Parse.fs                 语法解析器

     Interp.fs                 解释器

     interpc.fsproj        项目文件

     Contcomp.fs         编译器

     Machine.fs            指令定义

     microcc.fsproj      编译器项目文件

   - 项目运行

     **解释器：**

     dotnet restore interpc.fsproj //可选

     dotnet clean interpc.fsproj  //可选

     dotnet build -v n interpc.fsproj //构建，-v n查看详细生成过程

     ./bin/Debug/net5.0/interpc.exe  测试的文件 参数

     dotnet run -p interpc.fsproj 测试的文件 参数

     dotnet run -p interpc.fsproj -g 测试的文件 参数 //显示token AST 等调试信息  

     **编译器：**

     dotnet restore microcc.fsproj

     dotnet clean microcc.fsproj

     dotnet build microcc.fsproj //构建编译器

     dotnet run -p microcc.fsproj 测试的文件 //执行编译器

     ./bin/Debug/net5.0/microcc.exe 测试的文件 //直接执行

     **Java虚拟机：**

     javac Machine.java

     java Machine 测试的文件（.out)  参数 

     java Machinetrace 测试的文件 参数 //可以查看栈

     

   - 解释器部分是基于现有代码 MICROC 的改进，主要添加了如上表的功能，在我刚刚接触到代码的时候，我第一次阅读，发现 MICROC居然只能存储int类型的数据，我尝试在只能存储int类型的数据的情况下实现了string类型（非常非常非常非常困难，详见git的提交记录），但是后续提升四处碰壁，因为不能存储类型在结构中，实现类型检查简直是天方夜谭，最后我被迫修改了store的存储类型，从而重构了整个存储体系（技术要点中会说明），在新的系统架构下，我主要实现了以下功能：

     - 解释器支持以下类型：

       - 整数类型（原 MICROC 已经实现的内容）

         修改为通过Int关键字定义，使用createI函数可以根据2-16进制，使用字符串创建一个int类型的数值：

         ```c
        Void main(Int n) {
             Int a;
        a = createI("F122",16);
             print(16,a);
             print("%d",a);
             Int b;
             b = createI("1010",2);
             print(2,b);
             print("%d",b);
         }
         ```

         运行结果:
          ![image-20210627102433014](.\img\16.png)

       - 指针（原 MICROC 已经实现的内容）

       - 浮点数类型
         通过Float关键字定义，系统将正则表达式可以匹配为   `   ['0'-'9']+'.'['0'-'9']+ ` 的内容匹配为float数据类型,float类型之间可以进行基础的加减乘除操作等：
       
         ```c
         Void main(Int n) {
          Float a;
          a = 1.375;
          Float b;
          b = 1.5;
          print("%f",(a+b));
         }
         ```
          运行结果：
       
         ![image-20210626212313824](.\img\3.png)
       
       - bool类型
         通过Bool关键字定义，只支持定义为false和true，bool类型的数据可以自动转型为int类型输出（0和1）

         ```c
            Void main(Int n) {
                Bool b = true;
              	print("%d",b);
             }
         ```

         运行结果:

         ![image-20210626211109640](.\img\2.png)

       - 字符串类型

         通过String关键字定义一个字符串常量，可以将双引号包含的内容识别为字符串并存储

         ```c
          Void main(Int n) {
             String a;
             a="test1";
             print("%s",a);
             String b;
             b="mytest2";
             print("%s",a);
             print("%s",b);
             Int c;
             c=12;
             b="mytest2";
             print("%s",a);
             print("%s",b);
             print("%d",c);
         }
         ```
         运行结果：

         ![image-20210626220059120](.\img\4.png)

       - 字符类型

         通过Char关键字定义，原来的MICROC只能输出\n字符类型，新的解释器中通过将单引号中的指定字符识别为char进行存储
       
         ```c
         Void main(Int n) {
             Char a;
             a = '3';
             print("%c",a);
          }
         ```
       
         运行结果：
       
         ![image-20210626221751775](.\img\5.png)
       
       - 数组类型

         优化现有的数组的定义，在数组调用时，可以判定数组是否越界，对于越界的调用进行报错

         ```c
         Void main(Int n) {
             Int a[6];
             a[0]=1222;
             print("%d",a[0]);
             a[6] = 2;
         }
         ```
         运行结果：

         ![image-20210626221751775](.\img\1.png)

       - 结构体

         实现了结构体数据类型，在main函数之前进行定义，之后再主函数中可以定义指定数据结构的实体类型，并操作其中的元素

         ```c
         Struct test2 {
                 Int a;
           Float b;
         } ;
         Struct test1 {
                 Int a;
                 Float f;
                 Int arr[100];
                 Char c;
                 Bool b;
                 String s;
         } ; 
         Void main(Int n) {
           Int test3 ;
           test3 = 23;
           Struct test1 t;
           Struct test2 t2;
           t.a = 145;
           t.f = 12.3;
           t.s = "asasasas";
           t.arr[2]= 1;
           t.arr[5]= 2;
           t.arr[6]= 5;
           t2.a = 12121212;
           t.c = '1';
           t.b = false;
           print("%c",t.c);
           print("%d",t.b);
           print("%d",t2.a);
           print("%s",t.s);
           print("%d",t.a);
           print("%f",t.f);
           print("%d",t.arr[6]);
           print("%d",test3);
         }
         ```
         运行结果：

         ![image-20210626223101093](.\img\6.png)
       
     - 解释器数据类型支持以下的规则（将会在技术要点具体阐释）

       - 存储

         解释器中，数据通过以下的数据结构进行存储，将数据直接区分为不同的类型，不同类型的数据遵循不同的运算规则
         
         ```fsharp
         type mem =
         | INT of int
         | STRING of string
         | POINTER of int
         | FLOAT of float
         | CHAR of char
         | BOOLEAN of bool
         | STRUCT of string*int*int
         | ARRAY of typ*int*int
         ```
         
         通过定义的typeOf函数可以直接输出表达式的类型（以string形式返回）
         
         ```c
         Struct test1 {
                 Int a;
                 Float f;
                 Int arr[10];
                 Char c;
                 Bool b;
                 String s;
         } ;
         Void main(Int n) {
             Struct test1 t;
             Int ar[6];
             t.a = 145;
             t.f = 12.3;
             t.s = "asasasas";
             t.arr[2]= 1;
             t.arr[5]= 2;
             t.arr[6]= 5;
             t.b = false;
             t.c ='1';
             print("%s",typeOf(ar));
             print("%s",typeOf(t));
             print("%s",typeOf(t.a));
             print("%s",typeOf(t.f));
             print("%s",typeOf(t.c));
             print("%s",typeOf(t.b));
             print("%s",typeOf(t.s));
             print("%s",typeOf(t.arr[5]));
             print("%s",typeOf((float)t.a+1.4));
             print("%s",typeOf(1>2));
         }
         ```
        运行结果：
         
         ![image-20210627090321226](.\img\9.png)

       - 自动转型（弱类型）、类型检查

         在不同类型的数据进行运算时，遵循以下的转换规则，如果不能转换，会进行报错，每个表达式和函数等都有自己的运行需求参数，然后进过自动转型之后，还是不能满足需求，则会报。

         int -> bool、float、char
         char -> int
         float -> char、int
         bool-> int

         ```c
          Void main() {
                      print("%f",12);
                      print("%d",12.5);
                      print("%d",'3');
                      print("%c",67);
                      print("%c",12.12);
         }
         ```
          运行结果：

         ![image-20210627090023266](.\img\8.png)
         
       - 强制转型
       
         解释器除了在运算时，使用上述规则进行转换以外，还支持强制的类型转换，其中，int 0-9可以强制转换为char的‘0’-‘9’，反向转换同理；float类型在转换int时，将直接截断小数部分，目前只支持使用（int）、（char）、（float）三种强制转换符号

         ```c
             Void main() {
             print("%f",(((float)'c')+12));
                  print("%d",(int)12.5);
                  print("%d",(int)'3');
                  print("%c",(char)2);
                  print("%c",12.12);
              }
         ```
         
         运行结果：
         
          ![image-20210626230805838](.\img\7.png)
       
     - 变量

       - 变量的定义

           解释器和编译器在获取变量定义时，将对变量的名称进行检查，变量名一定要为小写字母开头，否则将会报错

           ```c
           Void main(Int n) {
               Char Aa;
               Int Asa[10];
            }
           ```
           运行结果: ![image-20210627095747974](.\img\13.png)

       - 定义时赋值
           解释器支持在变量定义时赋初值（不包括数组和结构体）

         ```c
         Void main(Int n) {
                  Int a = 1;
                  print("%d",a);
         }
         ```
         运行结果:
         
         ![image-20210627100127342](.\img\14.png)

       - 变量默认初值

         解释器支持在初始化变量时，每个类型的变量有一个初始值：（int为0，float为0.0，string为"",char为空字符,bool

         为false）:

         ```c
         Void main(Int n) {
                 Int a ;
                 Char b ;
                 String s  ;
                 Float f ;
                 Bool bool;
                 print("%d",a);
                 print("%d",bool);
                 print("%c",b);
                 print("%s",s);
                 print("%f",f);
         }
         ```
         
         运行结果：
         
         ![image-20210627100401270](.\img\15.png)
         
         

     - 解释器支持以下的运算：

       - 原MICROC 自带

         加减乘除和取余数等基本操作，与或非的运算

       - 自增自减

         解释器通过 ++ 和 -- 符号实现自增自减，自增自减识别++i和i++的区别，++i参与运算时，优先自增，之后参与运算；i++参与运算时，先参与运算，在运算结束后在自增，自减同理：

         ```c
         Void main(Int n) {
            Int i;
            Int a;
            i=0;
            do {
               print("%d", ++i);
            }  
            while(i<n);
            i=0;
            do {
           	  print("%d",i++);
            }  
            while(i<n);
            i=20;
            do {
               print("%d",i--);
            } 
            while(i>n); 
            i=20;
            do {
               print("%d",--i);
            }  
            while(i>n);
         }
         ```
          运行结果：
          ![image-20210627092130496](.\img\10.png)

       - 三目运算

         解释器通过a？b：c的实现了三目运算，判定a表达式的值来决定执行b或者c
         
         ```c
          Void main(Int n) {
              Int i;
         	i =  n>2?12:21;
              print("%d",i);
         }
         ```
         运行结果：
         
         ![image-20210627092519532](.\img\11.png)

       - +=等语法糖

         解释器支持使用+=、-=、*=、/=和%=等语法糖的写法

         ```c
         Void main(Int n) {
             Int a;
             a=10;
           	a+=n;
         	print("%d",a);
           	a=10;
         	a-=n;
           	print("%d",a);
         	a=10;
           	a*=n;
         	print("%d",a);
           	a=10;
           	a/=n;
           	print("%d",a);
           	a=10;
         	a%=n;
           	print("%d",a);
         }
         ```
         
          运行结果：
         ![image-20210627092757182](.\img\12.png)
         
       - 计算大小
       
         解释器支持使用sizeOf函数来计算变量的长度（数组返回数组长度、字符串返回字符串长度、结构体返回总体大小、其他返回1）：
         
         ```c
          Struct test1 {
                      Int a;
                      Float f;
                      Int arr[10];
                      Char c;
                      Bool b;
                      String s;
              } ;
              Void main(Int n) {
                  Struct test1 t;
                  Int ar[6];
                  t.arr[2]= 1; 
                  t.a = 145;
                  t.f = 12.3;
                  t.s = "asasasas"; 
                  t.arr[2]= 1;
                  ar[5] = 1;
                  t.b = false;
                  t.c ='1';
                  print("%d",sizeOf(ar));
                  print("%d",sizeOf(t));
                  print("%d",sizeOf(t.a));
                  print("%d",sizeOf(t.f));
                  print("%d",sizeOf(t.c));
                  print("%d",sizeOf(t.b));
                  print("%d",sizeOf(t.s));
                  print("%d",sizeOf(t.arr[5]));
                  print("%d",sizeOf(ar[5]));
                  print("%d",sizeOf((float)t.a+1.4));
               	 print("%d",sizeOf(1>2));
              }
         ```
         运行结果：
         ![image-20210627113453259](.\img\24.png)
     
     - 解释器支持以下语句:
     
       - 原MICORC自带
     
         if语句、while语句、基本的print语句
     
       - 格式化print语句
     
         解释器支持使用一下格式化通配符输出各种类型的数据（%d：int，%c：char，%s：string，%f：float)
     
         ```c
         Void main(Int n) {
                    Int a ;
                    Char b ;
                    String s  ;
                    Float f ;
                    Bool bool;
                    print("%d",a);
                    print("%d",bool);
                    print("%c",b);
                    print("%s",s);
                    print("%f",f);
         }
         ```
     
         运行结果:
     
         ![image-20210627100401270](.\img\15.png)
     
         解释器还支持使用2-16的进制类型说明输出一个格式化的x进制数:
     
         ```c
         Void main(Int n) {
             Int a;
             a = createI("F122",16);
             print(16,a);
             print("%d",a);
             Int b;
             b = createI("1010",2);
           print(2,b);
             print("%d",b);
         }
         ```
     
         运行结果:
     
         ![image-20210627102433014](.\img\16.png)
     
       - dowhile语句
     
         解释器支持类似c语言的dowhile语句:
     
         ```c
          Void main(Int n) {
                   Int i;
                   i=0;
                   do {
                        print("%d",i);
                        n=n-1;
                   }  
                   while(i<n);
          }
         ```
     
         运行结果:
     
         ![image-20210627103028551](.\img\17.png)
         
         dountil语句
       
         解释器支持dountil语句，执行一个函数直到满足某个条件
       
         ```c
         Void main(Int n) {
            Int i;
            i=0;
            do{
                 print("%d",i);
                 i = i+1;
            }  
         until(i>n);
         }
         ```
         
         运行结果:
         
         ![image-20210627103212163](.\img\18.png)
         
       - for语句
         解释器支持类型c语言的for循环语句
       
         ```c
         Void main(Int n) {
              Int i;
           i=0;
              for( i = 0 ; i < n;  i = i + 1){
               i=i+1;
                  print("%d",i);
           }
             print("%d",i);
         }
         ```
       
          运行结果:![image-20210627103406174](.\img\19.png)
       
       - forin语句
       
         解释器支持forin的语法，类似如下的定义方式，for  i  in  (a,b) ,如果a与b为int类型，则i将在依次赋值为a-b之间的数据，之后执行循环体，如果a与b为数组，则i将依次赋值为数组a到b之间的每一位，之后执行循环体:
       
         ```c
         Void main(Int n) {
             Int i;
          	for i in (3,7)
             {
                 print("%d",i);
             }
         }
         ```
       
         运行结果:
       
         ![image-20210627103840370](.\img\20.png)
       
         ```c
         Void main(Int n) { 
             Int a[10];
             a[3] =1;
             a[4] =2;
             a[5] =3;
          a[6] =4;
             a[7] =1;
          Int i;
             Int tI;
          for i in (a[3],a[7])
             {
                 print("%d",i);    
          }
         }
         ```
       
         运行结果：
       
         ![image-20210627103941645](.\img\21.png)
       
       - switch-case-default
         实现了不同于c语言的switch语句，当一个case语句匹配完成时，退出语句switch执行体，default关键字可以匹配全部的条件
       
         ```c
           Void main(Int n) {
             switch( n ){
                    case 1 :  print("%d",n);
                 case 2 :  print("%d",n+1);
                    default : print("%d",2);
                }
             print("%d",n);
            }
         ```
       
         运行结果:   ![image-20210627104231299](.\img\22.png)
       
       - 模式匹配
         本来想实现一个类似fsharp的模式匹配的，但是能力有限，仿照switchcase的模式实现了一个简单的matchPattern，只能精准匹配表达式的数值:
       
         ```c
         Void main(Int n) {
             match n with
             | 2 -> print("%d",n);
             | 3 -> print("%d",n+1);
             | _ -> print("%d",2);
         }
         ```
       
         运行结果:
         ![image-20210627105136300](.\img\23.png)
       
     - 函数
     
       - 支持返回值
     
         函数部分基本按照原来的MICORC代码实现，实现了return函数，可以返回值，尝试实现了简单的递归逻辑,如果返回值为null的，函数的结果按照false处理
     
         ```c
         Int fact(Int i){
             if(i == 1){
                 return 1;
             }else{
                 return i * fact(i - 1);
             }
         }
         Int main(){
             Int n;
             n=4;
             print("%d", fact(n));
             print("%d",n);
         }
         ```
     
         运行结果:![image-20210627114617517](.\img\25.png)

   - 编译器中，在MICORC的源代码的基础上进行改进，受限于指令集和运行栈只能存储int类型，只能将一些可以和int进行转换的参数进行相关的处理，因而，最后没能实现string类型的实现，尝试了将解释器中的大部分功能移植到了编译器中，部分功能可能适配不太完善，其中部分参考了Cuby的代码，为了将转化的结果进行验证，我们使用了Cuby的Java虚拟机，后来为了拓展新的功能，我们修改完善了虚拟机，现在可以更好得支持bool和float对象：
     - 编译器支持以下类型：
       - 整数类型（原 MICROC 已经实现的内容）
         修改为通过Int关键字定义，使用createI函数可以根据2-16进制，使用字符串创建一个int类型的数值：
         ```c
        Void main(Int n) {
             Int a;
             a = createI("F122",16);
          	print("%d",a);
          	Int b;
          	b = createI("1010",2);
          	print("%d",b);
          }
         ```
         运行结果:
        ![image-20210627160717916](.\img\27.png)
       - 指针（原 MICROC 已经实现的内容）
       - 浮点数类型
       通过Float关键字定义，系统将正则表达式可以匹配为   `   ['0'-'9']+'.'['0'-'9']+ ` 的内容匹配为float数据类型,float类型之间可以进行基础的加减乘除操作等,为了实现float的输出，我们在机器指令集中添加了输出float的PRINTF指令，并在Java虚拟机中添加了相应的代码，现在编译器可以更好的支持float的定义与输出了
       
         ```c
         Void main(Int n) {
          Float a;
          a = 1.375;
          Float b;
          b = 1.5;
          print("%f",(a+b));
         }
         ```
          运行结果：
         ![image-20210627161319355](.\img\28.png)
       - bool类型
         通过Bool关键字定义，只支持定义为false和true，bool类型的数据可以自动转型为int类型输出（0和1）
         ```c
          Void main(Int n) {
                Bool b = true;
              	print("%d",b);
             }
         ```
       运行结果:

         ![image-20210627161426798](.\img\29.png)

       - 字符类型
       
         通过Char关键字定义，新的编译中通过将单引号中的指定字符识别为char进行存储，们在机器指令集中添加了输出char的PRINTC指令，并在Java虚拟机中添加了相应的代码，现在编译器可以更好的支持char的定义与输出了
       
         ```c
       Void main(Int n) {
             Char a;
             a = '3';
             print("%c",a);
          }
         ```

         运行结果：
         
         ![image-20210627161632750](.\img\30.png)

       - 数组类型
       
         优化现有的数组的定义，支持多种类型的数组
       
         ```c
         Void main(Int n) {
           
             Int a[6];
           a[0]=1222;
             a[3] = 2;
           Int bb;
             bb=1;
         
             print("%d",a);
             return 0;
         
         }
         
         ```
         运行结果：
         
         ![image-20210627162339194](.\img\31.png)

       - 结构体
       
         实现了结构体数据类型，在main函数之前进行定义，之后再主函数中可以定义指定数据结构的实体类型，并操作其中的元素
       
         ```c
         struct student{
             int number;
           int number2;
             char name[5];
             float id;
         };
         int main(){
             struct student hello;
             hello.number = 10;
             hello.id = 234;
             hello.name[4] = 'c';
             hello.name[0] = 'a';
           print hello.number;
             print hello.name[4];
         }
         ```
         运行结果：
     ![image-20210627163251408](.\img\32.png)

     - 编译器数据类型支持以下的规则

       - 自动转型（弱类型）、类型检查

         在不同类型的数据进行运算时，遵循以下的转换规则，如果不能转换，会进行报错，每个表达式和函数等都有自己的运行需求参数，然后经过自动转型之后，还是不能满足需求，则会报错。

         int -> bool、float
         char -> int
          float -> int
          bool-> int

         ```c
         Void main() {
           
               print("%f",12.12+false);
               print("%f",12.25+1);
               print("%d",'a'+1);
           }
         ```

         运行结果：

         ![image-20210627164655311](.\img\33.png)

       - 强制转型

         编译器除了在运算时，使用上述规则进行转换以外，还支持强制的类型转换，其中，int 0-9可以强制转换为char的‘0’-‘9’，反向转换同理；float类型在转换int时，将直接截断小数部分，目前只支持使用（int）、（char）、（float）三种强制转换符号

         ```c
         Void main() {
             print("%d",(int)12.5);
             print("%d",(int)'3');
             print("%c",(char)2);
         }
         ```

         运行结果：

         ![image-20210627164814080](.\img\34.png)

     - 变量

       - 变量的定义
         编译器和解释器在获取变量定义时，将对变量的名称进行检查，变量名一定要为小写字母开头，否则将会报错： 

         ```c
         Void main(Int n) {
             Char Aa;
             Int Asa[10];
         ```

         运行结果: ![image-20210627095747974](.\img\13.png)

       - 定义时赋值
         编译器支持在变量定义时赋初值(包括全局和内部都可以)

         ```c
         Int  i =1;
         Void main(Int n) {
             Int a = 7;
             Char b = 'c';
             Float f = 12.32; 
             print("%d",i);
             print("%d",a);
             print("%c",b);
           print("%f",f);
           }
         ```

         运行结果:![image-20210627165054615](.\img\35.png)

     - 编译器支持以下的运算：
         
     - 原MICROC 自带
         
           加减乘除和取余数等基本操作，与或非的运算
         
         - 自增自减
         
           编译器通过 ++ 和 -- 符号实现自增自减，
         
           ```c
             Void main(Int n) {
                Int i;
                i=0;
                do {
                    
                    print("%d",i);
                    i++;
                }  
                while(i<n);
                i=0;
                do {
                   
                    print("%d",i);
                    ++i;
                }  
                while(i<n);
                i=20;
                do {
                   
                   print("%d",i);
                   i--;
                } 
                while(i>n); 
                i=20;
                do {
                   
                   print("%d",i);
                   --i;
                }  
                while(i>n);
             }
           ```
         
           运行结果：(有点长，截不全)
           ![image-20210627165622410](.\img\36.png)
         
         - 三目运算
         
           解释器通过a？b：c的实现了三目运算，判定a表达式的值来决定执行b或者c
         
           ```c
           Void main(Int n) {
                Int i;
           	i =  n>2?12:21;
                print("%d",i);
           }
           ```
         
           运行结果：
         
           ![image-20210627165732870](.\img\37.png)
         
         - +=等语法糖
         
           编译器支持使用+=、-=、*=、/=和%=等语法糖的写法
         
           ```c
           Void main(Int n) {
                 Int a;
                 a=10;
               	a+=n;
             	print("%d",a);
               	a=10;
             	a-=n;
               	print("%d",a);
             	a=10;
               	a*=n;
             	print("%d",a);
               	a=10;
               	a/=n;
               	print("%d",a);
               	a=10;
             	a%=n;
               	print("%d",a);
             }
           ```
         
           运行结果：(有点长，截不全)
             ![image-20210627165821524](.\img\38.png)
         

     - 解释器支持以下语句:

       - 原MICORC自带

         if语句、while语句、基本的print语句

       - 格式化print语句

         编译器支持使用一下格式化通配符输出各种类型的数据（%d：int，%c：char，%s：string，%f：float)，编译器执行时严格按照要求，如果输出的参数和通配符不匹配，则报错

         ```c
          Void main(Int n) {
                      Int a =1;
                      Char b ='c';
                      Float f = 12.2;
                      Bool bool = false;
                      print("%d",a);
                      print("%d",bool);
                      print("%c",b);
                      print("%f",f);
                      print("%f",a);
           }
         ```

         运行结果:![image-20210627170311704](.\img\39.png)

       - dowhile语句

         编译器支持类似c语言的dowhile语句:

         ```c
         Void main(Int n) {
                   Int i;
                   i=0;
                   do {
                        print("%d",i);
                        n=n-1;
                   }  
                   while(i<n);
          }
         ```

         运行结果:![image-20210627170419930](.\img\40.png)

       - dountil语句

         编译器支持dountil语句，执行一个函数直到满足某个条件

         ```c
         Void main(Int n) {
            Int i;
            i=0;
            do{
                 print("%d",i);
                 i = i+1;
            }  
         until(i>n);
         }
         ```

         运行结果:![image-20210627170501797](.\img\41.png)

       - for语句
         编译器支持类型c语言的for循环语句

         ```c
         Void main(Int n) {
              Int i;
           i=0;
              for( i = 0 ; i < n;  i = i + 1){
               i=i+1;
                  print("%d",i);
           }
             print("%d",i);
         }
         ```

          运行结果:![image-20210627170538009](.\img\42.png)

       - forin语句

         编译器支持forin的语法，类似如下的定义方式，for  i  in  (a,b) ,如果a与b为int类型，则i将在依次赋值为a-b之间的数据，之后执行循环体：

         ```c
         Void main(Int n) {
             Int i;
          	for i in (3,7)
             {
                 print("%d",i);
             }
         }
         ```

         运行结果:![image-20210627170701116](.\img\43.png)

       - switch-case-default
         实现了类似于c语言的switch语句，当一个case语句匹配完成时，继续执行后续的语句，default关键字可以匹配全部的条件

         ```c
          Void main(Int n) {
             switch( n ){
                    case 1 :  print("%d",n);
                    case 2 :  print("%d",n+1);
                    default : print("%d",2);
                }
             print("%d",n);
          }
         ```

         运行结果:   ![image-20210627170913004](.\img\44.png)

       - 模式匹配
         本来想实现一个类似fsharp的模式匹配的，但是能力有限，仿照switchcase的模式实现了一个简单的matchPattern，只能精准匹配表达式的数值，不过match表达式满足匹配的内容后就不会继续执行后续的语句，直接结束循环:

         ```c
         Void main(Int n) {
             match n with
             | 2 -> print("%d",n);
             | 3 -> print("%d",n+1);
             | _ -> print("%d",2);
         }
         ```

          运行结果:![image-20210627171047415](.\img\45.png)

       - break语句

         解释器实现了break语句，如果循环体中出现了break语句，则结束循环：

         ```c
         Void main(Int n) {
             Int i;
             i=0;
             for( i = 0 ; i < n;  i = i + 1){
                 i=i+1;
                 print("%d",i);
                 break;
             }
            print("%d",i);
         }
         ```

         运行结果：  ![image-20210627171524041](.\img\46.png)

       - continue语句

         编译器实现了continue的逻辑，当循环体遇到continue逻辑时，跳过后续的语句，直接执行下次循环

         ```c
         Void main(Int n) {
             Int i;
             i=0; 
             for( i = 0 ; i < n;  i = i + 1){
                 i=i+1;
                 continue;
                 print("%d",i);
             }
            print("%d",i);
         }
         ```

         运行结果：
         ![image-20210627204333139](.\img\47.png)

     



​     

3. 解决技术要点说明
   - 解决解释器类型定义存储和检查， 关键代码与步骤如下：

     1.将store的存储类型定义为自定义的type结构，给与不同的数据结构不同的标识，方便后续进行管理，之后将store的存储类型设置为mem

     ```fsharp
     type mem =
       | INT of int //整数
       | STRING of string //字符串
       | POINTER of int  //地址
       | FLOAT of float //浮点
       | CHAR of char //字符
       | BOOLEAN of bool //布尔
       | STRUCT of string*int*int //结构体(名字，下标，大小)
       | ARRAY of typ*int*int //数组（类别，开始位置，大小）
       
     type store = Map<address,mem> //存储的定义
     ```

     2.之后定义成员方法，用于取值，在需要将获得的存储单元转换为指定的数据类型的时候，调用对应的成员方法即可，实现部分的自动转型，如果对应的存储单元的内容不能转换为指定的数据类型，那么将会报错，从而实现了转型和类型检查：

     ```fsharp
       member this.int = 
         match this with 
         | INT i -> i
         | POINTER i -> i
         | FLOAT f -> int f
         | CHAR c -> int c
         | BOOLEAN b -> if b then 1 else 0
         | STRUCT (s,i,size) -> i
         | ARRAY (typ , i,size) -> i
         | _ -> failwith("not int")
       
       member this.string = 
         match this with 
         | STRING s -> s
         | _ -> failwith("not string")
       
       member this.char = 
         match this with 
         | CHAR c -> c
         | INT i -> char i
         | _ -> failwith("not char")
     
       member this.float = 
         match this with 
         | FLOAT f -> f
         | INT i -> float i
         | _ -> failwith("not float")
     
       member this.boolean = 
         match this with 
         | BOOLEAN b -> b
         | _ -> failwith("not boolean")
     
       member this.pointer = 
         match this with 
         | POINTER i -> i
         | INT i -> i
         | _ -> failwith("not pointer")
     ```

     3.最后定义成员方法，返回变量的类型，对于运算时的类型检查和数组结构体的取值，计算大小等有所帮助

     ```fsharp
      member this.checktype =
         match this with 
         | INT i -> TypI
         | FLOAT f -> TypF
         | CHAR c -> TypC
         | BOOLEAN b -> TypB
         | STRING s -> TypS
         | ARRAY(typ,i,size) ->  TypA(typ,Some size)
         | STRUCT (s,i,size) -> TypeStruct s
         | _ -> failwith("error")
     ```

     4.在解释器定义一个变量时，根据定义变量的类型，为每个变量添加初始值，数组中，第二位int类型为数组的地址，方便后续进行操作，结构体中放入结构体在结构体环境中的下标值，方便获取结构体的类型

     ```fsharp
     let rec allocate (typ, x) (env0, nextloc) structEnv sto0 : locEnv * store = 
         let (nextloc1, v, sto1) =
             match typ with
             //数组 调用initSto 分配 i 个空间
             | TypA (t, Some i) -> (nextloc+i, (ARRAY(t,nextloc,i)), initSto nextloc i sto0)
             | TypA (t, None) -> (nextloc, (ARRAY(typ,nextloc,0)), sto0)
             | TypeStruct s -> let (index,arg,size) = structLookup structEnv s 0
                               (nextloc+size, (STRUCT (s,index,size)), initSto nextloc size sto0)
             | TypB   -> (nextloc,  (BOOLEAN false), sto0)
             | TypI   -> (nextloc,  (INT 0),sto0)
             | TypP i -> (nextloc,  (POINTER 0),sto0)
             | TypC   -> (nextloc,  (CHAR (char 0)),sto0)
             | TypS   -> (nextloc, (STRING ""),sto0)
             | TypF   -> (nextloc, (FLOAT 0.0),sto0)
             | _ -> (nextloc,  (INT -1), sto0)      
         bindVar x v (env0, nextloc1) sto1
     ```

     5.在eval函数中,将返回值定义为mem，使得求值函数可以返回不同类型的参数，在定义的类型检查函数中，通过checktype类型方法的调用，获得当前对象的属性，并返回字符串，实现类型的检测

     ```c
     and eval e locEnv gloEnv structEnv store : mem  * store = 
         match e with
     	|Typeof e -> let (res,s) = eval e locEnv gloEnv structEnv store
                       match res.checktype with
                       | TypB   -> (STRING "Bool",s)
                       | TypI   -> (STRING "Int",s)
                       | TypP i -> (STRING "Pointer",s)
                       | TypC   -> (STRING "Char",s)
                       | TypS   -> (STRING "String",s)
                       | TypF   -> (STRING "Float",s)
                       | TypA (typ,i) -> (STRING "Array",s)
                       | TypeStruct str  -> (STRING ("Struct "+str),s)
     ```

     6.在各类的常量中，将识别为不同常量的数值，封装为mem数据结构作为返回值

     ```c
     	| CstI i         -> (INT i, store)
         | ConstNull      -> (INT 0 ,store)
         | ConstBool b    -> (BOOLEAN b,store)
         | ConstString s  -> (STRING s,store)
         | ConstFloat f   -> (FLOAT (float f),store)
         | ConstChar c    -> (CHAR c, store)
         | Addr acc       -> let (acc1,s) = access acc locEnv gloEnv structEnv store
                             (POINTER acc1, s)
     ```

     7.在执行每个运算时，将对获得的参数的类型进行检测，如果类型不满足要求，即调用成员方法时，不能转化为对应的值，则将报错。例如：在格式化输出的函数中，需要对输出的变量求值后，获取其mem类型值对应的真实值，如果其真实值不能转化为指定的类型，那么就会报错：

     ```c
     | Print(op,e1)   -> let (i1, store1) = eval e1 locEnv gloEnv structEnv store
                             let res = 
                               match op with
                               | "%c"   -> (printf "%c " i1.char; i1)
                               | "%d"   -> (printf "%d " i1.int ; i1)  
                               | "%f"   -> (printf "%f " i1.float ;i1 )
                               | "%s"   -> (printf "%s " i1.string ;i1 )
                             (res, store1)  
     ```

     ![image-20210627125815409](.\img\26.png)

   - 解决解释器结构体定义， 关键代码与步骤如下

     1.首先在全局中定义一个结构体的用于存储每个struct的参数结构，其中包括struct的名称，参数列表和大小

     ```fsharp
     type structEnv = (string *  paramdecs * int ) list
     ```

     2.之后定义一个lookup函数来对struct进行查询

     ```fsharp
     let rec structLookup env x index=
         match env with
         | []                            -> failwith(x + " not found")
         | (name, arglist, size)::rhs    -> if x = name then (index, arglist, size) else structLookup rhs x (index+1)
     ```

     3.在clex和absyn中定义struct关键字，之后在cpar中，使用对如何解析struct进行定义:

     ```fsharp
     //在main函数之前定义struct
     Topdec: 
         Vardec SEMI                         { Vardec (fst $1, snd $1) }
       | Fundec                              { $1 }
       | VariableDeclareAndAssign SEMI       { VariableDeclareAndAssign(first $1, second $1 , third $1)  }
       | StructDec SEMI                      { Structdec(fst $1,snd $1) }
     ;
     
     //解析为结构体
     StructDec:
       | STRUCT NAME LBRACE MemberList RBRACE         { ($2, $4) }
     ;
     
     //结构体中的参数列表
     MemberList:
         /* empty */ { [] }
         | Vardec SEMI MemberList { $1 :: $3 }
     ;
     
     ```

     4.在解释器中，在每个函数中加上structEnv的结构，使得每个函数都可以访问struct结构，在执行的入门函数中，代入全局定义的structEnv.

     ```fsharp
     exec mainBody mainBodyEnv (varEnv, funEnv) structEnv store1
     ```

     5.在绑定全局变量的函数中，如果识别到struct类型，则先计算结构体参数的总大小，之后结构体将其更新到structEnv的环境中，形成一个structEnv的全局环境

     ```fsharp
     | Structdec (name,list) :: decr ->
               let rec sizeof list all = 
                 match list with
                 | [] -> all
                 | ( typ ,string ):: tail -> sizeof tail ((allsize typ) + all)
               let fin = sizeof list 0
               addv decr locEnv funEnv ((name,list, fin) :: structEnv) store
     ```

     6.在定义一个结构体的实体类时，使用structlookup函数查询结构体在structEnv中的位置，并将其放入STRUCT数据中，在调用时，可以快速获取结构体的位置，之后再存储中预留结构体大小的位置，为后续结构体元素赋值进行准备

     ```fsharp
     let rec allocate (typ, x) (env0, nextloc) structEnv sto0 : locEnv * store = 
         let (nextloc1, v, sto1) =
             match typ with
             | TypeStruct s -> let (index,arg,size) = structLookup structEnv s 0
                               (nextloc+size, (STRUCT (s,index,size)), initSto nextloc size sto0)
                  
         bindVar x v (env0, nextloc1) sto1
     ```

     7.当调用一个结构体中一个元素的值时，首先通过结构体的名字获得结构体的位置，此处存储的类型是STRUCT(string,int,int),第二个int类型的参数为结构体在结构体环境中的第几个，从而可以快速定位到结构体的位置，之后，将结构体的调用的元素的名称在结构体的参数列表中进行搜寻，在递归时同时计算出该参数的偏移值。最后将STRUCT的入门存储位置减去结构体的大小算出结构体第一个参数的位置，加上计算出的偏移值，获得当前的元素在store的位置（地址）作为返回值。

     ```fsharp
     | AccStruct(acc,acc2) ->  let (b, store1) = access acc locEnv gloEnv structEnv store
                                   let aval = getSto store1 b
                                   let list = structEnv.[aval.int]
                                   let param =
                                       match list with 
                                       | (string,paramdecs,int) -> paramdecs
                                   let sizestruct =
                                       match list with 
                                       | (string,paramdecs,i) -> i
                                   let a = b - sizestruct;
                                   let rec lookupidx list index = 
                                       match list with
                                       | [] -> failwith("can not find ")
                                       | (typ , name ) ::tail -> match acc2 with
                                                                 | AccVar x -> if x = name then index 
                                                                                           else lookupidx tail ( index + ( allsize typ) )
                                                                 | AccIndex( acc3, idx ) ->  match acc3 with
                                                                                             | AccVar y ->  if name = y then 
                                                                                                               let size = 
                                                                                                                 match typ with
                                                                                                                 | TypA(typ,Some i) -> i
                                                                                                                 | TypA(typ,None) -> 0
                                                                                                               let (i, store2) = eval idx locEnv gloEnv structEnv store1
                                                                                                               if(i.int>=size) then  failwith( " index out of size" )
                                                                                                               elif(i.int<0) then failwith( " index out of size" )
                                                                                                                             else (index + i.int)
                                                                                                            else lookupidx tail (index + (allsize typ))
     
     ```

     

     

4. 心得体会（结合自己情况具体说明）
   - 大项目开发过程心得

     - 张帅：这学期老师很早就给了我们MICROC的代码，也很早就告诉我们应该开始研究了大作业的内容了，奈何之前感觉应付编译原理的相关作业和理论知识就很困难了，在没有掌握相关的理论知识的情况下，也很难进行大作业的编写，所以直到6月7号才算是正式开始大作业的编程，刚开始因为F#的基础不够牢固，感觉举步维艰，很难读清楚也很难进行大作业的编程，被迫在尝试一段时间后，恶补了F#的语法，之后在模仿MICORC的基础上，开始完善解释器，实现了一些简单的诸如dowhile，for等函数的，也犯了很多的失误等。之后在尝试实现string时，发现store的存储结构居然只支持int，int类型的局限很大程度限制了多样化的数据类型，开始我尝试通过实现类似char数组的结构来实现string，但是这样的话只能为string先分配一片固定的空间，定义string时并不知道string有几位，在实现float时，我甚至尝试了使用IEEE754标准来存储它，后来在实现查看老师的教案时，发现可以将返回值定义为自己的数据结构，而不是局限于int，所以跟附近的同学进行了讨论和研究，最终实现了自己定义数据结构的存储类型，多样化的存储类型有效帮助实现完善了后续的功能，包括类型检查，类型转化和初始化定义等。最后我发现自己实现的内容都比较低级，自己想尝试一些比较高级的特性，在尝试其他无果后，我在参考老师给我优秀作业的Cbuy的情况下，想实现一个解释器的oo（Cuby实现了编译器的结构体），我参考它的逻辑，定义全局的struct环境，优化的存储等，最后实现了解释器的结构体的定义，在我想实现oo的函数定义时，我发现自己能力实在有限，也没有足够的时间了，值得放弃，转而开始研究编译器。

       在开发过程中，我更加深刻认识到了左值右值的区别，对于语法树和环境与闭包有了具象的认识，之前都是在课堂上看一些理论的东西，实际操作研究的也很少，最后在一点一点自己摸索的过程中，理解更加深刻了；我也在开发过程中认识到了老师喜欢F#语言的原因，F#语言对于类型定义的灵活性，模式匹配的灵活性，都是我接触的其他语言无可比拟的，同时fslex和fsyacc这两个语法词法解析器也着实好用有效，我很难想象不依靠他们我应该怎么完成我的作业。

       最后，在开发中才认识到自己对于知识的缺失和掌握的不足，虽然花了将近三个礼拜的时间，但是完成的内容多数的比较低级的语法词法，真的称得上还可以的只有类型检查和结构体，但是也已经是黔驴技穷了，如果还有时间我还是希望可以实现更多的内容，真的是越编程越觉得写一门语言的有趣，很可惜大作业只能完成这些内容。

     - 沈文燕：

       ​		本学期学习了《编译语言原理与编译》这门课，对编译的本质和F#这门语言有了更具深刻的理解。学习了词法分析（将输入分解成一个个独立的词法符号，即“单词符号”，简称单词），语法分析（分析程序的短语结构），语义分析（推算程序含义）的概念以及分别是怎么实现的。对编译的过程有了比较清晰的了解。

       ​		由于大作业的主要代码是F#语言，所以对于大作业的理解和完成还是有一定的困难程度的。通过F#语言的学习和应用，对编程语言也有了更加广阔的认识。通过与C语言、Java语言等横向对比，然后对编程有了比较简单的分类概念。F#属于函数式编程语言、C语言属于结构化编程语言、Java属于面向对象编程语言。对于函数式编程语言来说，它具有并行、单元测试、没有额外作用，不修改状态、引用透明、代码部署热等特点。当然对于本学期，课程来说，只是对函数式编程语言进行了简单的了解，以及语法运用。老师上课，还补充了包括垃圾回收、续算等比较高级的特性。给了大家很大的学习空间。
   - 本课程建议
     - 张帅：
     
       **课程难度**上的话，我认为这门课程是大学四年中最困难的课程了，之前完全没有接触过相关内容，而且一开始就需要我们对ocmal和fsharp语言进行学习，让我感觉非常吃力，每周应付作业多需要花上很多的时间，大作业更是花了大量的功夫进行编程，让我感觉到了自己对于计算机了解的有限和能力的局限性，产生了非常大的自我怀疑；
     
       **课程进度**上的话，感觉郭老师的班级，郭老师希望我们可以了解学习更多的内容，会比张老师班快很多，内容确实也是比较丰富，郭老师也很希望我们可以在课外自己学习一些相关内容，但我个人来说，消化课堂上的内容在课外已经花费了很多的时间了，还是非常吃力的；
     
       **建议上**的话我希望老师可以适当降低一下进度，讲一下作业习题等，给大部分同学一些缓冲的时间，我个人来说这样的进度还是很吃力的；同时我希望老师实验课可以带着大家一起做一些实验，或者讲解一些大作业的代码，最好有一节课可以带领大家理解如何修改大作业的代码，将自己的功能添加进程序中，感觉周围很多同学都是在期末无从下手，读懂研究就花费了很多的时间。
     
     - 沈文燕:
     
       （1）老师上课讲述的东西过于泛化，就是有时候老师讲是一回事，做作业又是另一回事。感觉上课听讲得到的收获会少很多。更多的时候是课后通过网课来针对性地学习。
     
       （2）资料很多，无法区分精读和泛读，很能理解老师想要我们学习很多的心情。但是，对于大家来说平时的时间还是有比较有限制，因为还是在大三下的课程，并且原来的理论基础并不是很好，跟上课程会有些许困难。有时候有心想读，但是由于资料冗杂，不能做合理区分。
     
       （3）希望老师能对一些作业进行一些答案的提供。有时候像DFA等等作业，无法知道我做完了是否正确，也无法找到自己思维逻辑的错误之处。当然老师提供的工具也很好用。但是有些答案无非通过工具获取。
     
       （4）由于这门课已经变成了必选课，希望老师能照顾一下大部分人的过课率。讲解更加简单通俗易懂。
     
       