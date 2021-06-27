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

