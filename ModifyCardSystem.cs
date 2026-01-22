using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single modification applied to a card.
/// </summary>
[System.Serializable]
public class CardModification
{
    public ModificationType Type;
    public int Value;
    public string Source;
    public int RemainingTurns; // -1 = permanent
    public ulong AppliedByPlayerID;

    public CardModification(ModificationType type, int value, string source, int duration, ulong appliedBy)
    {
        Type = type;
        Value = value;
        Source = source;
        RemainingTurns = duration;
        AppliedByPlayerID = appliedBy;
    }
}

/// <summary>
/// System that handles the ModifyCardGA performer.
/// Applies temporary or permanent stat modifications to cards.
/// </summary>
public class ModifyCardSystem : MonoBehaviour
{
    /// <summary>
    /// Dictionary tracking all active modifications by card instance ID.
    /// </summary>
    private static Dictionary<int, List<CardModification>> activeModifications = new Dictionary<int, List<CardModification>>();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ModifyCardGA>(ModifyCardPerformer);
        // Subscribe to turn end to decrement modification durations
        ActionSystem.SubscribeReaction<EndTurnGA>(OnTurnEnd, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ModifyCardGA>();
        ActionSystem.UnsubscribeReaction<EndTurnGA>(OnTurnEnd, ReactionTiming.POST);
    }

    /// <summary>
    /// Performer for the ModifyCard action.
    /// Applies the stat modification to the target card.
    /// </summary>
    private IEnumerator ModifyCardPerformer(ModifyCardGA modifyCardGA)
    {
        CardClick2 card = modifyCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in ModifyCardPerformer.");
            yield break;
        }

        Debug.Log($"Modifying card: {card.name} - {modifyCardGA.ModType} by {modifyCardGA.ModifierValue} (source: {modifyCardGA.ModificationSource})");

        // Create and store the modification
        var modification = new CardModification(
            modifyCardGA.ModType,
            modifyCardGA.ModifierValue,
            modifyCardGA.ModificationSource,
            modifyCardGA.Duration,
            modifyCardGA.ModifierPlayerID
        );

        int cardInstanceId = card.GetInstanceID();
        if (!activeModifications.ContainsKey(cardInstanceId))
        {
            activeModifications[cardInstanceId] = new List<CardModification>();
        }
        activeModifications[cardInstanceId].Add(modification);

        // Apply the visual update
        UpdateCardDisplay(card);

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Updates the card's visual display to reflect current modifications.
    /// </summary>
    private void UpdateCardDisplay(CardClick2 card)
    {
        int totalPowerMod = GetTotalModification(card, ModificationType.Power);
        int totalAttackMod = GetTotalModification(card, ModificationType.Attack);
        
        Debug.Log($"Card {card.name} now has +{totalPowerMod} power, +{totalAttackMod} attack from modifications.");
    }

    /// <summary>
    /// Called at the end of each turn to decrement modification durations.
    /// </summary>
    private void OnTurnEnd(GameAction action)
    {
        List<int> cardsToClean = new List<int>();

        foreach (var kvp in activeModifications)
        {
            var modifications = kvp.Value;
            for (int i = modifications.Count - 1; i >= 0; i--)
            {
                if (modifications[i].RemainingTurns > 0)
                {
                    modifications[i].RemainingTurns--;
                    if (modifications[i].RemainingTurns == 0)
                    {
                        modifications.RemoveAt(i);
                    }
                }
            }
            if (modifications.Count == 0)
            {
                cardsToClean.Add(kvp.Key);
            }
        }

        foreach (var cardId in cardsToClean)
        {
            activeModifications.Remove(cardId);
        }
    }

    /// <summary>
    /// Gets the total modification value for a specific stat on a card.
    /// </summary>
    public static int GetTotalModification(CardClick2 card, ModificationType type)
    {
        int cardInstanceId = card.GetInstanceID();
        if (!activeModifications.ContainsKey(cardInstanceId))
        {
            return 0;
        }

        int total = 0;
        foreach (var mod in activeModifications[cardInstanceId])
        {
            if (mod.Type == type || mod.Type == ModificationType.All)
            {
                total += mod.Value;
            }
        }
        return total;
    }

    /// <summary>
    /// Gets all active modifications on a card.
    /// </summary>
    public static List<CardModification> GetModifications(CardClick2 card)
    {
        int cardInstanceId = card.GetInstanceID();
        if (!activeModifications.ContainsKey(cardInstanceId))
        {
            return new List<CardModification>();
        }
        return new List<CardModification>(activeModifications[cardInstanceId]);
    }

    /// <summary>
    /// Removes all modifications from a specific source.
    /// </summary>
    public static void RemoveModificationsBySource(CardClick2 card, string source)
    {
        int cardInstanceId = card.GetInstanceID();
        if (!activeModifications.ContainsKey(cardInstanceId))
        {
            return;
        }
        activeModifications[cardInstanceId].RemoveAll(m => m.Source == source);
    }

    /// <summary>
    /// Clears all modifications from a card.
    /// </summary>
    public static void ClearAllModifications(CardClick2 card)
    {
        int cardInstanceId = card.GetInstanceID();
        if (activeModifications.ContainsKey(cardInstanceId))
        {
            activeModifications.Remove(cardInstanceId);
        }
    }

    /// <summary>
    /// Static helper method to easily modify a card from anywhere.
    /// Usage: ModifyCardSystem.ModifyCard(card, ModificationType.Attack, 2, playerId);
    /// </summary>
    public static void ModifyCard(CardClick2 card, ModificationType modType, int value, ulong playerID, string source = "effect", int duration = -1)
    {
        var modifyCardGA = new ModifyCardGA(card, modType, value, playerID, source, duration);
        ActionSystem.Instance.Perform(modifyCardGA);
    }

    /// <summary>
    /// Static helper method to modify a card as a reaction (within another action).
    /// Usage: ModifyCardSystem.AddModifyReaction(card, ModificationType.Power, 1, playerId);
    /// </summary>
    public static void AddModifyReaction(CardClick2 card, ModificationType modType, int value, ulong playerID, string source = "effect", int duration = -1)
    {
        var modifyCardGA = new ModifyCardGA(card, modType, value, playerID, source, duration);
        ActionSystem.Instance.AddReaction(modifyCardGA);
    }
}
