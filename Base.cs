using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;

public class Base : NetworkBehaviour, IPointerClickHandler
{
    public Vector3 playablePosition;
    public List<CardClick2> Cards { get; private set; } = new();
    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("Base clicked: " + gameObject.name + " by " + NetworkManager.Singleton.LocalClientId + " which is player " + (1 + (NetworkManager.Singleton.LocalClientId % 2)));
        // GameObject selectedCard = ReferenceManager.Instance.heldCards[NetworkManager.Singleton.LocalClientId].SelectedCard;
        // Debug.Log("Selected card is: " + selectedCard);
        ReferenceManager.Instance.heldCards[NetworkManager.Singleton.LocalClientId].PlaySelectedOnBase(this); // Deselect the card
    }

    public void AddCard(CardClick2 card)
    {
        if (card != null && !Cards.Contains(card))
        {
            Cards.Add(card);
            // card.transform.SetParent(transform); // Set the parent to this base
            card.container = CardClick2.Container.Base; // Update the container type //
            card.cardSlot = transform; // Set the card's slot reference to this base
            // card.Location = gameObject; // Update the current base reference
            card.transform.DOLocalMove(playablePosition, 0.5f);
            // card.transform.localPosition = Vector3.up * transform.localScale.x * .5f; // Reset position to the base's position
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
