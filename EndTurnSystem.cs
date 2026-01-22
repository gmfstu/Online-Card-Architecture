using System.Collections;
using UnityEngine;

/// <summary>
/// System that handles the EndTurnGA performer.
/// Ends the current player's turn and triggers BeginTurn for the next player.
/// </summary>
public class EndTurnSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EndTurnGA>(EndTurnPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EndTurnGA>();
    }

    /// <summary>
    /// Performer for the EndTurn action.
    /// Switches the turn to the next player and triggers BeginTurn as a reaction.
    /// </summary>
    private IEnumerator EndTurnPerformer(EndTurnGA endturnGA)
    {
        Debug.Log($"Ending turn for player with ID: {endturnGA.PlayerID}");
        
        // Get the next player ID before ending turn
        ulong nextPlayerID = ActionSystem.Instance.playerTurn.Value;
        
        // End the current turn (switches player turn)
        ActionSystem.Instance.EndTurn();
        
        // Get the new current player (who's turn is beginning)
        nextPlayerID = ActionSystem.Instance.normalPlayerTurn;
        
        // Add BeginTurn as a post reaction for the next player
        ActionSystem.Instance.AddReaction(new BeginTurnGA(nextPlayerID));
        
        Debug.Log($"Next player's turn beginning: {nextPlayerID}");

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Static helper method to easily end turn from anywhere.
    /// Usage: EndTurnSystem.EndTurn(playerId);
    /// </summary>
    public static void EndTurn(ulong playerID)
    {
        var endTurnGA = new EndTurnGA(playerID);
        ActionSystem.Instance.Perform(endTurnGA);
    }
}
