# Card Game Application

A digital card game. This project is a card game that allows players to draft a deck type, fight opponets, build cards, and win.

## Map System
- The map system creates a map that the player can traverse through! Either a regular node, Elite node, Boss node Shop node, or Random Encounter node.
-- [Explanation] MapView.cs create a jagged array that hold the object MapNode. MapNode.cs holds important data such as position, spriteID, and mapNode Type. The jagged array is populated through specific editable parameters that can expand or shrink. Each indivdual map node is generated randomly along global parameters to make the map feel less robotic. The jagged array then loops through and establishes connections that DO NOT overlap. This data is saved into a text file called map.txt. The map will also try to deparse this file as a JSON file but if it fails it will just make a new one.

## Draft Types

## Action System

## Card System

## Ability System

## Fighting other card

## Win/Lose Condition

## Save/Load System

## Gameplay
