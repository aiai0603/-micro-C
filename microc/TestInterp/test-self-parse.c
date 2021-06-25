Void main(Int n) {
   Int i;
   Int a;
   i=0;
   do {
       a= ++i;
       print("%d",a);

   }  
   while(i<n);
   i=0;
   do {
       a = i++;
       print("%d",i);
   }  
   while(i<n);
   i=20;
   do {
      a = i--;
      print("%d",i);
   } 
   while(i>n); 
   i=20;
   do {
      a = --i;
      print("%d",a);
   }  
   while(i>n);
}

