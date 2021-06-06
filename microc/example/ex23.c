// micro-C example 23 -- exponentially slow Fibonacci

void main(int n) {
  int i;
  i = 0;
  while (i < n) {
    print i;
    print fib(i);
    println;
    i = i + 1;
  }
}

int fib(int n) {
  if (n < 2)
    return 1;
  else 
    return fib(n-2) + fib(n-1);
}
