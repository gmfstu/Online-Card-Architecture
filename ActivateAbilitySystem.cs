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
        CardClick2 card = activateAbilityGA.Card;
        string abilityName = activateAbilityGA.AbilityName;

        if (card == null)
        {
            Debug.LogError("Card is null in ActivateAbilityPerformer.");
            yield break;
        }

        Debug.Log($"Activating ability '{abilityName}' on card: {card.name}");

        // This is where you would implement specific ability logic
        // You can extend this with a switch statement or ability registry
        // For now, this logs the ability activation and can be extended later
        
        // Example: Parse the card effect and execute it
        if (!string.IsNullOrEmpty(card.cardEffect) && card.cardEffect != "None" && card.cardEffect != "Null")
        {
            Debug.Log($"Executing card effect: {card.cardEffect}");
            // Card effects would be handled here or trigger additional GameActions
        }

        yield return new WaitForSeconds(0.2f);
    }
}
