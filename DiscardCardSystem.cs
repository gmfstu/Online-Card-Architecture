using System.Collections;
using UnityEngine;

public class DiscardCardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DiscardCardGA>();
    }

    private IEnumerator DiscardCardPerformer(DiscardCardGA discardCardGA)
    {
        CardClick2 card = discardCardGA.card;
        Transform discardPile = discardCardGA.discardPile;
        HeldCards sourceHand = discardCardGA.sourceHand;

        if (card == null || discardPile == null)
        {
            Debug.LogError("Card or discard pile is null in DiscardCardPerformer.");
            yield break;
        }

        // Remove card from hand if it's in a hand
        if (sourceHand != null)
        {
            sourceHand.RemoveCard(card.gameObject);
        }

        // Update card container and parent
        card.container = CardClick2.Container.Discard;
        card.transform.SetParent(discardPile);
        card.transform.localPosition = Vector3.zero;

        Debug.Log($"Discarded card: {card.name}");

        yield return new WaitForSeconds(0.1f);
    }
}
