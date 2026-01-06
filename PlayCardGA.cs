using System;
using UnityEngine;

public class PlayCardGA : GameAction
{
    public CardClick2 card { get; private set; }
    public Base baseTarget { get; private set; }

    public PlayCardGA(CardClick2 card, Base baseTarget)
    {
        this.card = card;
        this.baseTarget = baseTarget;
    }
}
