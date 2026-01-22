using System.Collections;
using UnityEngine;

public class ActivateAbilitySystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ActivateAbilityGA>(ActivateAbilityPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ActivateAbilityGA>();
    }

    private IEnumerator ActivateAbilityPerformer(ActivateAbilityGA activateAbilityGA)
    {
        CardClick2 sourceCard = activateAbilityGA.sourceCard;
        string abilityEffect = activateAbilityGA.abilityEffect;
        GameObject target = activateAbilityGA.target;

        if (sourceCard == null)
        {
            Debug.LogError("Source card is null in ActivateAbilityPerformer.");
            yield break;
        }

        Debug.Log($"Activating ability on card: {sourceCard.name} with effect: {abilityEffect}");

        // This is where specific ability logic would be implemented
        // For extensibility, you could use a strategy pattern or ability registry
        // Example: AbilityRegistry.ExecuteAbility(abilityEffect, sourceCard, target);
        
        // Placeholder for ability execution
        switch (abilityEffect)
        {
            case "Draw 2 cards":
                // Example of chaining game actions
                if (sourceCard.deck != null)
                {
                    DrawCardGA drawCard1 = new DrawCardGA(sourceCard.transform.parent.parent, sourceCard.deck);
                    DrawCardGA drawCard2 = new DrawCardGA(sourceCard.transform.parent.parent, sourceCard.deck);
                    activateAbilityGA.PostReactions.Add(drawCard1);
                    activateAbilityGA.PostReactions.Add(drawCard2);
                }
                break;
            case "Deal 2 damage":
                Debug.Log("Dealing 2 damage to target");
                // Implement damage logic here
                break;
            default:
                Debug.LogWarning($"Unknown ability effect: {abilityEffect}");
                break;
        }

        yield return new WaitForSeconds(0.1f);
    }
}
