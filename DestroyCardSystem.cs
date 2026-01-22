using System.Collections;
using UnityEngine;

/// <summary>
/// System that handles the DestroyCardGA performer.
/// Removes cards from the board and places them in the scrap pile.
/// </summary>
public class DestroyCardSystem : MonoBehaviour
{
    /// <summary>
    /// Reference to the scrap pile transform (can be assigned in inspector).
    /// If null, cards will be deactivated but not moved to a specific pile.
    /// </summary>
    public Transform scrapPile;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DestroyCardGA>();
    }

    /// <summary>
    /// Performer for the DestroyCard action.
    /// Removes the card from the board and moves it to the scrap pile.
    /// </summary>
    private IEnumerator DestroyCardPerformer(DestroyCardGA destroyCardGA)
    {
        CardClick2 card = destroyCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in DestroyCardPerformer.");
            yield break;
        }

        Debug.Log($"Destroying card: {card.name} (source: {destroyCardGA.DestroySource}, destroyer: {destroyCardGA.DestroyerPlayerID})");

        // Remove the card from its base if it was on one
        if (destroyCardGA.SourceBase != null)
        {
            destroyCardGA.SourceBase.Cards.Remove(card);
        }

        // If the card tracks its location, clear it
        if (card.Location != null)
        {
            Base locationBase = card.Location.GetComponent<Base>();
            if (locationBase != null)
            {
                locationBase.Cards.Remove(card);
            }
        }

        // Update the card's container to Scrap
        card.container = CardClick2.Container.Scrap;
        card.cardSlot = null;

        // Move to scrap pile if one exists
        if (scrapPile != null)
        {
            card.transform.SetParent(scrapPile);
            card.transform.localPosition = Vector3.zero;
        }

        // Deactivate the card
        card.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.15f);
    }

    /// <summary>
    /// Static helper method to easily destroy a card from anywhere.
    /// Usage: DestroyCardSystem.DestroyCard(card, sourceBase, destroyerPlayerId);
    /// </summary>
    public static void DestroyCard(CardClick2 card, Base sourceBase, ulong destroyerPlayerID, string source = "effect")
    {
        var destroyCardGA = new DestroyCardGA(card, sourceBase, destroyerPlayerID, source);
        ActionSystem.Instance.Perform(destroyCardGA);
    }

    /// <summary>
    /// Static helper method to destroy a card as a reaction (within another action).
    /// Usage: DestroyCardSystem.AddDestroyReaction(card, sourceBase, destroyerPlayerId);
    /// </summary>
    public static void AddDestroyReaction(CardClick2 card, Base sourceBase, ulong destroyerPlayerID, string source = "combat")
    {
        var destroyCardGA = new DestroyCardGA(card, sourceBase, destroyerPlayerID, source);
        ActionSystem.Instance.AddReaction(destroyCardGA);
    }
}
