using UnityEngine;

public class ModifyCardGA : GameAction
{
    public CardClick2 Card { get; private set; }
    public int PowerModifier { get; private set; }
    public int AttackModifier { get; private set; }
    public int DefenseModifier { get; private set; }
    public int HealthModifier { get; private set; }

    public ModifyCardGA(CardClick2 card, int powerMod = 0, int attackMod = 0, int defenseMod = 0, int healthMod = 0)
    {
        this.Card = card;
        this.PowerModifier = powerMod;
        this.AttackModifier = attackMod;
        this.DefenseModifier = defenseMod;
        this.HealthModifier = healthMod;
    }
}
