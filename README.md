# Card Game Application

A digital card game. This project is a card game that allows players to draft a deck type, fight opponets, build cards, and win. Below is an explanation of the most important systems. A QUICK DEMO IS AT THE BOTTOM OF THIS README.

## Map System
- The map system creates a map that the player can traverse through! Either a regular node, Elite node, Boss node Shop node, or Random Encounter node.
- [Explanation] MapView.cs create a jagged array that hold the object MapNode. MapNode.cs holds important data such as position, spriteID, and mapNode Type. The jagged array is populated through specific editable parameters that can expand or shrink. Each indivdual map node is generated randomly along global parameters to make the map feel less robotic. The jagged array then loops through and establishes connections that DO NOT overlap. This data is saved into a text file called map.txt. The map will also try to deparse this file as a JSON file but if it fails it will just make a new one.

## Draft Types
- Choose a card type to use and get as rewards!
- [Explanation]

## Action System
- Choose a card type to get as rewards!
- [Explanation] 
  
## Card/Ability System
- Pick a card and build into a stronger version of itself!
- [Explanation] Card(Card.cs) has many variables including: name, health, cost, ability chain, abilitys, ect. These variables are loaded whenever a Regular node, Elite node, Boss node (Enemy encounter) is instantiate. The game(GameViewSystem.cs) creates a match(Match.cs) with two players(Player.cs) you and the opponent. The cards are loaded by game(GameViewSystem.cs) through a static factory(DeckFactory.cs). This factory has all the cards in a folder with different subgroups. The cards are build like robots. First they are intantiated based on the type(To ensure scalability). Then componets are added on to card itself. Component such as <AbilityRoot>, <Afflictions>, or <Target> can be added if the JSON data calls for it. The components gives cards the ability to act within the game world. Within the <AbilityRoot> components as a array of Abilitys within it. Each ability

## Ability System
- Mix and match different abilities to create the best ability chain.
- [Explanation] The <AbilityRoot> component has an ArrayList called abilities. This array looped through whenever a card is played

## Status System
- Got get afflictions!
- [Explanation] Card.cs has many variables including: name, health, cost,

## Ability System
- Choose a card type to get as rewards!
- [Explanation]

## Fighting other card
- Choose a card type to get as rewards!
- [Explanation]

## Win/Lose Condition
- Choose a card type to get as rewards!
- [Explanation]

## Save/Load System
- Choose a card type to get as rewards!
- [Explanation]

## Gameplay Video
- Choose a card type to get as rewards!
- [Explanation]
