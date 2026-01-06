using System;
using System.Collections;
using UnityEngine;

public class PlayCardSystem : Singleton<PlayCardSystem>
{
     private void OnEnable()
    {
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        CardClick2 card = playCardGA.card;
        Base targetBase = playCardGA.baseTarget;

        if (card == null || targetBase == null) // ai test case slop!!!
        {
            Debug.LogError("Card or target base is null in PlayCardPerformer.");
            yield break;
        }

        card.heldCards.RemoveCard(card.gameObject); // Remove the card from the hand
        targetBase.AddCard(card); // Add the card to the target base
        card.PlayOnBase(targetBase); // Update the card's current base reference

        Debug.Log("Activate ability; " + card.cardEffect);
        if (card.cardEffect == "Draw 2 cards") {
            Debug.Log(card.transform.parent + "idwaidhpsaihpi");
            DrawCardGA drawCardGA = new(card.transform.parent.parent, card.transform.parent.GetComponent<Deck>()); // TODO: pass the deck component here, or get it from the card?
            // ActionSystem.SubscribeReaction<DrawCardGA>(firstDrawCardGA, ReactionTiming.POST);
            ActionSystem.Instance.AddReaction(drawCardGA); // TODO: is this pre or post or what?? but it works!!
            ActionSystem.Instance.AddReaction(drawCardGA);
            // TODO: remove the card, so you're not playing actions on bases that shouldn't be.
        }

        yield return new WaitForSeconds(0.5f); // Optional delay for drawing animation or effect
    }
}
