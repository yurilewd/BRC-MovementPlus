## 2.0.0
Full rework of rails and grinding mechanics

Grinding and wallriding no longer steals your speed when starting one, in certain instances this can still happen but should be far less often to the point of almost never

Rail goons have been reworked and can be done on almost any rail corner, to compensate for this they have been weakened but can also be stacked, they now also work with a flat amount instead of a strength

Rail hard corners have been reworked, these are no longer tied to framerate and can be done on far more corners

Rails no longer hold you tight when trying to jump while upside down

Rail goons can be fully disabled, previously there were some cases where they would still sneak in even though disabled

Many mechanics now use the players average speed over a period of time instead of their current speed, this should make many mechanics feel smoother

A new mechanic know as a buttslap, to perform one you need to be performing a ground trick while in the air and during this ground trick period you can jump in mid air, this restores some of your combo meter and gives you a small boost of speed along with the jump, this can be performed multiple times in a given period

New mechanic known as a ledge climb cancel, to perform one you need to jump while doing the ledge climb animation, if done correctly you will cancel the animation your speed will be returned plus a little bit of bonus speed as well

A pretty large refactor of the code, most things were tried to be kept almost the same but it was a lot changed so various things might be slightly different

A soft rework to the super trick jump, it should behave almost the same but the math behind it is very different, this allows you to customize it far more inside the config

Many changes to the default config, most changes are meant to peel the game a little away from instant bursts of speed and instead have a more deliberate and smooth growth of speed

Reworked config, many more things added to the config and many existing things were slightly changed or just renamed, this will likely break existing configs and they will need to be redone if you edited them

Rail slope jumps have been refactored and should behave far more reliably

probably other stuff that I just forgot about, a ton was changed lol


## 1.1.2
The Vert bugs now have a cap and config values for it


## 1.1.1
Wallride frameboost runoff now disabled by default, added config value

Fixed a wallride issue

New sloped rail mechanic, jumping on a rail sloped up will give a higher jump and jumping on a downward slope will give a boost of speed


## 1.1.0
New collision change, this stops almost all instances of clipping through walls

New Goon Storage additions, hitting a frameboost will retain your highest stored goon

New combo mechanics, boosting while on the ground now works similarly to a manual and does not drop your combo, furthermore your combo is never dropped instantly upon touching the ground the timer will tick down very quickly instead

Vert ramp jump now has a cap

Vertical rails now retain speed better with a frameboost

A hand plant on top of some vertical rails can now be frameboosted

Super sliding on carpets has been improved

Hard landings have been changed, if in a movestyle you will no longer have a hard landing, if on your feet you will only have a hard landing if moving past your default maximum speed and having been in the air for 1.5 seconds.


## 1.0.3
whoops, fixed the vert stuff


## 1.0.2
More Slopcrew fixes


## 1.0.1
Fixed the perfect manual not working with Slopcrew


## 1.0.0
Initial release