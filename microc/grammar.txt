About the grammar for micro-C * sestoft@itu.dk * 2009-09-29
-----------------------------------------------------------

Micro-C is a sublanguage of C.  By design, a syntactically well-formed
micro-C program is also a syntactically well-formed C program, and the
intention is that the meaning of the program should be the same in
both languages (except that micro-C is untyped and permits assignment
to array variables, which C does not).

Many simplifications have been made compared to real C:

     * datatypes: only int and char variables, arrays, and pointers
     * no structs, unions, doubles, function pointers, ...
     * no initializers in variable declarations
     * functions can return only int, char, void


First attempt at a grammar
--------------------------

This first version of the grammar shows what we want the language to
include.  However, it is ambiguous and will have to be rewritten for
use in a parser specification (see below).  Some grammar rules are
already complex enough to deserve an explanation:

 * The variable description vardesc reflects the variable declaration
   syntax of C and C++, where the type specification surrounds the
   variable; unlike in Standard ML, F#, Java and C#, the type cannot
   be isolated syntactically from the variable name.

 * The definition of a comma-separated list (of variables or argument
   expressions) must be split into three cases:
   
      empty list
      one element (and no comma)
      more than one element (separated by commas)
  
   This is best achieved using two nonterminal symbols, as
   illustrated by comma-separated argument expression lists:

      exprs ::=
              <empty>
              expr1

      expr1 ::=
              expr
              expr , expr1

   The non-recursive nonterminal exprs distinguishes the empty case
   from the non-empty case.  The recursive nonterminal expr1
   distinguishes the one-element case from the multi-element case.

 * We distinguish between access expressions (those that have an
   lvalue), and general expressions.  The address operator (&) can be
   applied only to access expressions.  In an array element access
   e1[e2], expression e1 must be an access expression.


main ::=                                program
        topdecs EOF

topdecs ::=                             top-level declarations
        <empty>
        topdec topdecs

topdec ::=                              top-level declaration
        vardec ;                        global variable declaration
        fundec                          function declaration

vardec ::=                              variable or parameter declaration
        typ vardesc

vardesc ::=                             variable description
        NAME                            variable
        * vardesc                       pointer to 
        ( vardesc )                     parenthesized variable description
        vardesc [ ]                     array of
        vardesc [ int ]                 array of given size

fundec ::=                              function declaration
        void NAME ( paramdecs ) block
        typ  NAME ( paramdecs ) block

paramdecs ::=                           comma-separated parameter list
        <empty>
        paramdecs1

paramdecs1 ::=                          non-empty parameter declaration list
        vardec
        vardec , paramdecs1

stmt ::= 
        if (expr) stmt                  if-statement
        if (expr) stmt else stmt        if-else statement
        while (expr) stmt               while-loop
        expr ;                          expression as statement
        return ;                        return
        return expr ;                   return
        block                           block statement
        
block ::=                               block statement
        { stmtordecseq }

stmtordecseq ::=                        statements and declarations
        <empty>                         empty sequence
        stmt stmtordecseq               statement
        vardec ; stmtordecseq           local variable declaration

expr ::=
        access                          access
        access = expr                   assignment
        const                           constant literal
        NAME ( exprs )                  function call
        ( expr )                        parenthesized expression
        & access                        address of
        ! expr                          logical negation
        print expr                      print integer expression
        expr +  expr                    plus
        expr -  expr                    minus
        expr *  expr                    times
        expr /  expr                    quotient
        expr %  expr                    remainder
        expr == expr                    equal to
        expr != expr                    not equal to
        expr >  expr                    greater than
        expr <  expr                    less than
        expr >= expr                    greater than or equal to
        expr <= expr                    less than or equal to
        expr && expr                    sequential and
        expr || expr                    sequential or

access ::=
        NAME                            local or global variable
        * expr                          pointer dereferencing
        access [ expr ]                 array indexing

exprs ::=
        <empty>                         empty list of expressions
        exprs1                          non-empty list of expressions

exprs1 ::=
        expr                            list with one expression
        expr , exprs1                   list with more than one expr

const ::=                               constant literals
        CSTINT                          integer literal
        - CSTINT                        negative integer
        NULL                            the NULL literal

typ ::=
        int                             integer type
        char                            character type


Second attempt at a grammar, more suitable for a parser specification
---------------------------------------------------------------------

