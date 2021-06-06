// micro-C example 11 -- n queens problem * 1996-12-20, 2009-10-01

// Running this with the micro-C interpreter "eval" written in F#:
// Finding the 724 solutions to the 10 queens problem takes 6-7
// seconds on a 1.6 GHz Pentium M.

// Running this micro-C program, when compiled to bytecode: 
// Finding the 724 solutions to the 10 queens problem takes 0.6
// seconds with an abstract machine implemented in Java and executed
// on Sun JDK 1.5 Hotspot on a 1.6 GHz Pentium M running Windows XP.  
// It takes 0.2 seconds with an abstract machine implemented in C
// and executed on a 1.86 GHz Intel Xeon running Linux.

void main(int n) {
  int i; 
  int u;
  int used[100];
  int diag1[100];
  int diag2[100];
  int col[100];

  u = 1;
  while (u <= n) {
    used[u] = false;
    u = u+1;
  }

  u = 1;
  while (u <= 2 * n) {
    diag1[u] = diag2[u] = false;
    u = u+1;
  }

  i = u = 1;
  while (i > 0) {
    while (i <= n && i != 0) {
      while (u <= n && (used[u] || diag1[u-i+n] || diag2[u+i]))
	u = u + 1;
      if (u <= n) { // not used[u]; fill col[i] then try col[i+1]
	col[i] = u; 
	used[u] = diag1[u-i+n] = diag2[u+i] = true; 
	i = i+1; u = 1;
      } else {			// backtrack; try to find a new col[i-1]
	i = i-1;
	if (i > 0) { 
	  u = col[i]; 
	  used[u] = diag1[u-i+n] = diag2[u+i] = false; 
	  u = u+1;
	} 
      }
    }

    if (i > n) {                // output solution, then backtrack
      int j;
      j = 1;
      while (j <= n) {
	print col[j];  
	j = j+1;
      }
      println;
      i = i-1; 
      if (i > 0) { 
	u = col[i]; 
	used[u] = diag1[u-i+n] = diag2[u+i] = false; 
	u = u+1;
      }
    }
  }
}
