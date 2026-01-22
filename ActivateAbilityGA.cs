using System;
using UnityEngine;

/// <summary>
/// Enum defining when an ability can be activated.
/// </summary>
public enum AbilityTiming
{
    OnPlay,       // When the card is played
    OnTurnStart,  // At the beginning of owner's turn
    OnTurnEnd,    // At the end of owner's turn
    OnDestroy,    // When the card is destroyed
    OnDiscard,    // When the card is discarded
    Manual,       // Manually activated by player
    Passive,      // Always active while card is in play
    Triggered     // Activated by another game event
}

/// <summary>
/// GameAction that represents activating a card's ability.
/// This is the core action for triggering any card effect.
/// Cards can attach reactions to trigger "on ability activation" effects.
/// </summary>
public class ActivateAbilityGA : GameAction
{
    /// <summary>
    /// The card whose ability is being activated.
    /// </summary>
    public CardClick2 SourceCard { get; private set; }

    /// <summary>
    /// The ability/effect string being activated (from card data).
    /// </summary>
    public string AbilityEffect { get; private set; }

    /// <summary>
    /// The timing context for this ability activation.
    /// </summary>
    public AbilityTiming Timing { get; private set; }

    /// <summary>
    /// Optional target card for targeted abilities.
    /// </summary>
    public CardClick2 TargetCard { get; private set; }

    /// <summary>
    /// Optional target base for base-targeted abilities.
    /// </summary>
    public Base TargetBase { get; private set; }

    /// <summary>
    /// The player who activated the ability.
    /// </summary>
    public ulong ActivatorPlayerID { get; private set; }

    /// <summary>
    /// Creates a new ActivateAbilityGA instance.
    /// </summary>
    /// <param name="sourceCard">The card with the ability.</param>
    /// <param name="abilityEffect">The effect string to execute.</param>
    /// <param name="activatorPlayerID">The ID of the player activating.</param>
    /// <param name="timing">When/why the ability is being activated.</param>
    /// <param name="targetCard">Optional target card (default: null).</param>
    /// <param name="targetBase">Optional target base (default: null).</param>
    public ActivateAbilityGA(CardClick2 sourceCard, string abilityEffect, ulong activatorPlayerID, 
                             AbilityTiming timing = AbilityTiming.Manual, 
                             CardClick2 targetCard = null, Base targetBase = null)
    {
        this.SourceCard = sourceCard;
        this.AbilityEffect = abilityEffect;
        this.ActivatorPlayerID = activatorPlayerID;
        this.Timing = timing;
        this.TargetCard = targetCard;
        this.TargetBase = targetBase;
    }
}
