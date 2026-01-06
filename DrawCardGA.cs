using System;
using UnityEngine;

public class DrawCardGA : GameAction
{
    public Transform player { get; private set; }
    public Deck deckComponent { get; private set; }

    public DrawCardGA(Transform player, Deck deckComponent)
    {
        this.player = player;
        this.deckComponent = deckComponent;
    }
}
