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

