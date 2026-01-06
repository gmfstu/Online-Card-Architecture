using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
// using Random = UnityEngine.Random;

public class Deck : NetworkBehaviour // TODO: make this a singleton... its best for everyone. // NO IT ISNT
{
    public List<GameObject> cards = new();
    public GameObject cardPrefab;
    public HeldCards heldCards;
    public int deckSize = 30;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(generateDeck());
    }

    private IEnumerator generateDeck() {
        while (!JSONReader.Instance.isLoaded)
        {
            yield return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            cards.Add(transform.GetChild(i).gameObject); // Get the cards from the HeldCards component
        }

        // while ()

        // for (int i = 0; i < deckSize; i++)
        // {
        //     GameObject card = Instantiate(cardPrefab, transform.position + Vector3.up * i * 0.01f, Quaternion.identity, transform);
        //     // card.GetComponent<NetworkObject>().Spawn();
        //     int cardIndex = UnityEngine.Random.Range(2, JSONReader.Instance.cardDataList.carddata.Length); // TODO: LET HOLLY FIX THIS
        //     card.GetComponent<CardClick2>().cardID = cardIndex; // Assuming cardID is set to the index for simplicity
        //     cards.Add(card);
        // }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("I am player " + (OwnerClientId + 1) + " and I pressed space to draw a card.");
                // shutup ai slop
                if (ActionSystem.Instance.isPreforming) return;
                // if (ActionSystem.Instance.playerTurn.Value != OwnerClientId) { // CHANGED
                if (ActionSystem.Instance.normalPlayerTurn != OwnerClientId) {
                    Debug.Log("It is not my turn, I cannot draw a card. Current player ID turn is: " + ActionSystem.Instance.playerTurn + " and my ID is: " + OwnerClientId);
                    return;
                }
                DrawCardGA drawCardGA = new(transform.parent, this);
                ActionSystem.Instance.Perform(drawCardGA);
                Debug.Log("Drawing card 1: In Deck.cs");
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                if (ActionSystem.Instance.isPreforming) return;
                // if (ActionSystem.Instance.playerTurn.Value != OwnerClientId) { // CHANGED
                if (ActionSystem.Instance.normalPlayerTurn != OwnerClientId) {
                    Debug.Log("It is not my turn, I cannot end the turn.");
                    return;
                }
                EndTurnGA endTurnGA = new(OwnerClientId);
                ActionSystem.Instance.Perform(endTurnGA);
            }
        }
    }
}
