using System.Collections;
using UnityEngine;

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

    private IEnumerator EndTurnPerformer(EndTurnGA endturnGA)
    {
        Debug.Log("Ending turn for player with ID: " + endturnGA.PlayerID);
        ActionSystem.Instance.EndTurn();

        yield return new WaitForSeconds(0.1f); // Optional delay for drawing animation or effect
    }
}
