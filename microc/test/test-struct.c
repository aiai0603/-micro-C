Struct test {
        Int a;
        Float b;
        String s;
        Int arr[100];

} ;


Struct test2 {
        Int a;
        Float b;

} ;
    
Void main(Int n) {
  Int test ;
  test = 23;
  Struct test t;
  Struct test2 t2;
  t.a = 145;
  t.b = 12.3;
  t.s = "asasasas";
  t.arr[2]= 1;
  t.arr[5]= 2;
  t.arr[6]= 5;

  t2.a = 12121212;

 print("%d",t2.a);

  print("%d",t.a);
  print("%f",t.b);
  println(t.s);
  print("%d",t.arr[6]);
  print("%d",test);

 
}