Important changes: 

 * We split statements into two kinds: stmtm and stmtu.  In the
   former, there are no if-statements at top-level without a matching
   else-branch.  In the latter there maybe if-statements without a
   matching else-branch.  This `dangling else' problem is discussed in
   Mogensen's book.  The implication is that

      if (e1) if (e2) s1 else s2 

   is parsed the same way as 

      if (e1) { if (e2) s1 else s2 }

   not as

      if (e1) { if (e2) s1 } else s2 
   
 * The grammar would still be ambiguous and cause shift/reduce
   conflicts, if we did not use precedence declarations on the
   operators that can appear in an expression:

       right    =             /* lowest precedence */
       nonassoc PRINT
       left     ||
       left     &&
       left     ==  != 
       nonassoc >  <  >=  <=
       left     +  - 
       left     *  /  %
       nonassoc !  &
       nonassoc [             /* highest precedence  */

   Most of these are quite obvious and can be taken straight from a
   textbook on C or Java.  For instance, the assignment operator (=)
   must bind less strongly than all of the logical and arithmetic
   operators, the logical connectives must bind less strongly than the
   comparisons, logical or (||) binds less strongly than logical and
   (&&), the <, >, <=, >= comparisons must be non-associative, lowest
   precedence, and so on.  

   The high precedence given to the left bracket ([) is necessary to
   avoid ambiguity and parse conflicts in expressions and variable
   declarations.  For expressions it implies that 
   
     * the parsing of  &a[2]  is  &(a[2]), not  (&a)[2] 

     * the parsing of  *a[2]  is  *(a[2]), not  (*a)[2]

     For variable declarations, it implies that 
   
     * the parsing of  int *a[10]  is  int *(a[10]), not  int (*a)[10]

   The low precedence given to PRINT is necessary to avoid ambiguity
   and parse conflicts in expressions with two-argument operators.
   It implies that 

     * the parsing of  print 2 + 5  is  print (2 + 5), not (print 2) + 5

   By introducing extra nonterminals and grammar rules, one can live
   without the precedence declarations.  Mogensen's book describes how
   to do that too.

 * Including access expressions into general expressions leads to
   ambiguity and conflicts.  We add a new nonterminal exprnotaccess to
   explicitly represent non-access expressions.

 * For non-access expressions, we distinguish atomic expressions (a
   variable, a constant, or an expression within parentheses) from
   non-atomic ones.  This avoids ambiguity in pointer dereferencing
   expressions, so that

     * the parsing of  *x*2  is  (*x)*2, not  *(x*2)

main ::= 
        topdecs EOF                     program

topdecs ::=                             top-level declarations
        <empty>
        topdec topdecs

topdec ::=                              top-level declaration
        vardec ;
        fundec  

vardec ::=                              variable or parameter declaration
        typ vardesc

vardesc ::=                             variable description
        NAME                            variable
        * vardesc                       pointer to
        ( vardesc )                     parenthesized variable description
        vardesc [ ]                     array of
        vardesc [ int ]                 array of (with allocation)

fundec ::=                              function declaration
        void NAME ( paramdecs ) block
        typ  NAME ( paramdecs ) block

paramdecs ::=                           comma-separated parameter list
        <empty>
        paramdecs1

paramdecs1 ::=                          non-empty parameter declaration list
        vardec
        vardec , paramdecs1             

stmt ::=                                statement
        stmtm                           without unmatched trailing if-else
        stmtu                           with unmatched trailing if-else

stmtm ::=                               no unmatched trailing if-else
        expr ;                          expression statement
        return ;                        return
        return expr ;                   return
        block                           block statement
        if (expr) stmtm else stmtm      if-else statement
        while (expr) stmtm              while-statement

stmtu ::=
        if (expr) stmtm else stmtu      unmatched trailing if
        if (expr) stmt                  unmatched if
        while (expr) stmtu              unmatched 

block ::=                               block statement
        { stmtordecseq }

stmtordecseq ::= 
        <empty>                         empty sequence
        stmt stmtordecseq               statement
        vardec ; stmtordecseq           local variable declaration

expr ::=
        access                          access expression
        exprnotaccess                   other expression types

exprnotaccess ::=
        atexprnotaccess                 atomic expression, not access
        access = expr                   assignment
        NAME ( exprs )                  function call
        ! expr                          logical negation
        print expr                      print expr's value
        println                         print value and newline
        expr +  expr                    plus
        expr -  expr                    minus
        expr *  expr                    times
        expr /  expr                    quotient
        expr %  expr                    remainder
        expr == expr                    equal to
        expr != expr                    not equal to
        expr >  expr                    greater than
        expr <  expr                    less than
        expr >= expr                    greater than or equal to
        expr <= expr                    less than or equal to
        expr && expr                    sequential and
        expr || expr                    sequential or

atexprnotaccess ::= 
        const                           constant
        ( exprnotaccess )               parenthesis
        & access                        address operator

access ::= 
        NAME                            variable
        ( access )                      parenthesis
        * access                        pointer dereferencing
        * atexprnotaccess               pointer dereferencing
        access [ expr ]                 array element access

exprs ::=
        <empty>                         empty list of expressions
        exprs1                          non-empty list of expressions

exprs1 ::=
        expr                            list with one expression
        expr , exprs1                   list with more than one expr

const ::=
        CSTINT                          non-negative integer constant
        - CSTINT                        negative integer constant
        null                            null constant

type ::=
        int                             the int type 
        char                            the char type


Lexical matters: tokens and comments
------------------------------------

NAME:       [`a`-`z``A`-`Z`][`a`-`z``A`-`Z``0`-`9`]*

            except for the keywords, which are:

            char else false if int null print println return true void while

CSTINT:     [0-9]+

CSTBOOL:    false | true

CSTSTRING:  `"`...`"` 
            with C string escapes \a \b \t \n \v \f \r \" \\ \ddd \uxxxx

OP:         + - * / % = == != < > <= >= && ||


There are two kinds of comments (not inside strings, not nested):

        // comment extends to end of line

        /* ... */ delimited comment
