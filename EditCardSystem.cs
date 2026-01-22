using System.Collections;
using UnityEngine;

public class EditCardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EditCardGA>(EditCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EditCardGA>();
    }

    private IEnumerator EditCardPerformer(EditCardGA editCardGA)
    {
        CardClick2 card = editCardGA.Card;

        if (card == null)
        {
            Debug.LogError("Card is null in EditCardPerformer.");
            yield break;
        }

        Debug.Log($"Editing card: {card.name}");

        // Update card ID and reload data if specified
        if (editCardGA.NewCardID.HasValue)
        {
            card.cardID = editCardGA.NewCardID.Value;
            Debug.Log($"Card ID changed to: {editCardGA.NewCardID.Value}");
            // You may want to reload the card data from JSON here
        }

        // Update card name
        if (!string.IsNullOrEmpty(editCardGA.NewName))
        {
            if (card.cardNameText != null)
            {
                card.cardNameText.text = editCardGA.NewName;
            }
            card.transform.name = editCardGA.NewName;
            Debug.Log($"Card name changed to: {editCardGA.NewName}");
        }

        // Update card description
        if (!string.IsNullOrEmpty(editCardGA.NewDescription))
        {
            if (card.cardDescriptionText != null)
            {
                card.cardDescriptionText.text = editCardGA.NewDescription;
            }
            Debug.Log($"Card description changed to: {editCardGA.NewDescription}");
        }

        // Update card effect
        if (!string.IsNullOrEmpty(editCardGA.NewEffect))
        {
            card.cardEffect = editCardGA.NewEffect;
            Debug.Log($"Card effect changed to: {editCardGA.NewEffect}");
        }

        yield return new WaitForSeconds(0.2f);
    }
}
