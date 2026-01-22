using System.Collections;
using UnityEngine;

/// <summary>
/// System that handles the BeginTurnGA performer.
/// Automatically triggers at the start of each player's turn, allowing cards
/// and abilities to react to turn start events.
/// </summary>
public class BeginTurnSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<BeginTurnGA>(BeginTurnPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<BeginTurnGA>();
    }

    /// <summary>
    /// Performer for the BeginTurn action.
    /// This is called at the start of each turn and can trigger draw cards
    /// and other turn-start effects.
    /// </summary>
    private IEnumerator BeginTurnPerformer(BeginTurnGA beginTurnGA)
    {
        Debug.Log($"Beginning turn for player with ID: {beginTurnGA.PlayerID}");

        // Note: Drawing cards at turn start can be added as reactions if desired
        // The BeginTurnGA provides CardsToDrawOnTurnStart for this purpose
        // Example of how a card ability might use this:
        // ActionSystem.Instance.AddReaction(new DrawCardGA(playerTransform, deckComponent));

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Static helper method to easily trigger BeginTurn from anywhere.
    /// Usage: BeginTurnSystem.TriggerBeginTurn(playerId);
    /// </summary>
    public static void TriggerBeginTurn(ulong playerID, int cardsToDrawOnTurnStart = 1)
    {
        var beginTurnGA = new BeginTurnGA(playerID, cardsToDrawOnTurnStart);
        ActionSystem.Instance.Perform(beginTurnGA);
    }
}
