using System;
using UnityEngine;

/// <summary>
/// GameAction that represents destroying a card (usually a minion on the board).
/// Unlike discard, destroyed cards typically go to a separate "scrap" pile and
/// may have different interaction rules.
/// Cards can attach reactions to trigger "on destroy" effects.
/// </summary>
public class DestroyCardGA : GameAction
{
    /// <summary>
    /// The card being destroyed.
    /// </summary>
    public CardClick2 Card { get; private set; }

    /// <summary>
    /// The base the card was on when destroyed (if applicable).
    /// </summary>
    public Base SourceBase { get; private set; }

    /// <summary>
    /// The source that caused the destruction (e.g., "combat", "effect", "ability").
    /// </summary>
    public string DestroySource { get; private set; }

    /// <summary>
    /// The player who caused the destruction (may differ from the card's owner).
    /// </summary>
    public ulong DestroyerPlayerID { get; private set; }

    /// <summary>
    /// Creates a new DestroyCardGA instance.
    /// </summary>
    /// <param name="card">The card to destroy.</param>
    /// <param name="sourceBase">The base the card was on (can be null).</param>
    /// <param name="destroyerPlayerID">The ID of the player who destroyed the card.</param>
    /// <param name="destroySource">The source of destruction (default: "combat").</param>
    public DestroyCardGA(CardClick2 card, Base sourceBase, ulong destroyerPlayerID, string destroySource = "combat")
    {
        this.Card = card;
        this.SourceBase = sourceBase;
        this.DestroyerPlayerID = destroyerPlayerID;
        this.DestroySource = destroySource;
    }
}
