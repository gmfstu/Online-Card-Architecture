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
        CardClick2 card = discardCardGA.Card;
        Transform player = discardCardGA.Player;

        if (card == null)
        {
            Debug.LogError("Card is null in DiscardCardPerformer.");
            yield break;
        }

        Debug.Log($"Discarding card: {card.name} from player: {player.name}");

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

        // Update card container to discard pile
        card.container = CardClick2.Container.Discard;
        card.cardSlot = null;
        
        // Move card to discard pile (you may need to add a discard pile transform reference)
        // For now, just deactivate or move off screen
        card.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);
    }
}
