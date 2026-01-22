using UnityEngine;

public class DestroyCardGA : GameAction
{
    public CardClick2 Card { get; private set; }

    public DestroyCardGA(CardClick2 card)
    {
        this.Card = card;
    }
}
