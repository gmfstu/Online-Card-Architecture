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
        CardClick2 card = destroyCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in DestroyCardPerformer.");
            yield break;
        }

        Debug.Log($"Destroying card: {card.name}");

        // Remove card from hand if it's there
        if (card.container == CardClick2.Container.Hand && card.heldCards != null)
        {
            card.heldCards.RemoveCard(card.gameObject);
        }
        // Remove card from base if it's there
        else if (card.container == CardClick2.Container.Base && card.Location != null)
        {
            card.Location.GetComponent<Base>().Cards.Remove(card);
        }

        // Update card container to scrap (destroyed)
        card.container = CardClick2.Container.Scrap;
        card.cardSlot = null;
        
        // Permanently remove the card
        Destroy(card.gameObject);

        yield return new WaitForSeconds(0.2f);
    }
}
