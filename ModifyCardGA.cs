using UnityEngine;

public class ModifyCardGA : GameAction
{
    public CardClick2 targetCard { get; private set; }
    public int powerModifier { get; private set; }
    public int attackModifier { get; private set; }
    public int defenseModifier { get; private set; }
    public int healthModifier { get; private set; }

    public ModifyCardGA(CardClick2 targetCard, int powerMod = 0, int attackMod = 0, int defenseMod = 0, int healthMod = 0)
    {
        this.targetCard = targetCard;
        this.powerModifier = powerMod;
        this.attackModifier = attackMod;
        this.defenseModifier = defenseMod;
        this.healthModifier = healthMod;
    }
}
