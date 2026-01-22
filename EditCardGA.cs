using UnityEngine;

public class EditCardGA : GameAction
{
    public CardClick2 Card { get; private set; }
    public int? NewCardID { get; private set; }
    public string NewName { get; private set; }
    public string NewDescription { get; private set; }
    public string NewEffect { get; private set; }

    public EditCardGA(CardClick2 card, int? newCardID = null, string newName = null, string newDescription = null, string newEffect = null)
    {
        this.Card = card;
        this.NewCardID = newCardID;
        this.NewName = newName;
        this.NewDescription = newDescription;
        this.NewEffect = newEffect;
    }
}
