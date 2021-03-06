# Intro

This repo houses some code for an interview question I received - I thought it was fun so I plopped it in a repo.  Besides: now the interviewing team can poke around my code and get a sense for my approach to release management as well.

## Game Description

Design the classes and data structure for the game described below. Implement a method playGame() which simulates a game then prints the winning player and their score.

Game rules:
In this game players roll dice and try to collect the lowest score. A 4 counts as zero, all other numbers count as face value. A player’s score is the total spots showing on the dice when she finishes her turn (excluding fours because they’re zero). The object of the game is to get the lowest score possible.

The game is played as follows between 4 players:
Play starts with one person randomly being chosen to start rolling and proceeds in succession until all players have rolled.
The player rolls all five dice, then must keep at least one dice but may keep more at her discretion (She can stop on her first roll if she so wishes).
Those dice which are not kept are rolled again and each round she must keep at least one more until all the dice are out.
Once each player has rolled the player who scored the lowest wins.
Repeat for three more rounds in succession so that the next person starts rolling first (at the end each player will have started).
After all four rounds have been completed the player with the lowest combined score wins.

## Issue Tracking

I'm using Trello to document/track the actual work for this guy (to make sure I don't take too long implementing this - it is an interview after all!)  [Check it out here](https://trello.com/b/AU9KAhp3)