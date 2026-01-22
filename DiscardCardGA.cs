using System;
using UnityEngine;

/// <summary>
/// GameAction that represents discarding a card from a player's hand.
/// This moves the card from the hand to the discard pile.
/// Cards can attach reactions to trigger "on discard" effects.
/// </summary>
public class DiscardCardGA : GameAction
{
    /// <summary>
    /// The card being discarded.
    /// </summary>
    public CardClick2 Card { get; private set; }

    /// <summary>
    /// The player who owns the card being discarded.
    /// </summary>
    public ulong PlayerID { get; private set; }

    /// <summary>
    /// The source that caused the discard (e.g., "self", "opponent", "effect").
    /// Useful for determining if discard was voluntary or forced.
    /// </summary>
    public string DiscardSource { get; private set; }

    /// <summary>
    /// Creates a new DiscardCardGA instance.
    /// </summary>
    /// <param name="card">The card to discard.</param>
    /// <param name="playerID">The ID of the player discarding the card.</param>
    /// <param name="discardSource">The source of the discard action (default: "self").</param>
    public DiscardCardGA(CardClick2 card, ulong playerID, string discardSource = "self")
    {
        this.Card = card;
        this.PlayerID = playerID;
        this.DiscardSource = discardSource;
    }
}
