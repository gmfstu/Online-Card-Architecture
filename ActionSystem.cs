using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework.Internal;
using Unity.Netcode;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using UnityEngine;

public enum ReactionTiming
{
    PRE,
    POST
}
public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool isPreforming {get; private set;} = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private Dictionary<ulong, GameObject> players = new();
    public NetworkVariable<ulong> playerTurn {get; private set;} = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public ulong normalPlayerTurn {get; private set;} = ulong.MaxValue; // For testing purposes, not used in production
    private ulong goodPlayerId = ulong.MaxValue;
    private ulong evilPlayerId = ulong.MaxValue;

    public void AddPlayer(ulong playerId, GameObject playerObject)
    {
        if (goodPlayerId == ulong.MaxValue) // change if ever multiplayer!
        {
            goodPlayerId = playerId;
        } else {
            if (evilPlayerId == ulong.MaxValue)
            {
                evilPlayerId = playerId;
            } else {
                Debug.LogWarning("Both players are already set. Cannot add another player.");
                return;
            }
        }

        if (!players.ContainsKey(playerId))
        {
            players.Add(playerId, playerObject);
        }
        else
        {
            Debug.LogWarning($"Player with ID {playerId} already exists. Updating the player object.");
            players[playerId] = playerObject; // Update the player object if it already exists
        }

        Debug.Log("One ----------- " + goodPlayerId);
        Debug.Log("Two ----------- " + playerTurn);
        // playerTurn.Value = goodPlayerId;
        normalPlayerTurn = goodPlayerId; 

    }
    
    // [ServerRpc(RequireOwnership = false)]
    // public void EndTurnRPC() {
    //     Debug.Log("RPC PLEASE WORK");
    //     if (playerTurn.Value == goodPlayerId)
    //     {
    //         playerTurn.Value = evilPlayerId;
    //         Debug.Log("Ending turn for Good player, switching to Evil player.");
    //     } else if (playerTurn.Value == evilPlayerId)
    //     {
    //         playerTurn.Value = goodPlayerId;
    //         Debug.Log("Ending turn for Evil player, switching to Good player.");
    //     } else {
    //         Debug.LogWarning("Player turn is not set to a valid player ID.");
    //     }
    // }

    public void EndTurn() {
        Debug.Log(playerTurn.Value + " is the current player turn");
        Debug.Log("WHY CANT YOU HEAR ME!!!");
        if (playerTurn.Value == goodPlayerId)
        {
            playerTurn.Value = evilPlayerId;
            normalPlayerTurn = evilPlayerId; // Update the normal player turn for testing
            Debug.Log("Ending turn for Good player, switching to Evil player.");
        } else if (playerTurn.Value == evilPlayerId)
        {
            playerTurn.Value = goodPlayerId;
            normalPlayerTurn = goodPlayerId; // Update the normal player turn for testing
            Debug.Log("Ending turn for Evil player, switching to Good player.");
        } else {
            Debug.LogWarning("Player turn is not set to a valid player ID.");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddReaction(GameAction gameAction) {
        reactions?.Add(gameAction);
    }

    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        if (isPreforming) return;
        isPreforming = true;
        StartCoroutine(Flow(action, () =>
        {
            isPreforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    private IEnumerator Flow(GameAction action, System.Action OnPerformFinished = null)
    {
        Debug.Log("Step 3: In ActionSystem.cs, in flow, pre reactions beginning");
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        Debug.Log("Step 4: In ActionSystem.cs, reactions over, beginning performer");
        reactions = action.PreformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        Debug.Log("Step 5: In ActionSystem.cs, perfermer over, beginning post reactions");
        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnPerformFinished?.Invoke();
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            Debug.Log("Player 2 card should be drawing, the performers dictionary contains: " + performers[type](action));
            yield return performers[type](action);
        }
        //  
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type))
        {
            performers[type] = wrappedPerformer;
        }
        else
        {
            performers.Add(type, wrappedPerformer);
        }
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type))
        {
            performers.Remove(type);
        }
    }

    public static void SubscribeReaction<T>(Action<GameAction> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs; 
        void wrappedReaction(GameAction action) => reaction(action);
        {
            if (subs.ContainsKey(typeof(T)))
            {
                subs[typeof(T)].Add(wrappedReaction);
            }
            else
            {
                subs.Add(typeof(T), new());
                subs[typeof(T)].Add(wrappedReaction);
            }
        }
    }

    public static void UnsubscribeReaction<T>(Action<GameAction> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction(action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}
