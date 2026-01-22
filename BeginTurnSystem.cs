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
        
        // Add any begin turn logic here (e.g., draw card, reset resources, etc.)
        // This is where card effects that trigger "at the start of your turn" would fire
        
        yield return new WaitForSeconds(0.1f);
    }
}
