using System;
using UnityEngine;

/// <summary>
/// GameAction that represents the beginning of a player's turn.
/// This is the counterpart to EndTurnGA and triggers at the start of each turn.
/// Cards can attach reactions to this action to trigger "on turn start" effects.
/// </summary>
public class BeginTurnGA : GameAction
{
    /// <summary>
    /// The ID of the player whose turn is beginning.
    /// </summary>
    public ulong PlayerID { get; private set; }

    /// <summary>
    /// The number of cards the player should draw at the start of their turn (default: 1).
    /// </summary>
    public int CardsToDrawOnTurnStart { get; private set; }

    /// <summary>
    /// Creates a new BeginTurnGA instance.
    /// </summary>
    /// <param name="playerID">The ID of the player whose turn is starting.</param>
    /// <param name="cardsToDrawOnTurnStart">Number of cards to draw at turn start (default 1).</param>
    public BeginTurnGA(ulong playerID, int cardsToDrawOnTurnStart = 1)
    {
        this.PlayerID = playerID;
        this.CardsToDrawOnTurnStart = cardsToDrawOnTurnStart;
    }
}
