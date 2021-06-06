// micro-C example 15 -- tail calls

void main(int n) {
  if (n!=0) {
    print n;
    main(n-1);
  } else 
    print 999999;
}
