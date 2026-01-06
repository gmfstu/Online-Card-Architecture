using UnityEngine;

public class ReferenceManager : Singleton<ReferenceManager>
{
    public GameObject playerPrefab;
    public GameObject goodPlayer;
    public GameObject evilPlayer;
    public int maxPlayers = 2;
    public InstantiatePlayer[] playerInstantiators = new InstantiatePlayer[2];
    public HeldCards[] heldCards = new HeldCards[2];
    public Deck[] decks = new Deck[2];
    
}
