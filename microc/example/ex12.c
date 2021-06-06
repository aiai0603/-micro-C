// micro-C example 12 -- tail calls

int main(int n) {
  if (n) 
    return main(n-1);
  else 
    return 17;
}
