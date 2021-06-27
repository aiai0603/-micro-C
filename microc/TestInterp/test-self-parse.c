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

