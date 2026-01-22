using System;
using UnityEngine;

/// <summary>
/// Enum defining the types of modifications that can be applied to a card.
/// </summary>
public enum ModificationType
{
    Attack,
    Defense,
    Health,
    Power,
    All // Applies the same modifier to all stats
}

/// <summary>
/// GameAction that represents temporarily modifying a card's stats.
/// These modifications are typically temporary and can be removed later.
/// Cards can attach reactions to trigger "on modify" effects.
/// </summary>
public class ModifyCardGA : GameAction
{
    /// <summary>
    /// The card being modified.
    /// </summary>
    public CardClick2 Card { get; private set; }

    /// <summary>
    /// The type of modification being applied.
    /// </summary>
    public ModificationType ModType { get; private set; }

    /// <summary>
    /// The value to modify by (can be positive for buffs or negative for debuffs).
    /// </summary>
    public int ModifierValue { get; private set; }

    /// <summary>
    /// The source of the modification (e.g., card name, effect name).
    /// Useful for tracking and potentially removing specific modifications.
    /// </summary>
    public string ModificationSource { get; private set; }

    /// <summary>
    /// The duration of the modification in turns. -1 means permanent until explicitly removed.
    /// </summary>
    public int Duration { get; private set; }

    /// <summary>
    /// The player who applied the modification.
    /// </summary>
    public ulong ModifierPlayerID { get; private set; }

    /// <summary>
    /// Creates a new ModifyCardGA instance.
    /// </summary>
    /// <param name="card">The card to modify.</param>
    /// <param name="modType">The type of stat to modify.</param>
    /// <param name="modifierValue">The value to add/subtract from the stat.</param>
    /// <param name="modifierPlayerID">The ID of the player applying the modification.</param>
    /// <param name="modificationSource">The source of the modification (default: "effect").</param>
    /// <param name="duration">Duration in turns, -1 for permanent (default: -1).</param>
    public ModifyCardGA(CardClick2 card, ModificationType modType, int modifierValue, ulong modifierPlayerID, 
                        string modificationSource = "effect", int duration = -1)
    {
        this.Card = card;
        this.ModType = modType;
        this.ModifierValue = modifierValue;
        this.ModifierPlayerID = modifierPlayerID;
        this.ModificationSource = modificationSource;
        this.Duration = duration;
    }
}
