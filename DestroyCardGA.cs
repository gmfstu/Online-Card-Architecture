using UnityEngine;

public class DestroyCardGA : GameAction
{
    public CardClick2 card { get; private set; }
    public Base sourceBase { get; private set; }

    public DestroyCardGA(CardClick2 card, Base sourceBase = null)
    {
        this.card = card;
        this.sourceBase = sourceBase;
    }
}
