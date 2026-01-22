using UnityEngine;

public class ActivateAbilityGA : GameAction
{
    public CardClick2 Card { get; private set; }
    public string AbilityName { get; private set; }
    public object[] AbilityParameters { get; private set; }

    public ActivateAbilityGA(CardClick2 card, string abilityName, params object[] parameters)
    {
        this.Card = card;
        this.AbilityName = abilityName;
        this.AbilityParameters = parameters;
    }
}
