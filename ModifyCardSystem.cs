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
        CardClick2 targetCard = modifyCardGA.targetCard;

        if (targetCard == null)
        {
            Debug.LogError("Target card is null in ModifyCardPerformer.");
            yield break;
        }

        // Note: This is a placeholder implementation. In a full system, you would need to:
        // 1. Add tracking fields to CardClick2 for current modified stats
        // 2. Update the visual display of power/attack/defense/health
        // 3. Store modification history for potential removal/reversal
        
        Debug.Log($"Modifying card: {targetCard.name}");
        Debug.Log($"Power: +{modifyCardGA.powerModifier}, Attack: +{modifyCardGA.attackModifier}, Defense: +{modifyCardGA.defenseModifier}, Health: +{modifyCardGA.healthModifier}");

        // TODO: Implement actual stat modification when CardClick2 has stat tracking
        // For now, we just log the modification
        // Example future implementation:
        // targetCard.currentPower += modifyCardGA.powerModifier;
        // targetCard.currentAttack += modifyCardGA.attackModifier;
        // targetCard.currentDefense += modifyCardGA.defenseModifier;
        // targetCard.currentHealth += modifyCardGA.healthModifier;
        // targetCard.UpdateVisuals();

        yield return new WaitForSeconds(0.1f);
    }
}
