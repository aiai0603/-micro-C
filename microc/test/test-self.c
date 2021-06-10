void main(int n) {
   int i;
   i=0;
   do {
       print("%d",i++);
   }  
   while(i<n);
   i=0;
   do {
       print("%d",++i);
   }  
   while(i<n);
   i=20;
   do {
       print("%d",--i);
   } 
   while(i>n); 
   i=20;
   do {
        print("%d",i--);
   }  
   while(i>n);
}

