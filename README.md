# Card Game Application!

A digital card game. This project is a card game that allows players to draft a deck type, fight opponents, build cards, and win. Below is an explanation of the most important systems. A QUICK DEMO IS AT THE BOTTOM OF THIS README.

## Map System
- The map system creates a map that the player can traverse through! Either a regular node, Elite node, Boss node Shop node, or Random Encounter node.
- [Explanation] MapView.cs create a jagged array that hold the object MapNode. MapNode.cs holds important data such as position, spriteID, and mapNode Type. The jagged array is populated through specific editable parameters that can expand or shrink. Each indivdual map node is generated randomly along global parameters to make the map feel less robotic. The jagged array then loops through and establishes connections that DO NOT overlap. This data is saved into a text file called map.txt. The map will also try to deparse this file as a JSON file but if it fails it will just make a new one.
- https://youtu.be/JSxK1-WkBKk
  
## Card/Ability System
- Pick a card and build into a stronger version of itself!
- [Explanation] The game(GameViewSystem.cs) creates a match(Match.cs) with two players(Player.cs), you and the opponent. The cards are loaded by game(GameViewSystem.cs) through a static factory(DeckFactory.cs). This factory has all the cards in a folder with different subgroups. Cards(Card.cs) has many variables including: name, health, cost, ability chain, abilitys, ect. The cards are build like robots. First they are intantiated based on the type(To ensure scalability). Then componets are added on to card itself. Component such as <AbilityRoot>, <Afflictions>, or <Target> can be added if the JSON data calls for it. The components gives cards the ability to act within the game world. Within the <AbilityRoot> components there is an array of Abilitys within it. Each ability inherents from the game action(GameAction.cs) class. The game action is posted as a notification that puts the action into a List of game actions. The game actions are sequentially read. Some abilities(ones that are played by card) inherent from the IAbilityLoader(IAbilityLoader.cs) interface. The interface has a method(Load) that allows different type of IAbilityLoaders to do unique effects.
- https://youtu.be/zxZRitnAuc0

## Affliction System
- Cards can have status effects!
- [Explanation] Cards have affilctions, which are ability chains that have extra variables. The affliction has a variable that is called using the notification system. Once the evoke string is called the action system load all of the abilities in the chain and runs the afflication for everycard on the stack.

## Gameplay Video
- https://youtu.be/FYksTip_SKA

