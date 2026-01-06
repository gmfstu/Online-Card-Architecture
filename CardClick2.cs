using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Animations;
using System;
using TMPro;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine.AI;
using Unity.Netcode;
using DG.Tweening;

public class CardClick2 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    private bool dragging = false;
    public int cardID = 0;
    public TextMeshProUGUI cardNameText;
    // public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardPowerText;
    public TextMeshProUGUI cardDescriptionText;
    public String cardEffect;
    public GameObject Location {get; private set;} = null; 
    public enum Container {
        Deck,
        Hand,
        Base,
        Scrap,
        Discard
    }
    public Container container = Container.Deck; // Default to Deck, can be changed in the inspector
    public Transform cardSlot = null;
    [HideInInspector] public HeldCards heldCards;
    [HideInInspector] public Deck deck;
    public bool isSelected = false;

    void Awake()
    {
        heldCards = transform.parent.parent.GetComponentInChildren<HeldCards>(); // TODO: make more vars in this script accessed like this
        deck = transform.parent.GetComponent<Deck>();
    }

    public void PlayOnBase(Base baseTarget)
    {
        if (baseTarget == null) return;

        if (Location != null)
        {
            // Remove the card from the current base
            Location.GetComponent<Base>().Cards.Remove(this); // this will probably need to be edited when i do switching bases...
        }
        
        Location = baseTarget.gameObject; // Update the current base reference
        container = Container.Base; // Update the container type
        // TODO: if statement for whether there is an onplay action? or is that a reaction...?
    }

    IEnumerator LoadCardData() {
        // Wait for the JSONReader to load the card data
        while (!JSONReader.Instance.isLoaded)
        {
            yield return null;
        }

        // Find the card data by ID
        foreach (var card in JSONReader.Instance.cardDataList.carddata)
        {
            if (card.id == cardID)
            {
                // Set the card details in the UI
                cardNameText.text = card.name;
                transform.name = card.name; // Set the GameObject name to the card name
                // cardTypeText.text = card.type;
                if (card.power < 0) cardPowerText.text = "";
                else cardPowerText.text = card.power.ToString();
                cardDescriptionText.text = card.description;
                cardEffect = card.effect;
                break;
            }
        }
    }

    void Start()
    {
        StartCoroutine(LoadCardData());
        // if (transform.parent.CompareTag("Deck")) {
            // who knows what i was doing here
        // }
    }

    void Update()
    {
        if ((container == Container.Hand) && cardSlot != null)
        {
            // Update the position of the card to match the card slot
            if (!isSelected) transform.DOMove(cardSlot.position, .4f);
            else transform.DOMove(cardSlot.position + Vector3.up * 0.05f, .2f); // Raise the card slightly when selected
        }
        if ((container == Container.Base) && cardSlot != null)
        {
            // Update the position of the card to match the card slot
            transform.DOMove(cardSlot.position + cardSlot.GetComponent<Base>().playablePosition, .4f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the click is a right-click
        if (container == Container.Hand /* && eventData.button == PointerEventData.InputButton.Right */)
        {
            if (ActionSystem.Instance.isPreforming) return; // TODO: TECHDEBT: Should this be a buffer & not check every time? 
            if (ActionSystem.Instance.playerTurn.Value != deck.OwnerClientId) {
                    Debug.Log("It is not my turn, I cannot click on a card. Current player turn is: " + ActionSystem.Instance.playerTurn);
                    return;
                }
            // Ideal architecture I should be thinking about this 0% of the time
            if (heldCards.SelectedCard != gameObject)
            {
                if (heldCards.SelectedCard != null) {
                    heldCards.SelectedCard.GetComponent<CardClick2>().isSelected = false; // Deselect the previously selected card
                }
                heldCards.SelectedCard = gameObject;
                isSelected = true; // here...?
            } else {
                heldCards.SelectedCard = null;
                isSelected = false;// HERE
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right && Location != null) { // use container
            // implement playing on other minions!
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // if (Location != null) return; // Prevent dragging if the card is already on a base
        // dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // if (dragging)
        // {
        //     transform.SetParent(eventData.pointerDrag.transform.parent);
        //     Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
        //     transform.position = new Vector3(worldPosition.x, transform.parent.position.y + 0.05f, transform.position.z);
        // }
        // else
        // {
        // }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // dragging = false;
        // transform.position = transform.parent.position; // Reset position to parent
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
