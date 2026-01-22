using System.Collections;
using UnityEngine;

public class DestroyCardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DestroyCardGA>();
    }

    private IEnumerator DestroyCardPerformer(DestroyCardGA destroyCardGA)
    {
        CardClick2 card = destroyCardGA.card;
        Base sourceBase = destroyCardGA.sourceBase;

        if (card == null)
        {
            Debug.LogError("Card is null in DestroyCardPerformer.");
            yield break;
        }

        // Remove card from base if it's on one
        if (sourceBase != null)
        {
            sourceBase.Cards.Remove(card);
        }

        // Update card to destroyed state
        card.container = CardClick2.Container.Scrap;
        card.cardID = 1; // Set to "Destroyed Card" ID from JSON
        
        // Optionally move to a scrap pile or deactivate
        card.gameObject.SetActive(false);

        Debug.Log($"Destroyed card: {card.name}");

        yield return new WaitForSeconds(0.1f);
    }
}
