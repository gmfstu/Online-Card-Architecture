using System.Collections;
using UnityEngine;

/// <summary>
/// System that handles the EditCardGA performer.
/// Permanently modifies card properties.
/// </summary>
public class EditCardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EditCardGA>(EditCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EditCardGA>();
    }

    /// <summary>
    /// Performer for the EditCard action.
    /// Permanently modifies the specified property of the card.
    /// </summary>
    private IEnumerator EditCardPerformer(EditCardGA editCardGA)
    {
        CardClick2 card = editCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in EditCardPerformer.");
            yield break;
        }

        Debug.Log($"Editing card: {card.name} - Setting {editCardGA.Property} (source: {editCardGA.EditSource})");

        switch (editCardGA.Property)
        {
            case CardProperty.Name:
                if (!string.IsNullOrEmpty(editCardGA.StringValue))
                {
                    card.cardNameText.text = editCardGA.StringValue;
                    card.transform.name = editCardGA.StringValue;
                    Debug.Log($"Card name changed to: {editCardGA.StringValue}");
                }
                break;

            case CardProperty.Power:
                card.cardPowerText.text = editCardGA.IntValue.ToString();
                Debug.Log($"Card power changed to: {editCardGA.IntValue}");
                break;

            case CardProperty.Effect:
                if (!string.IsNullOrEmpty(editCardGA.StringValue))
                {
                    card.cardEffect = editCardGA.StringValue;
                    Debug.Log($"Card effect changed to: {editCardGA.StringValue}");
                }
                break;

            case CardProperty.CardID:
                // Transform the card into a different card by loading new data
                yield return TransformCard(card, editCardGA.IntValue);
                break;

            case CardProperty.AttackMod:
            case CardProperty.DefenseMod:
            case CardProperty.HealthMod:
                // These would update internal tracking values
                Debug.Log($"Card {editCardGA.Property} changed to: {editCardGA.IntValue}");
                break;

            case CardProperty.Type:
                if (!string.IsNullOrEmpty(editCardGA.StringValue))
                {
                    Debug.Log($"Card type changed to: {editCardGA.StringValue}");
                }
                break;
        }

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Transforms a card into a different card by loading data from a new card ID.
    /// </summary>
    private IEnumerator TransformCard(CardClick2 card, int newCardID)
    {
        // Wait for JSONReader if not loaded
        while (!JSONReader.Instance.isLoaded)
        {
            yield return null;
        }

        // Find the new card data
        foreach (var cardData in JSONReader.Instance.cardDataList.carddata)
        {
            if (cardData.id == newCardID)
            {
                // Update all card properties
                card.cardID = newCardID;
                card.cardNameText.text = cardData.name;
                card.transform.name = cardData.name;
                
                if (cardData.power >= 0)
                    card.cardPowerText.text = cardData.power.ToString();
                else
                    card.cardPowerText.text = "";
                    
                card.cardDescriptionText.text = cardData.description;
                card.cardEffect = cardData.effect;
                
                Debug.Log($"Card transformed into: {cardData.name} (ID: {newCardID})");
                yield break;
            }
        }

        Debug.LogWarning($"Could not find card data for ID: {newCardID}");
    }

    /// <summary>
    /// Static helper method to easily edit a card's string property from anywhere.
    /// Usage: EditCardSystem.EditCardProperty(card, CardProperty.Name, "New Name", playerId);
    /// </summary>
    public static void EditCardProperty(CardClick2 card, CardProperty property, string value, ulong playerID, string source = "effect")
    {
        var editCardGA = new EditCardGA(card, property, value, playerID, source);
        ActionSystem.Instance.Perform(editCardGA);
    }

    /// <summary>
    /// Static helper method to easily edit a card's integer property from anywhere.
    /// Usage: EditCardSystem.EditCardProperty(card, CardProperty.Power, 5, playerId);
    /// </summary>
    public static void EditCardProperty(CardClick2 card, CardProperty property, int value, ulong playerID, string source = "effect")
    {
        var editCardGA = new EditCardGA(card, property, value, playerID, source);
        ActionSystem.Instance.Perform(editCardGA);
    }

    /// <summary>
    /// Static helper method to transform a card into another card.
    /// Usage: EditCardSystem.TransformCard(card, newCardId, playerId);
    /// </summary>
    public static void TransformCardTo(CardClick2 card, int newCardID, ulong playerID, string source = "effect")
    {
        var editCardGA = new EditCardGA(card, CardProperty.CardID, newCardID, playerID, source);
        ActionSystem.Instance.Perform(editCardGA);
    }

    /// <summary>
    /// Static helper method to edit a card property as a reaction.
    /// Usage: EditCardSystem.AddEditReaction(card, CardProperty.Power, 10, playerId);
    /// </summary>
    public static void AddEditReaction(CardClick2 card, CardProperty property, int value, ulong playerID, string source = "effect")
    {
        var editCardGA = new EditCardGA(card, property, value, playerID, source);
        ActionSystem.Instance.AddReaction(editCardGA);
    }

    /// <summary>
    /// Static helper method to edit a card string property as a reaction.
    /// Usage: EditCardSystem.AddEditReaction(card, CardProperty.Effect, "Draw 3 cards", playerId);
    /// </summary>
    public static void AddEditReaction(CardClick2 card, CardProperty property, string value, ulong playerID, string source = "effect")
    {
        var editCardGA = new EditCardGA(card, property, value, playerID, source);
        ActionSystem.Instance.AddReaction(editCardGA);
    }
}
