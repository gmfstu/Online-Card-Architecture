using System.Collections;
using UnityEngine;

/// <summary>
/// System that handles the DiscardCardGA performer.
/// Moves cards from the player's hand to the discard pile.
/// </summary>
public class DiscardCardSystem : MonoBehaviour
{
    /// <summary>
    /// Reference to the discard pile transform (can be assigned in inspector).
    /// If null, cards will be deactivated but not moved to a specific pile.
    /// </summary>
    public Transform discardPile;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DiscardCardGA>();
    }

    /// <summary>
    /// Performer for the DiscardCard action.
    /// Removes the card from the player's hand and moves it to the discard pile.
    /// </summary>
    private IEnumerator DiscardCardPerformer(DiscardCardGA discardCardGA)
    {
        CardClick2 card = discardCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in DiscardCardPerformer.");
            yield break;
        }

        Debug.Log($"Discarding card: {card.name} for player {discardCardGA.PlayerID} (source: {discardCardGA.DiscardSource})");

        // Remove the card from the player's hand if it's there
        if (card.heldCards != null && card.container == CardClick2.Container.Hand)
        {
            card.heldCards.RemoveCard(card.gameObject);
        }

        // Update the card's container to Discard
        card.container = CardClick2.Container.Discard;

        // Move to discard pile if one exists
        if (discardPile != null)
        {
            card.transform.SetParent(discardPile);
            card.transform.localPosition = Vector3.zero;
        }

        // Optionally deactivate the card visually
        card.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Static helper method to easily discard a card from anywhere.
    /// Usage: DiscardCardSystem.DiscardCard(card, playerId);
    /// </summary>
    public static void DiscardCard(CardClick2 card, ulong playerID, string source = "self")
    {
        var discardCardGA = new DiscardCardGA(card, playerID, source);
        ActionSystem.Instance.Perform(discardCardGA);
    }

    /// <summary>
    /// Static helper method to discard a card as a reaction (within another action).
    /// Usage: DiscardCardSystem.AddDiscardReaction(parentAction, card, playerId);
    /// </summary>
    public static void AddDiscardReaction(CardClick2 card, ulong playerID, string source = "effect")
    {
        var discardCardGA = new DiscardCardGA(card, playerID, source);
        ActionSystem.Instance.AddReaction(discardCardGA);
    }
}
