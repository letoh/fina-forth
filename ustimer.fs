\ Don't use this, include timer.fs
libc gettimeofday ptr int (int) gettimeofday
: sus ( -- seconds microseconds)
   0 0 sp@ 0 gettimeofday -21 ?throw swap ;
