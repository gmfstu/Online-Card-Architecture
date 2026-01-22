using UnityEngine;

public class EditCardGA : GameAction
{
    public CardClick2 targetCard { get; private set; }
    public int newCardID { get; private set; }

    public EditCardGA(CardClick2 targetCard, int newCardID)
    {
        this.targetCard = targetCard;
        this.newCardID = newCardID;
    }
}
