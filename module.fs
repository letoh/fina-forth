\ *S ANS Forth name scoping mechanism  -- BNE

\ *P In large Forth programs, organizing namespace by breaking it up into
\ ** different modules makes for a cleaner and less error prone system.
\ ** The names of the four words used to do this are in common use by MPE.

\ *P This version can be added to any ANS Forth. It hides private words of
\ ** a module in a temporary namespace. You are free to nest modules and
\ ** manipulate the search order within a module, with the understanding
\ ** that the implementation assumes the current wordlist is at the top of
\ ** the search order.

: MODULE ( <name> -- )
   get-order wordlist dup set-current                 \ search order is:
   create dup , swap 1+   set-order ;                 \ ... public private

: END-MODULE
   previous definitions ;

: EXPORT ( <name> -- )
   get-order >in @ bl word find dup 0= abort" ???"    \ copy private
   >r >r >r 2 pick set-current r> >in ! : r> compile, \ to public
   postpone ; r> 0< 0= if immediate then
   over set-current set-order ;

: EXPOSE-MODULE ( <name> -- )                         \ add module to
   get-order ' >body @ swap 1+ set-order ;            \ search order

\ *P Usage

\ *E MODULE test
\ ** : bar boop over bop ;
\ ** : foo blah blivit ;
\ ** EXPORT foo
\ ** \ more...
\ ** END-MODULE

\ *P For debugging purposes, you can make the whole module visible with
\ ** EXPOSE-MODULE test. Afterwards, you can hide it again using PREVIOUS.

