using System.Collections;
using UnityEngine;

public class ModifyCardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ModifyCardGA>(ModifyCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ModifyCardGA>();
    }

    private IEnumerator ModifyCardPerformer(ModifyCardGA modifyCardGA)
    {
        CardClick2 card = modifyCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in ModifyCardPerformer.");
            yield break;
        }

        Debug.Log($"Modifying card: {card.name} with Power+{modifyCardGA.PowerModifier}, Attack+{modifyCardGA.AttackModifier}, Defense+{modifyCardGA.DefenseModifier}, Health+{modifyCardGA.HealthModifier}");

        // Modify the card's power display
        if (modifyCardGA.PowerModifier != 0 && card.cardPowerText != null)
        {
            if (int.TryParse(card.cardPowerText.text, out int currentPower))
            {
                int newPower = currentPower + modifyCardGA.PowerModifier;
                card.cardPowerText.text = newPower.ToString();
            }
        }

        // Note: Attack, Defense, and Health modifiers would require additional card properties
        // For now, these are placeholders for when those stats are fully implemented
        // You can extend CardClick2 to have attackValue, defenseValue, healthValue properties

        yield return new WaitForSeconds(0.2f);
    }
}
