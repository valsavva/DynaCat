# dynacat
The source code of the platformer game called "DynaCat" by nomnomapps. This game was released on Apple Store in 2012 
Game trailer: http://tinyurl.com/DynaCatTrailer
Game level walk-throughs: http://tinyurl.com/DynaCatWalkthrough

In the heart of the game is the game engine - "XGame". It allows for easy creation of simple, platformer-type games.
Also can be used for generic multi-platform apps with game-ified UI.

Engine features:
- Multi-platform. Built upon MonoGame (http://www.monogame.net/). Makes it possible to run on iOS, Android, Windows Phone, OS X, Windows
- XML based schema for level description
- support for "classes" of objects
```XML
<Class Id="clsBarrel">
    <Block IsExploding="true" Edges="Stick">
        <!-- destroy -->
        <BoolTrigger Condition="this:BoomClose" EnterAction="this_setDestroy1.Start()" StayTriggered="false" Group="this_BoxBoomStates"/>
        <SequenceSet Id="this_setDestroy1" InProgress="false">
            <Do Action="this.SetEdges('None');this:Destroyed" />
            <NumAnimation Target="this.Opacity" From="1" To="0" Duration="0.3" Autoreverse="false" RepeatCount="2"/>
            <Do Action="this.Disable()"/>
        </SequenceSet>
    </Block>
</Class>
```
- support for "includes" (one XML can "include" another)
- built-in types for sound effects, background music, spritesheets, fonts and other media
```XML
<Resources RootFolder="Sound">
    <SoundFile Id="sfAirplane" Source="sfxAirplane"/>
</Resources>
```
- built-in support for "hero" and "enimy"
- animations are done through "property animation" approach:
```XML
<NumAnimation Target="this_grpShowPoints.Opacity" From="1" To="0.5" RepeatCount="1" Duration="1" Fill="Hold" />
```
- "sequences", "parallels", "conditions", "triggers" and "loop" provide ultimate flexibility and control over animations the game flow:
```XML
<ParallelSet RepeatCount="1">
    <NumAnimation Target="this_grpShowPoints.Opacity" From="1" To="0.5" RepeatCount="1" Duration="1" Fill="Hold" />
    <RandomSet RepeatCount="1" >
        <NumAnimation Target="this_grpShowPoints.Y" From="0" To="-50" IsDelta="true" RepeatCount="1" Duration="1" Fill="Hold" />
        <NumAnimation Target="this_grpShowPoints.Y" From="0" To="-40" IsDelta="true" RepeatCount="1" Duration="1" Fill="Hold" />
    </RandomSet>
    <RandomSet RepeatCount="1" >
        <NumAnimation Target="this_grpShowPoints.X" From="-4" To="4" IsDelta="true" Autoreverse="true" RepeatCount="1.25" Duration="0.4" Fill="Hold" />
        <NumAnimation Target="this_grpShowPoints.X" From="4" To="-4" IsDelta="true" Autoreverse="true" RepeatCount="1.25" Duration="0.4" Fill="Hold" />
    </RandomSet>
</ParallelSet>
```
- simple collision detection
- element of physics engine - objects "bounce" or "stick"
```
<BoolTrigger Condition="hero:bounce" EnterAction="this_setHeroBounce.Start()"/>
```

The root object - Game - is responsible for enumerating all levels present in the game.
The engine automatically parses the level metadata and compiles it into a runnable flow.

On every tick the engine evaluates all objects in the current level and gives them a chance to mutate their state.

More detailed documentaion is to follow...
