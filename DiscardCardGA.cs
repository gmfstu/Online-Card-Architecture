using UnityEngine;

public class DiscardCardGA : GameAction
{
    public CardClick2 Card { get; private set; }
    public Transform Player { get; private set; }

    public DiscardCardGA(CardClick2 card, Transform player)
    {
        this.Card = card;
        this.Player = player;
    }
}
