using System.Collections;
using UnityEngine;

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

    private IEnumerator BeginTurnPerformer(BeginTurnGA beginTurnGA)
    {
        Debug.Log("Beginning turn for player with ID: " + beginTurnGA.PlayerID);

        // This is where turn-start effects would be triggered
        // Examples:
        // - Draw a card at the start of turn
        // - Refill action points/mana
        // - Trigger "at start of turn" abilities on cards in play
        // - Reset card states (e.g., exhausted cards become ready)

        yield return new WaitForSeconds(0.1f);
    }
}
