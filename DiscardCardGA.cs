using UnityEngine;

public class DiscardCardGA : GameAction
{
    public CardClick2 card { get; private set; }
    public Transform discardPile { get; private set; }
    public HeldCards sourceHand { get; private set; }

    public DiscardCardGA(CardClick2 card, Transform discardPile, HeldCards sourceHand)
    {
        this.card = card;
        this.discardPile = discardPile;
        this.sourceHand = sourceHand;
    }
}
