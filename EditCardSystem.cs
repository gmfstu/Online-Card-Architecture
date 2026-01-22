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
        CardClick2 targetCard = editCardGA.targetCard;
        int newCardID = editCardGA.newCardID;

        if (targetCard == null)
        {
            Debug.LogError("Target card is null in EditCardPerformer.");
            yield break;
        }

        // Change the card's ID, which will cause it to reload data
        int oldCardID = targetCard.cardID;
        targetCard.cardID = newCardID;

        // Reload card data by restarting the LoadCardData coroutine
        targetCard.StartCoroutine(ReloadCardData(targetCard));

        Debug.Log($"Edited card from ID {oldCardID} to ID {newCardID}");

        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator ReloadCardData(CardClick2 card)
    {
        // Wait for the JSONReader to be loaded
        while (!JSONReader.Instance.isLoaded)
        {
            yield return null;
        }

        // Find and apply the new card data
        foreach (var cardData in JSONReader.Instance.cardDataList.carddata)
        {
            if (cardData.id == card.cardID)
            {
                card.cardNameText.text = cardData.name;
                card.transform.name = cardData.name;
                if (cardData.power < 0) 
                    card.cardPowerText.text = "";
                else 
                    card.cardPowerText.text = cardData.power.ToString();
                card.cardDescriptionText.text = cardData.description;
                card.cardEffect = cardData.effect;
                break;
            }
        }
    }
}
