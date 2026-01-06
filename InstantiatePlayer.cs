using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class InstantiatePlayer : NetworkBehaviour {
    public GameObject cam;

    public enum PlayerID {
        Good = 1,
        Evil = 2
    }
    public PlayerID playerID;
    // [HideInInspector] public InstantiatePlayer opponent; // probably do want this
    [HideInInspector] public Deck deck {get; private set;} 
    [HideInInspector] public HeldCards heldCards {get; private set;}

    override public void OnNetworkSpawn() {
        deck = GetComponentInChildren<Deck>();
        heldCards = GetComponentInChildren<HeldCards>();

        if (IsOwner) {
            cam.SetActive(true);
        }

        if (OwnerClientId % 2 == 0) { // i fear this is not the best approach cus what about spectators
            playerID = PlayerID.Good;
            ReferenceManager.Instance.goodPlayer = gameObject; 
            Debug.Log("Good player instantiated: " + gameObject.name); // MAKING THIS WORK
            ReferenceManager.Instance.playerInstantiators[0] = this; 
            ReferenceManager.Instance.heldCards[0] = heldCards;
            ReferenceManager.Instance.decks[0] = deck;
            ActionSystem.Instance.AddPlayer(OwnerClientId, gameObject); // Set the player ID in ActionSystem

        } else { // this is currently being run twice! set values, so they can be set twice.
            playerID = PlayerID.Evil;
            transform.rotation = Quaternion.Euler(0, 180, 0); // Rotate the player to face the opposite direction
            ReferenceManager.Instance.evilPlayer = gameObject; 
            ReferenceManager.Instance.playerInstantiators[1] = this; 
            ReferenceManager.Instance.heldCards[1] = heldCards;
            ReferenceManager.Instance.decks[1] = deck;
            ActionSystem.Instance.AddPlayer(OwnerClientId, gameObject); // Set the player ID in ActionSystem
        }

        Debug.Log(OwnerClientId);
    }
}
