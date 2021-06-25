import  Type.*;
import exception.ImcompatibleTypeError;
import exception.OperatorError;

import java.io.*;
import java.util.ArrayList;
import java.util.regex.Pattern;

public class Machine {
    private final static int STACKSIZE = 1000;

    public static void main(String[] args) throws FileNotFoundException, IOException, OperatorError, ImcompatibleTypeError {
        if (args.length == 0)
            System.out.println("Usage: java Machine <programfile> <arg1> ...\n");
        else {
            execute(args, false);
        }

//        args = new String[1];
//        args[0] = "D:\\Yuby\\ \\testing\\ex(float).out";
////        args[1] = "Hello";
////        args[2] = "1.2";
////        args[3] = "100";
////        args[4] = "123Hel";
//        execute(args, false);
    }


    static void execute(String[] args, boolean trace) throws FileNotFoundException, IOException, OperatorError, ImcompatibleTypeError {
        ArrayList<Integer> program = readfile(args[0]);

         BaseType[] stack = new  BaseType[STACKSIZE];

         BaseType[] inputArgs = new  BaseType[args.length - 1];

        for (int i = 1; i < args.length; i++) {
            if(Pattern.compile("(?i)[a-z]").matcher(args[i]).find()){
                char[] input = args[i].toCharArray();
                 CharType[] array = new  CharType[input.length];
                for(int j = 0; j < input.length; ++j) {
                    array[j] = new  CharType(input[j]);
                }
                inputArgs[i-1] = new  ArrayType(array);
            }
            else if(args[i].contains(".")){
                inputArgs[i-1] = new  FloatType(new Float(args[i]).floatValue());
            }
            else {
                inputArgs[i-1] = new  IntType(new Integer(args[i]).intValue());
            }
        }


//        for(int i = 0; i < inputArgs.length; ++i){
//            if(inputArgs[i] instanceof  ArrayType){
//                 BaseType[] a = (( ArrayType)inputArgs[i]).getValue();
//                for(int j = 0; j < a.length; ++j){
//                    if(a[j] instanceof  CharType){
//                        System.out.print((( CharType)a[j]).getValue());
//                    }
//                    else if(a[j] instanceof  IntType){
//                        System.out.print((( IntType)a[j]).getValue());
//                    }
//                    else if(a[j] instanceof  FloatType){
//                        System.out.print((( FloatType)a[j]).getValue());
//                    }
//                }
//                System.out.println();
//            }
//            else if(inputArgs[i] instanceof  CharType){
//                System.out.println((( CharType)inputArgs[i]).getValue());
//            }
//            else if(inputArgs[i] instanceof  IntType){
//                System.out.println((( IntType)inputArgs[i]).getValue());
//            }
//            else if(inputArgs[i] instanceof  FloatType){
//                System.out.println((( FloatType)inputArgs[i]).getValue());
//            }
//
//        }

        long startTime = System.currentTimeMillis();
        execCode(program, stack, inputArgs, trace);
        long runtime = System.currentTimeMillis() - startTime;
        System.err.println("\nRan " + runtime/1000.0 + " seconds");
    }


