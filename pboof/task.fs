object clone task
task extend
: newtask s" 64 64 64 task: _task  _task" evaluate ;
string instance name  :noname s" noname" name set ; execute
1 cells member tid  newtask tid !
1 cells member ticker  ' drop ticker !
1 cells member data  data off
2variable tmp
: run  ( -- )
   data ticker tmp 2!
   tid @ build  
   tid @ activate  
   tmp 2@  begin  over @ over @execute  pause  again ;
: stop
   tid @ kill ;
: cloned  cloned  newtask tid ! ;
extended
