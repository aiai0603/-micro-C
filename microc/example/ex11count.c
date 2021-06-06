// micro-C example 11 -- count n queens solutions * 1996-12-20, 2017-04-11

// Finding and counting the 14200 solutions to the 12 queens problem
// takes 31.4 secs when executed using the micro-C interpreter "eval"
// written in F#, when running on a 2.5 GHz Intel i7.

// It takes at least 2.2 sec when the micro-C program is compiled
// (unoptimized) to bytecode and interpreted on the Machine.java
// bytecode interpreter run on a 2.5 GHz Intel i7; and 1.3 sec when
// interpreted on the machine.c bytecode interpreter.

// It takes 0.380 sec when the micro-C program is compiled to
// (templatish stack-only) x86 assembly code, on the 2.5 GHz i7.

// It takes 0.090 sec when the micro-C program is compiled to
// more register-based x86 assembly code, on the 2.5 GHz i7.


void main(int n) {
  int i; 
  int u;
  int used[100];
  int diag1[100];
  int diag2[100];
  int col[100];
  int count;
  count = 0;

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
	diag1[u-i+n] = diag2[u+i] = used[u] = true; 
	i = i+1; u = 1;
      } else {			// backtrack; try to find a new col[i-1]
	i = i-1;
	if (i > 0) { 
	  u = col[i];
	  diag1[u-i+n] = diag2[u+i] = used[u] = false; 	  
	  u = u+1;
	} 
      }
    }

    if (i > n) {                // count solution, then backtrack
      count = count + 1;
      i = i-1; 
      if (i > 0) { 
	u = col[i];
	diag1[u-i+n] = diag2[u+i] = used[u] = false; 
	u = u+1;
      }
    }
  }
  print count;
  println;
}
