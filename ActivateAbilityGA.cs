using UnityEngine;

public class ActivateAbilityGA : GameAction
{
    public CardClick2 sourceCard { get; private set; }
    public string abilityEffect { get; private set; }
    public GameObject target { get; private set; }

    public ActivateAbilityGA(CardClick2 sourceCard, string abilityEffect, GameObject target = null)
    {
        this.sourceCard = sourceCard;
        this.abilityEffect = abilityEffect;
        this.target = target;
    }
}
