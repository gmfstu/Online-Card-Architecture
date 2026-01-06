using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DrawCardSystem : MonoBehaviour
{
    public GameObject deck;
    // private Deck deckComponent;
    private HeldCards heldCards;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        // heldCards = hand.GetComponent<HeldCards>();
        // deckComponent = deck.GetComponent<Deck>();
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
    }

    private IEnumerator DrawCardPerformer(DrawCardGA drawcardGA)
    {
        Debug.Log(drawcardGA.deckComponent.name + " is being drawn from by " + drawcardGA.player.name);
        // if (deck.transform.childCount == 0) yield return null; // No cards left to draw // HOW DO COROUTINES WORK
        Debug.Log(deck.name + " has " + drawcardGA.deckComponent.cards.Count + " cards left to draw.");
        GameObject drawnCard = drawcardGA.deckComponent.cards[0]; // Get the top card from the deck
        
        drawcardGA.deckComponent.cards.RemoveAt(0); // Remove the card from the deck's list

        drawcardGA.player.GetComponent<InstantiatePlayer>().heldCards.AddCard(drawnCard); // Add the card to the hand

        // TODO: move this to the cards playOnBase method?
        // drawnCard.transform.DOMove(drawcardGA.player.GetComponent<InstantiatePlayer>().heldCards, 0.5f); // GET THE RIGHT POSITION
        // THIS LINE ABOVE used to be the way that the cards move into position, now it is in the card script
        // drawnCard.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(0.1f); // Optional delay for drawing animation or effect
    }
}
