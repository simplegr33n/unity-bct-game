# Battle Card Tactics
*... a pretty dumb working title*

<img src="https://github.com/simplegr33n/CBT/blob/master/screenshots/welcome.png?raw=true" width="600">

## About
BCT is the start to an open-source Tactical RPG engine with the ultimate goal of developing a game that delivers a balanced and competitive multiplayer experience along with a robust solo-play campaign. I took a break from solo-developing this project in early March 2018 and let it sit a few months while I pursued other interests. I am now pushing the project to GitHub as a way of encouraging myself to continue progress, as well as to open it up to any who might be interested in getting involved or finding some inspiration for their own projects.

<img src="https://github.com/simplegr33n/CBT/blob/master/screenshots/solar%20flare%203.png?raw=true" width="600">

### Current Status
At present, the maps in the game are all at least semi-playable (they were all developed in different stages and so some of the older ones no longer work so well) and if you're lucky you can get through a game from beginging to end in single-player mode. There are some bugs in terms of turn logic as well as physical movement in the game which can sometimes break the game completely. Adding insult to an already buggy experience, the current 'music' is painful to endure and the camera movement is often nauseating. Beyond that, no map is yet in its final form (one of them is even straight copied from Final Fantasy Tactics), so there is lots of work yet in terms of level design and textures. So too for character and ability design, modeling, texturing, animation. I've sketched a few original models and abilities into the game for testing during development, but none are in any way complete or even necessarily destined to be in a final form of the game.

<img src="https://github.com/simplegr33n/CBT/blob/master/screenshots/movement.png?raw=true" width="600">

Multiplayer currently uses Firebase for the backend as it's free at low volume and because the multiplayer implementation at this point is merely a crude passing of messages back and forth for each turn. It is currently not fully functional, as I started building out unit ability functions since first implementing multiplayer. At this point my focus is on creating a clean and complete working single player version before returning to look at multiplayer - though a basic mechanism for game creation, joining, turn management, and messaging is already somewhat implemented. I may simply pull Firebase and multiplayer functionality out of the project altogether for the time being for simplicity. Currently you would have to create your own Firebase Real Time Database in order to get multiplayer working on a clone of this repo. 

<img src="https://github.com/simplegr33n/CBT/blob/master/screenshots/firebase.png?raw=true" width="600">

***Gameplay Video***

[![Gameplay Video](https://img.youtube.com/vi/5OgnzV0K_1s/0.jpg)](https://www.youtube.com/watch?v=5OgnzV0K_1s)

### *//TODO:* ***Overview of basic gameplay***

### *//TODO:* ***Overview of major scripts/methods***

### *//TODO:* ***Overview of map construction***

### *//TODO:* ***TODO list // link to trello***

## Contact
### Wanna help?
If you're interested at all in helping out in terms of **game engine, multiplayer backend, music, models, textures, level design, animation, ui, ux, etc. etc.** (basically anything), do not hesitate to reach out: **ggdev3@gmail.com**. 

Issues and pull requests also welcome, and feel free to make use of my assets/code in any way you find useful (credit would be appreciated). Basically all assets (save for the Black Rook's profile picture, the skybox, and the map Zarghidas) are original works. 

## License
The MIT License

Copyright 2018 Geoff Golda

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.