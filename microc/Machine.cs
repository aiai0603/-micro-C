/* File MicroC/Machine.cs
   A unified-stack abstract machine for imperative programs.

   To execute a program file using this abstract machine, do:

      Machine.exe <programfile> <arg1> <arg2> ...

   or, to get a trace of the program execution:

      Machinetrace <programfile> <arg1> <arg2> ...

*/
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class Machine
{
    static void Main(string[] args)
    {
        // foreach (var arg in args) {
        // Console.WriteLine(arg);
        // }
        List<string> arglist = new List<string>(args);

        if (args.Length == 0)
            System.Console.WriteLine("Usage: Machine.exe <programfile> <arg1> ...\n");
        else if (arglist.Contains("-t"))
        {
            arglist.Remove("-t");
            execute(arglist.ToArray(), true);
        }
        else
            execute(arglist.ToArray(), false);
    }

    // These numeric instruction codes must agree with Machine.fs:

    const int
      CSTI = 0, ADD = 1, SUB = 2, MUL = 3, DIV = 4, MOD = 5, //基本运算 
      EQ = 6, LT = 7, NOT = 8,  //关系运算
      DUP = 9, SWAP = 10,   //栈操作
      LDI = 11, STI = 12,   // 栈变量
      GETBP = 13, GETSP = 14, INCSP = 15, //栈帧管理
      GOTO = 16, IFZERO = 17, IFNZRO = 18, CALL = 19, TCALL = 20, RET = 21,//分支，过程调用 
      PRINTI = 22, PRINTC = 23,  //库函数
      LDARGS = 24,  //参数
      STOP = 25;  //停机

    const int STACKSIZE = 10000;

    // Read code from file and execute it

    static void execute(string[] arglist, bool trace)
    {
        int[] p = readfile(arglist[0]);                // Read the program from file
        int[] s = new int[STACKSIZE];               // The evaluation stack
        int[] iargs = new int[arglist.Length - 1];
        for (int i = 1; i < arglist.Length; i++)           // Push commandline arguments
            iargs[i - 1] = Int32.Parse(arglist[i]);

        long starttime = DateTime.Now.Millisecond;
        execcode(p, s, iargs, trace);            // Execute program proper
        long runtime = DateTime.Now.Millisecond - starttime;
        Console.WriteLine("\nRan " + runtime / 1000.0 + " seconds");
    }

    // The machine: execute the code starting at p[pc] 
    //  p: 程序
    //  s: 运行栈
    //  iargs: 参数
    //  trace: 调试标志
    static int execcode(int[] p, int[] s, int[] iargs, bool trace)
    {
        int bp = -999;  // Base pointer, for local variable access 
        int sp = -1;    // Stack top pointer
        int pc = 0;     // Program counter: next instruction
        for (; ; )
        {
            if (trace)
                printsppc(s, bp, sp, p, pc);
            switch (p[pc++])
            {
                case CSTI:
                    s[sp + 1] = p[pc++]; sp++; break;
                case ADD:
                    s[sp - 1] = s[sp - 1] + s[sp]; sp--; break;
                case SUB:
                    s[sp - 1] = s[sp - 1] - s[sp]; sp--; break;
                case MUL:
                    s[sp - 1] = s[sp - 1] * s[sp]; sp--; break;
                case DIV:
                    s[sp - 1] = s[sp - 1] / s[sp]; sp--; break;
                case MOD:
                    s[sp - 1] = s[sp - 1] % s[sp]; sp--; break;
                case EQ:
                    s[sp - 1] = (s[sp - 1] == s[sp] ? 1 : 0); sp--; break;
                case LT:
                    s[sp - 1] = (s[sp - 1] < s[sp] ? 1 : 0); sp--; break;
                case NOT:
                    s[sp] = (s[sp] == 0 ? 1 : 0); break;
                case DUP:
                    s[sp + 1] = s[sp]; sp++; break;
                case SWAP:
                    { int tmp = s[sp]; s[sp] = s[sp - 1]; s[sp - 1] = tmp; }
                    break;
                case LDI:                    // load indirect
                                             //根据栈顶的偏移量，从栈上取值
                    s[sp] = s[s[sp]]; break;   // s,i==> s,s(i)
                case STI:                    // store indirect, keep value on top
                                             // 根据栈顶的偏移量与值，将值放入栈
                    s[s[sp - 1]] = s[sp]; s[sp - 1] = s[sp]; sp--; break;  //s,i,v ==> s,v; s(i) = v
                case GETBP:                //栈基址指针
                    s[sp + 1] = bp; sp++; break;
                case GETSP:               //栈指针
                    s[sp + 1] = sp; sp++; break;
                case INCSP:
                    sp = sp + p[pc++]; break;
                case GOTO:
                    pc = p[pc]; break;
                case IFZERO:
                    pc = (s[sp--] == 0 ? p[pc] : pc + 1); break;
                case IFNZRO:
                    pc = (s[sp--] != 0 ? p[pc] : pc + 1); break;
                case CALL:
                    {
                        int argc = p[pc++];
                        for (int i = 0; i < argc; i++)     // Make room for return address
                            s[sp - i + 2] = s[sp - i];     // and old base pointer
                        s[sp - argc + 1] = pc + 1; sp++;   // 返回地址 r
                        s[sp - argc + 1] = bp; sp++;      //旧的 bp
                        bp = sp + 1 - argc;              //新的 bp
                        pc = p[pc];                     //更新 pc ,转移到新地址
                    }
                    break;
                case TCALL:
                    {
                        int argc = p[pc++];                // Number of new arguments
                        int pop = p[pc++];         // Number of variables to discard
                        for (int i = argc - 1; i >= 0; i--)    // Discard variables
                            s[sp - i - pop] = s[sp - i];
                        sp = sp - pop; pc = p[pc];
                    }
                    break;
                case RET:
                    {
                        int res = s[sp];
                        sp = sp - p[pc]; bp = s[--sp]; pc = s[--sp];
                        s[sp] = res;
                    }
                    break;
                case PRINTI:
                    System.Console.Write(s[sp] + " "); break;
                case PRINTC:
                    System.Console.Write((char)(s[sp])); break;
                case LDARGS:
                    for (int i = 0; i < iargs.Length; i++) // Push commandline arguments
                        s[++sp] = iargs[i];
                    break;
                case STOP:
                    return sp;
                default:
                    throw new Exception("Illegal instruction " + p[pc - 1]
                                               + " at address " + (pc - 1));
            }
        }
    }

    // Print the stack machine instruction at p[pc]

    static string insname(int[] p, int pc)
    {
        switch (p[pc])
        {
            case CSTI: return "CSTI " + p[pc + 1];
            case ADD: return "ADD";
            case SUB: return "SUB";
            case MUL: return "MUL";
            case DIV: return "DIV";
            case MOD: return "MOD";
            case EQ: return "EQ";
            case LT: return "LT";
            case NOT: return "NOT";
            case DUP: return "DUP";
            case SWAP: return "SWAP";
            case LDI: return "LDI";
            case STI: return "STI";
            case GETBP: return "GETBP";
            case GETSP: return "GETSP";
            case INCSP: return "INCSP " + p[pc + 1];
            case GOTO: return "GOTO " + p[pc + 1];
            case IFZERO: return "IFZERO " + p[pc + 1];
            case IFNZRO: return "IFNZRO " + p[pc + 1];
            case CALL: return "CALL " + p[pc + 1] + " " + p[pc + 2];
            case TCALL: return "TCALL " + p[pc + 1] + " " + p[pc + 2] + " " + p[pc + 3];
            case RET: return "RET " + p[pc + 1];
            case PRINTI: return "PRINTI";
            case PRINTC: return "PRINTC";
            case LDARGS: return "LDARGS";
            case STOP: return "STOP";
            default: return "<unknown>";
        }
    }

    // Print current stack and current instruction

    static void printsppc(int[] s, int bp, int sp, int[] p, int pc)
    {
        System.Console.Write("[ ");
        for (int i = 0; i <= sp; i++)
            System.Console.Write(s[i] + " ");
        System.Console.Write("]");
        System.Console.WriteLine("{" + pc + ": " + insname(p, pc) + "}");
    }

    // Read instructions from a file

    public static int[] readfile(string filename)
    {
        List<Int32> rawprogram = new List<Int32>();
        StreamReader sr = new StreamReader(filename);

        string[] ins = Regex.Split(sr.ReadToEnd(), "\\s+");
        foreach (string item in ins)
        {
            rawprogram.Add(Int16.Parse(item));
        }



        return rawprogram.ToArray();
    }

}