    private static int execCode(ArrayList<Integer> program,  BaseType[] stack,  BaseType[] inputArgs, boolean trace) throws ImcompatibleTypeError, OperatorError {
        int bp = -999;
        int sp = -1;
        int pc = 0;
        int hr = -1;
        for (;;) {
            if (trace)
                printSpPc(stack, bp, sp, program, pc);
            switch (program.get(pc++)) {
                case Instruction.CSTI:
                    stack[sp + 1] = new  IntType(program.get(pc++)); sp++; break;
                case Instruction.CSTF:
                    stack[sp + 1] = new  FloatType(Float.intBitsToFloat(program.get(pc++))); sp++; break;
                case Instruction.CSTC:
                    stack[sp + 1] = new  CharType((char)(program.get(pc++).intValue())); sp++; break;
                case Instruction.ADD: {
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "+");
                    sp--;
                    break;
                }
                case Instruction.SUB:{
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "-");
                    sp--;
                    break;
                }

                case Instruction.MUL: {
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "*");
                    sp--;
                    break;
                }
                case Instruction.DIV:
                    if((( IntType)stack[sp]).getValue()==0)
                    {
                        System.out.println("hr:"+hr+" exception:"+1);
                        while (hr != -1 && (( IntType)stack[hr]).getValue() != 1 )
                        {
                            hr = (( IntType)stack[hr+2]).getValue();
                            System.out.println("hr:"+hr+" exception:"+new  IntType(program.get(pc)).getValue());
                        }
                            
                        if (hr != -1) { 
                            sp = hr-1;    
                            pc = (( IntType)stack[hr+1]).getValue();
                            hr = (( IntType)stack[hr+2]).getValue();    
                        } else {
                            System.out.print(hr+"not find exception");
                            return sp;
                        }
                    }
                    else{
                        stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "/");
                        sp--; 
                    }
                    
                    break;
                case Instruction.MOD:
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "%");
                    sp--;
                    break;
                case Instruction.EQ:
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "==");
                    sp--;
                    break;
                case Instruction.LT:
                    stack[sp - 1] = binaryOperator(stack[sp-1], stack[sp], "<");
                    sp--;
                    break;
                case Instruction.NOT: {
                    Object result = null;
                    if(stack[sp] instanceof  FloatType){
                        result = (( FloatType)stack[sp]).getValue();
                    }else if (stack[sp] instanceof  IntType){
                        result = (( IntType)stack[sp]).getValue();
                    }
                    stack[sp] = (Float.compare(new Float(result.toString()), 0.0f) == 0 ? new  IntType(1) : new  IntType(0));
                    break;
                }
                case Instruction.DUP:
                    stack[sp+1] = stack[sp];
                    sp++;
                    break;
                case Instruction.SWAP: {
                     BaseType tmp = stack[sp];  stack[sp] = stack[sp-1];  stack[sp-1] = tmp;
                    break;
                }
                case Instruction.LDI:
                    stack[sp] = stack[(( IntType)stack[sp]).getValue()]; break;
                case Instruction.STI:
                    stack[(( IntType)stack[sp-1]).getValue()] = stack[sp]; stack[sp-1] = stack[sp]; sp--; break;
                case Instruction.GETBP:
                    stack[sp+1] = new  IntType(bp); sp++; break;
                case Instruction.GETSP:
                    stack[sp+1] = new  IntType(sp); sp++; break;
                case Instruction.INCSP:
                    sp = sp + program.get(pc++); break;
                case Instruction.GOTO:
                    pc = program.get(pc); break;
                case Instruction.IFZERO: {
                    Object result = null;
                    int index = sp--;
                    if(stack[index] instanceof  IntType){
                        result = (( IntType)stack[index]).getValue();
                    }else if(stack[index] instanceof  FloatType){
                        result = (( FloatType)stack[index]).getValue();
                    }
                    pc = (Float.compare(new Float(result.toString()), 0.0f) == 0 ? program.get(pc) : pc + 1);
                    break;
                }
                case Instruction.IFNZRO: {
                    Object result = null;
                    int index = sp--;
                    if (stack[index] instanceof  IntType) {
                        result = (( IntType) stack[index]).getValue();
                    } else if (stack[index] instanceof  FloatType) {
                        result = (( FloatType) stack[index]).getValue();
                    }
                    pc = (Float.compare(new Float(result.toString()), 0.0f) != 0 ? program.get(pc) : pc + 1);
                    break;
                }
                case Instruction.CALL: {
                    int argc = program.get(pc++);
                    for (int i=0; i<argc; i++)
                        stack[sp-i+2] = stack[sp-i];
                    stack[sp-argc+1] = new  IntType(pc+1); sp++;
                    stack[sp-argc+1] = new  IntType(bp);   sp++;
                    bp = sp+1-argc;
                    pc = program.get(pc);
                    break;
                }
                case Instruction.TCALL: {
                    int argc = program.get(pc++);
                    int pop  = program.get(pc++);
                    for (int i=argc-1; i>=0; i--)
                        stack[sp-i-pop] = stack[sp-i];
                    sp = sp - pop; pc = program.get(pc);
                } break;
                case Instruction.RET: {
                    BaseType res = stack[sp];
                    sp = sp - program.get(pc); bp = (( IntType)stack[--sp]).getValue(); pc = (( IntType)stack[--sp]).getValue();
                    stack[sp] = res;
                } break;
                case Instruction.PRINTI: {
                    Object result;
                    if(stack[sp] instanceof IntType){
                        result = ((IntType)stack[sp]).getValue();
                    }else if(stack[sp] instanceof FloatType){
                        result = ((FloatType)stack[sp]).getValue();
                    }else {
                        result = ((CharType)stack[sp]).getValue();
                    }

                    System.out.print(String.valueOf(result) + " ");
                    break;
                }
                case Instruction.PRINTC:
                    System.out.print((((CharType)stack[sp])).getValue()); break;
                case Instruction.PRINTF:
                    System.out.print((((FloatType)stack[sp])).getValue()); break;
                case Instruction.LDARGS:
                    for (int i=0; i < inputArgs.length; i++) // Push commandline arguments
                        stack[++sp] = inputArgs[i];
                    break;
                case Instruction.STOP:
                    return sp;
                case Instruction.PUSHHR:{
                    stack[++sp] = new IntType(program.get(pc++));    //exn
                    int tmp = sp;       //exn address
                    sp++;
                    stack[sp++] = new IntType(program.get(pc++));   //jump address
                    stack[sp] = new IntType(hr);
                    hr = tmp;
                    break;
                }
                case Instruction.POPHR:
                    hr = (( IntType)stack[sp--]).getValue();sp-=2;break;
                case Instruction.THROW:
                    System.out.println("hr:"+hr+" exception:"+new  IntType(program.get(pc)).getValue());

                    while (hr != -1 && (( IntType)stack[hr]).getValue() != program.get(pc) )
                    {
                        hr = (( IntType)stack[hr+2]).getValue(); //find exn address
                        System.out.println("hr:"+hr+" exception:"+new  IntType(program.get(pc)).getValue());
                    }
                        
                    if (hr != -1) { // Found a handler for exn
                        sp = hr-1;    // remove stack after hr
                        pc = (( IntType)stack[hr+1]).getValue();
                        hr = (( IntType)stack[hr+2]).getValue(); // with current handler being hr     
                    } else {
                        System.out.print(hr+"not find exception");
                        return sp;
                    }break;

                default:
                    throw new RuntimeException("Illegal instruction " + program.get(pc-1)
                            + " at address " + (pc-1));

            }


        }


    }

    public static  BaseType binaryOperator( BaseType lhs,  BaseType rhs, String operator) throws ImcompatibleTypeError, OperatorError {
        Object left;
        Object right;
        int flag = 0;
        if (lhs instanceof  FloatType) {
            left = (( FloatType) lhs).getValue();
            flag = 1;
        } else if (lhs instanceof  IntType) {
            left = (( IntType) lhs).getValue();
        } else {
            throw new ImcompatibleTypeError("ImcompatibleTypeError: Left type is not int or float");
        }

        if (rhs instanceof  FloatType) {
            right = (( FloatType) rhs).getValue();
            flag = 1;
        } else if (rhs instanceof  IntType) {
            right = (( IntType) rhs).getValue();
        } else {
            throw new ImcompatibleTypeError("ImcompatibleTypeError: Right type is not int or float");
        }
         BaseType result = null;

        switch(operator){
            case "+":{
                if (flag == 1) {
                    result =  new  FloatType(Float.parseFloat(String.valueOf(left)) + Float.parseFloat(String.valueOf(right)));
                } else {
                    result = new  IntType(Integer.parseInt(String.valueOf(left)) + Integer.parseInt(String.valueOf(right)));
                }
                break;
            }
            case "-":{
                if (flag == 1) {
                    result = new  FloatType(Float.parseFloat(String.valueOf(left)) - Float.parseFloat(String.valueOf(right)));
                } else {
                    result = new  IntType(Integer.parseInt(String.valueOf(left)) - Integer.parseInt(String.valueOf(right)));
                }
                break;
            }
            case "*":{
                if (flag == 1) {
                    result = new  FloatType(Float.parseFloat(String.valueOf(left)) * Float.parseFloat(String.valueOf(right)));
                } else {
                    result = new  IntType(Integer.parseInt(String.valueOf(left)) * Integer.parseInt(String.valueOf(right)));
                }
                break;
            }
            case "/":{
                if(Float.compare(Float.parseFloat(String.valueOf(right)), 0.0f) == 0){
                    throw new OperatorError("OpeatorError: Divisor can't not be zero");
                }
                if (flag == 1) {
                    result = new  FloatType(Float.parseFloat(String.valueOf(left)) / Float.parseFloat(String.valueOf(right)));
                } else {
                    result = new  IntType(Integer.parseInt(String.valueOf(left)) / Integer.parseInt(String.valueOf(right)));
                }
                break;
            }
            case "%":{
                if (flag == 1) {
                    throw new OperatorError("OpeatorError: Float can't mod");
                } else {
                    result = new  IntType(Integer.parseInt(String.valueOf(left)) % Integer.parseInt(String.valueOf(right)));
                }
                break;
            }
            case "==":{
                if (flag == 1) {
                    if((float) left == (float) right){
                        result = new  IntType(1);
                    }
                    else{
                        result = new  IntType(0);
                    }
                } else {
                    if((int) left == (int) right){
                        result = new  IntType(1);
                    }
                    else{
                        result = new  IntType(0);
                    }
                }
                break;
            }
            case "<":{
                if (flag == 1) {
                    if((float) left < (float) right){
                        result = new  IntType(1);
                    }
                    else{
                        result = new  IntType(0);
                    }
                } else {
                    if((int) left < (int) right){
                        result = new  IntType(1);
                    }
                    else{
                        result = new  IntType(0);
                    }
                }
                break;
            }
        }
        return result;
    }


    private static String insName(ArrayList<Integer> program, int pc) {
        switch (program.get(pc)) {
            case Instruction.CSTI:   return "CSTI " + program.get(pc+1);
            case Instruction.CSTF:   return "CSTF " + program.get(pc+1);
            case Instruction.CSTC:   return "CSTC " + (char)(program.get(pc+1).intValue());
            case Instruction.ADD:    return "ADD";
            case Instruction.SUB:    return "SUB";
            case Instruction.MUL:    return "MUL";
            case Instruction.DIV:    return "DIV";
            case Instruction.MOD:    return "MOD";
            case Instruction.EQ:     return "EQ";
            case Instruction.LT:     return "LT";
            case Instruction.NOT:    return "NOT";
            case Instruction.DUP:    return "DUP";
            case Instruction.SWAP:   return "SWAP";
            case Instruction.LDI:    return "LDI";
            case Instruction.STI:    return "STI";
            case Instruction.GETBP:  return "GETBP";
            case Instruction.GETSP:  return "GETSP";
            case Instruction.INCSP:  return "INCSP " + program.get(pc+1);
            case Instruction.GOTO:   return "GOTO " + program.get(pc+1);
            case Instruction.IFZERO: return "IFZERO " + program.get(pc+1);
            case Instruction.IFNZRO: return "IFNZRO " + program.get(pc+1);
            case Instruction.CALL:   return "CALL " + program.get(pc+1) + " " + program.get(pc+2);
            case Instruction.TCALL:  return "TCALL " + program.get(pc+1) + " " + program.get(pc+2) + " " +program.get(pc+3);
            case Instruction.RET:    return "RET " + program.get(pc+1);
            case Instruction.PRINTI: return "PRINTI";
            case Instruction.PRINTC: return "PRINTC";
            case Instruction.LDARGS: return "LDARGS";
            case Instruction.STOP:   return "STOP";
            case Instruction.THROW:  return "THROW" + program.get(pc+1);
            case Instruction.PUSHHR: return "PUSHHR" + " " + program.get(pc+ 1) + " " + program.get(pc+2) ;
            case Instruction.POPHR: return "POPHR";
            default:     return "<unknown>";
        }
    }


    private static void printSpPc( BaseType[] stack, int bp, int sp, ArrayList<Integer> program, int pc) {
        System.out.print("[ ");
        for (int i = 0; i <= sp; i++) {
            Object result = null;
            if(stack[i] instanceof  IntType){
                result = (( IntType)stack[i]).getValue();
            }else if(stack[i] instanceof  FloatType){
                result = (( FloatType)stack[i]).getValue();
            }else if(stack[i] instanceof  CharType){
                result = (( CharType)stack[i]).getValue();
            }
            System.out.print(String.valueOf(result) + " ");
        }
        System.out.print("]");
        System.out.println("{" + pc + ": " + insName(program, pc) + "}");
    }


    private static ArrayList<Integer> readfile(String filename) throws FileNotFoundException, IOException {
        ArrayList<Integer> program = new ArrayList<Integer>();
        Reader inp = new FileReader(filename);

        StreamTokenizer tStream = new StreamTokenizer(inp);
        tStream.parseNumbers();
        tStream.nextToken();
        while (tStream.ttype == StreamTokenizer.TT_NUMBER) {
            program.add(new Integer((int)tStream.nval));
            tStream.nextToken();
        }

        inp.close();

        return program;
    }
}


class Machinetrace {
    public static void main(String[] args)
            throws FileNotFoundException, IOException, OperatorError, ImcompatibleTypeError {
        if (args.length == 0)
            System.out.println("Usage: java Machinetrace <programfile> <arg1> ...\n");
        else
            Machine.execute(args, true);
    }
}
