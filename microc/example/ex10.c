// micro-C example 10 -- return a result from function; nested blocks

void main(int n) {
  int i;
  i = 0;
  while (i <= n) {
    print fac(i);
    i = i + 1;
  }
  print n;
}

int fac(int n) {
  if (n == 0)			/* fac's n */
    return 1;
  else 
    return n * fac(n-1);
}
