// micro-C example 22 -- leapyear, optimization of andalso and orelse

void main(int n) {
  int y;
  y = 1889;
  while (y < n) {
    y = y + 1;
    if (leapyear(y))
      print y;
  }
}

int leapyear(int y) {
  return y % 4 == 0 && (y % 100 != 0 || y % 400 == 0);
}
