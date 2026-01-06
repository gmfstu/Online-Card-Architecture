using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HeldCards : NetworkBehaviour // REDO THIS SCRIPT!
{
    public List<GameObject> Cards;
    public int MaxCards = 8;
    public GameObject slotPrefab;
    public float Spacing = 0.25f; // Space between cards
    public GameObject testCardToAdd;
    public GameObject SelectedCard = null;
    public Transform TargetSlot = null;
    
    void Start()
    {
        int count = transform.childCount;
        for(int i = 0; i < count; i++)
        {
            GameObject child = transform.GetChild(i).GetChild(0).gameObject;
            Cards.Add(child);
        }
    }

    public void AddCard(GameObject card)
    {
        Debug.Log("Adding card: " + card.name);
        if (!Cards.Contains(card))
        {
            Cards.Add(card);
            GameObject newSlot = Instantiate(slotPrefab, transform);
            // card.transform.SetParent(newSlot.transform);
            // card.transform.SetParent(null);
            card.GetComponent<CardClick2>().cardSlot = newSlot.transform; // Set the card's slot reference
            card.GetComponent<CardClick2>().container = CardClick2.Container.Hand; // Update the container type
            TargetSlot = newSlot.transform;
        }
        float count = Cards.Count;
        float index = 1;
        foreach (GameObject loopcard in Cards)
        {
            Vector3 destination = transform.position + new Vector3((index++ - (count / 2 + 0.5f)) * Spacing, 0, 0);
            loopcard.GetComponent<CardClick2>().cardSlot.DOMove(destination, 0.1f);

            // loopcard.GetComponent<CardClick2>().cardSlot.localPosition = destination; // Set the position of the card slot
        }
    }

    public void RemoveCard(GameObject card)
    {
        if (SelectedCard == card)
        {
            SelectedCard = null; // Deselect the card if it was selected
            card.GetComponent<CardClick2>().isSelected = false; 
        }
        
        if (Cards.Contains(card))
        {
            Cards.Remove(card);
            card.GetComponent<CardClick2>().cardSlot = null; // Clear the card's slot reference
            // card.GetComponent<CardClick2>().container = CardClick2.Container.Discard; // Update the container type
            // card.transform.SetParent(null); // or set to a different parent if needed
        }
        else {
            Debug.LogWarning("Card not found in HeldCards: " + card.name);
        }
        float count = Cards.Count;
        float index = 1;
        foreach (GameObject loopcard in Cards)
        {
            Vector3 destination = transform.position + new Vector3((index++ - (count / 2 + 0.5f)) * Spacing, 0, 0);
            loopcard.GetComponent<CardClick2>().cardSlot.DOMove(destination, 0.1f);
            // loopcard.transform.parent.position = transform.position + new Vector3((index++ - (count / 2 + 0.5f)) * Spacing, 0, 0);
        }
    }

    public void PlaySelectedOnBase(Base baseTarget)
    {
        Debug.Log("Held card is: " + SelectedCard);
        if (SelectedCard != null) {
            if (ActionSystem.Instance.isPreforming) return;
            if (ActionSystem.Instance.playerTurn.Value != OwnerClientId) return;
            if (ActionSystem.Instance.normalPlayerTurn != OwnerClientId) return; // CHANGED

            PlayCardGA playCardGA = new PlayCardGA(SelectedCard.GetComponent<CardClick2>(), baseTarget);
            ActionSystem.Instance.Perform(playCardGA);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) {
            if (Cards.Count < MaxCards)
            {
                GameObject newCard = Instantiate(testCardToAdd);
                newCard.transform.SetParent(transform);
                // newCard.transform.DOLocalMove(Vector3.zero, 0.2f); // tweening attempt shelved cus idk what this does
                newCard.transform.localPosition = Vector3.zero; // Reset position
                AddCard(newCard);
            }
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            if (SelectedCard != null)
            {
                SelectedCard.transform.localPosition = Vector3.zero; // Reset position
                SelectedCard = null;
            }
        }
        if (TargetSlot != null && transform.position != TargetSlot.position) {
            // transform.position = TargetSlot.position;
        }
    }
}
