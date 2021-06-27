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

