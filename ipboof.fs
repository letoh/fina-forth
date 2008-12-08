\ OBJECT STACK

16 cells
constant /ostack
\g Size of object stack

create otop /ostack allot
\g Start of object stack

otop /ostack + 
constant obottom
\g Bottom of object stack

obottom 
value otos
\g Pointer to top of object stack

\g Push object to object stack
: >o ( obj -- O: -- obj )
   otos cell- to otos  otos ! ;

\g Drop object from object stack
: odrop ( O: obj -- )
   otos cell+ to otos ;

\g Fetch topmost object from object stack
: o@ ( -- obj O: obj -- obj )
   otos @ ;

\g Pop object from object stack
: o> ( -- obj O: obj -- )
   o@ odrop ;

\g Duplicate topmost object
: odup ( O: obj -- obj )
   o@ >o ;

\g Pick item from object stack
: opick ( u -- obj ) 
   cells otos + @ ;

\g Depth of object stack
: odepth ( -- u )
   obottom otos - 1 cells / ;

\g Print object stack
: o.s
   ." O: " odepth 0 <# [char] > hold #s [char] < hold #> type space
   odepth 0 ?do i opick . loop cr ;

\ OBJECT ARENA

1024 16 * constant /oarena     \ Size of object arena
create o0 /oarena allot        \ Start of object arena
here constant otop             \ End of object arena
o0 value ohere                 \ Object arena pointer
: osize ( -- u )  ohere o0 - ; \ Current size of object arena
0 value sdict0                 \ Saved dict0
0 value sdicttop               \ Saved dicttop
0 value shere                  \ Saved here
: >oarena                      \ Switch compilation area to object arena
   dict0 to sdict0  dicttop to sdicttop  here to shere
   o0 to dict0  otop to dicttop  ohere to here  ;
: oarena>                      \ Restore normal compilation area
   here to ohere  sdict0 to dict0  sdicttop to dicttop  shere to here ;


\ WORLD

\ Some definitions:
\ Current object: topmost object in object stack
\ Active object: object that will receive subsequent messages

\ Create the bootstrap world, just a wordlist and a sizeof field in the object arena
>oarena wordlist 3 cells , oarena> 
constant (world)


\ Push the bootstrap world to the object and order stacks and establish it as 
\ current worldist, making it the active object
(world) >o o@ +order definitions

: (extend) ( O: obj -- obj )
   o@ +order definitions ;

: (extended)  previous definitions ;

\g Make current object the active object
: extend ( O: obj -- obj )
   odup (extend) ;

\g Restore previously active object
: extended ( O: obj -- )
   (extended) odrop ;

\g Send late message to current object
: (late) ( addr len --  O: obj -- ) (extend) nfa doword extended ;

\g Send late message to current object
: late ( "message" -- ?  runtime: ? -- ?  O: obj -- )
   parse-word postpone sliteral postpone (late) ; immediate

\g Address of current object size field
: sizeof ( -- addr  O: obj -- obj )
   o@ cell+ cell+ ;

\g Create member in active object
: member ( size "name" -- ) 
   >oarena dup allot oarena> 
   create sizeof @ , sizeof +! does> @ o@ + ; immediate

\g Clone active object
: cloned ( -- clone  O: prototype -- prototype )
   >oarena here 
   o@ @ wordlist !  \ Create wordlist and chain it to prototype wordlist
   sizeof here over @ cell- cell- dup allot move  \ Clone prototype data
   oarena> ;

: single  parse-word (late) ;

\g Runtime for cloned objects
: doobj @r+ >o @r+ execute odrop ;

\g Clone active object by sending a late CLONED message
: clone ( "name" --  )
   o@  extended create immediate  >o 
   extend  late cloned ,
   does> @ state @ if postpone doobj dup , then >o single ;

\g Runtime for instance members
: doinst @r+ o@ + >o @r+ execute odrop ;

\g Instance active object as member of the previously active object
: instance ( "name" -- )
   o@  sizeof @ extended create immediate sizeof @ , sizeof +!  >o
   extend late cloned drop
   does> @ state @ if postpone doinst dup , then o@ + >o single ;

\g Dump object memory 
: dump  o@ sizeof @ dump ;

\g Print attribute
: .attr ( "attr" -- )
   odepth 1- spaces @r+ dup xt>name .name ." member at " 
   execute dup 16 based . ." : " @ . cr ;

\g Print object
: print
   odepth 2 - spaces ." object at " o@ 16 based . ." :" cr .attr sizeof ;

extended

(world) +order
: world (world) >o single ;
previous


\ OBJECT

world extend

world clone object
\g Prototype object

extended