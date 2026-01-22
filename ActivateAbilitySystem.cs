using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// System that handles the ActivateAbilityGA performer.
/// Parses and executes card ability effects.
/// </summary>
public class ActivateAbilitySystem : MonoBehaviour
{
    /// <summary>
    /// Dictionary of registered ability handlers.
    /// Key is the effect string pattern, value is the handler function.
    /// </summary>
    private static Dictionary<string, Action<ActivateAbilityGA>> abilityHandlers = new Dictionary<string, Action<ActivateAbilityGA>>();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ActivateAbilityGA>(ActivateAbilityPerformer);
        
        // Register default ability handlers
        RegisterDefaultHandlers();
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ActivateAbilityGA>();
    }

    /// <summary>
    /// Registers the default ability handlers for common effects.
    /// </summary>
    private void RegisterDefaultHandlers()
    {
        // Draw cards handler
        RegisterAbilityHandler("Draw", (ga) =>
        {
            // Parse "Draw X cards" format
            string[] parts = ga.AbilityEffect.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int count))
            {
                var deck = ga.SourceCard.deck;
                var player = ga.SourceCard.transform.parent.parent;
                for (int i = 0; i < count; i++)
                {
                    ActionSystem.Instance.AddReaction(new DrawCardGA(player, deck));
                }
            }
        });

        // Deal damage handler
        RegisterAbilityHandler("Deal", (ga) =>
        {
            // Parse "Deal X damage" format
            string[] parts = ga.AbilityEffect.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int damage))
            {
                Debug.Log($"Dealing {damage} damage from {ga.SourceCard.name}");
                // Damage logic would be implemented here
            }
        });

        // Buff/Modify handler
        RegisterAbilityHandler("Add", (ga) =>
        {
            // Parse "Add X to stat" format
            string[] parts = ga.AbilityEffect.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int value))
            {
                if (ga.TargetCard != null)
                {
                    ModificationType modType = ModificationType.All;
                    if (ga.AbilityEffect.ToLower().Contains("attack")) modType = ModificationType.Attack;
                    else if (ga.AbilityEffect.ToLower().Contains("defense")) modType = ModificationType.Defense;
                    else if (ga.AbilityEffect.ToLower().Contains("power")) modType = ModificationType.Power;
                    
                    ActionSystem.Instance.AddReaction(new ModifyCardGA(ga.TargetCard, modType, value, ga.ActivatorPlayerID, ga.SourceCard.name));
                }
            }
        });

        // Discard handler
        RegisterAbilityHandler("Discard", (ga) =>
        {
            if (ga.TargetCard != null)
            {
                ActionSystem.Instance.AddReaction(new DiscardCardGA(ga.TargetCard, ga.ActivatorPlayerID, ga.SourceCard.name));
            }
        });

        // Destroy handler
        RegisterAbilityHandler("Destroy", (ga) =>
        {
            if (ga.TargetCard != null)
            {
                var targetBase = ga.TargetCard.Location?.GetComponent<Base>();
                ActionSystem.Instance.AddReaction(new DestroyCardGA(ga.TargetCard, targetBase, ga.ActivatorPlayerID, "ability"));
            }
        });
    }

    /// <summary>
    /// Performer for the ActivateAbility action.
    /// Parses the effect string and executes the appropriate handler.
    /// </summary>
    private IEnumerator ActivateAbilityPerformer(ActivateAbilityGA activateAbilityGA)
    {
        CardClick2 card = activateAbilityGA.SourceCard;
        string effect = activateAbilityGA.AbilityEffect;

        if (card == null)
        {
            Debug.LogError("Source card is null in ActivateAbilityPerformer.");
            yield break;
        }

        if (string.IsNullOrEmpty(effect) || effect == "None" || effect == "Null")
        {
            Debug.Log($"Card {card.name} has no effect to activate.");
            yield break;
        }

        Debug.Log($"Activating ability: {effect} from {card.name} (timing: {activateAbilityGA.Timing})");

        // Find and execute matching handler
        bool handlerFound = false;
        foreach (var handler in abilityHandlers)
        {
            if (effect.StartsWith(handler.Key, StringComparison.OrdinalIgnoreCase))
            {
                handler.Value(activateAbilityGA);
                handlerFound = true;
                break;
            }
        }

        if (!handlerFound)
        {
            Debug.LogWarning($"No handler registered for effect: {effect}");
        }

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Registers a custom ability handler for a specific effect pattern.
    /// </summary>
    /// <param name="effectPrefix">The prefix to match (e.g., "Draw", "Deal").</param>
    /// <param name="handler">The handler function to execute.</param>
    public static void RegisterAbilityHandler(string effectPrefix, Action<ActivateAbilityGA> handler)
    {
        if (abilityHandlers.ContainsKey(effectPrefix))
        {
            abilityHandlers[effectPrefix] = handler;
        }
        else
        {
            abilityHandlers.Add(effectPrefix, handler);
        }
    }

    /// <summary>
    /// Unregisters an ability handler.
    /// </summary>
    public static void UnregisterAbilityHandler(string effectPrefix)
    {
        if (abilityHandlers.ContainsKey(effectPrefix))
        {
            abilityHandlers.Remove(effectPrefix);
        }
    }

    /// <summary>
    /// Static helper method to easily activate an ability from anywhere.
    /// Usage: ActivateAbilitySystem.ActivateAbility(card, effect, playerId);
    /// </summary>
    public static void ActivateAbility(CardClick2 sourceCard, string effect, ulong playerID, AbilityTiming timing = AbilityTiming.Manual)
    {
        var activateAbilityGA = new ActivateAbilityGA(sourceCard, effect, playerID, timing);
        ActionSystem.Instance.Perform(activateAbilityGA);
    }

    /// <summary>
    /// Static helper method to activate an ability with a target card.
    /// Usage: ActivateAbilitySystem.ActivateAbilityOnCard(sourceCard, effect, targetCard, playerId);
    /// </summary>
    public static void ActivateAbilityOnCard(CardClick2 sourceCard, string effect, CardClick2 targetCard, ulong playerID, AbilityTiming timing = AbilityTiming.Manual)
    {
        var activateAbilityGA = new ActivateAbilityGA(sourceCard, effect, playerID, timing, targetCard);
        ActionSystem.Instance.Perform(activateAbilityGA);
    }

    /// <summary>
    /// Static helper method to activate an ability as a reaction.
    /// Usage: ActivateAbilitySystem.AddAbilityReaction(card, effect, playerId);
    /// </summary>
    public static void AddAbilityReaction(CardClick2 sourceCard, string effect, ulong playerID, AbilityTiming timing = AbilityTiming.Triggered)
    {
        var activateAbilityGA = new ActivateAbilityGA(sourceCard, effect, playerID, timing);
        ActionSystem.Instance.AddReaction(activateAbilityGA);
    }
}
