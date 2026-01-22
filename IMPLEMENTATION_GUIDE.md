# Online Card Architecture - Implementation Guide

## Architecture Overview

This Unity multiplayer card game uses a secure, modular Game Action system built on Unity.Netcode for peer-to-peer networking.

### Security Assessment

#### ✅ Security Features
1. **Unity.Netcode Only**: All networking uses Unity.Netcode exclusively - no third-party networking libraries
2. **No Direct RPCs in Game Actions**: All GA (Game Action) files are RPC-free, channeling logic through ActionSystem
3. **State Synchronization**: NetworkVariable<ulong> handles turn state across clients
4. **Ownership Validation**: Turn checks ensure only the current player can perform actions
5. **Lightweight Data**: Only essential game state (player IDs, turn tracking) is transmitted

#### Network Architecture
- **Two-Player Limit**: System enforces exactly 2 players (Good/Evil)
- **Turn-Based**: ActionSystem manages turn flow with NetworkVariable synchronization
- **Client Authority**: Players validate their own actions; ActionSystem coordinates
- **Minimal Bandwidth**: Turn-based nature requires minimal network traffic

### Modularity

The Game Action system provides complete modularity for adding new mechanics:

```
GameAction (Abstract Base)
    ├── PreReactions (List)
    ├── PerformReactions (List)
    └── PostReactions (List)

ActionSystem (Singleton)
    ├── Performer Registry (Dictionary<Type, Func<GameAction, IEnumerator>>)
    ├── Subscriber Registry (Pre/Post)
    └── Flow Management
```

## Implemented Game Actions

### 1. BeginTurnGA
**Purpose**: Triggers at the start of a player's turn
**Usage**:
```csharp
BeginTurnGA beginTurn = new BeginTurnGA(playerID);
ActionSystem.Instance.Perform(beginTurn);
```

### 2. EndTurnGA (Existing)
**Purpose**: Ends current player's turn and switches to opponent
**Usage**:
```csharp
EndTurnGA endTurn = new EndTurnGA(playerID);
ActionSystem.Instance.Perform(endTurn);
```

### 3. DrawCardGA (Existing)
**Purpose**: Draws a card from deck to hand
**Usage**:
```csharp
DrawCardGA drawCard = new DrawCardGA(playerTransform, deckComponent);
ActionSystem.Instance.Perform(drawCard);
```

### 4. PlayCardGA (Existing)
**Purpose**: Plays a card from hand onto a base
**Usage**:
```csharp
PlayCardGA playCard = new PlayCardGA(cardComponent, targetBase);
ActionSystem.Instance.Perform(playCard);
```

### 5. DiscardCardGA
**Purpose**: Moves a card from hand or base to discard pile
**Usage**:
```csharp
DiscardCardGA discardCard = new DiscardCardGA(cardComponent, playerTransform);
ActionSystem.Instance.Perform(discardCard);
```

### 6. DestroyCardGA
**Purpose**: Permanently removes a card from the game
**Usage**:
```csharp
DestroyCardGA destroyCard = new DestroyCardGA(cardComponent);
ActionSystem.Instance.Perform(destroyCard);
```

### 7. ModifyCardGA
**Purpose**: Changes card stats (power, attack, defense, health)
**Usage**:
```csharp
// Give a minion +3 power
ModifyCardGA modifyCard = new ModifyCardGA(cardComponent, powerMod: 3);
ActionSystem.Instance.Perform(modifyCard);
```

### 8. ActivateAbilityGA
**Purpose**: Triggers a card's activated ability
**Usage**:
```csharp
ActivateAbilityGA activateAbility = new ActivateAbilityGA(cardComponent, "DefenseBuff");
ActionSystem.Instance.Perform(activateAbility);
```

### 9. EditCardGA
**Purpose**: Changes card identity/properties (transformation effects)
**Usage**:
```csharp
// Transform a card into a Basic Minion
EditCardGA editCard = new EditCardGA(cardComponent, newCardID: 2);
ActionSystem.Instance.Perform(editCard);
```

## How to Add New Game Actions

### Step 1: Create GameAction Class
```csharp
using UnityEngine;

public class MyNewGA : GameAction
{
    public CardClick2 TargetCard { get; private set; }
    public int SomeParameter { get; private set; }

    public MyNewGA(CardClick2 targetCard, int someParameter)
    {
        this.TargetCard = targetCard;
        this.SomeParameter = someParameter;
    }
}
```

### Step 2: Create System Class
```csharp
using System.Collections;
using UnityEngine;

public class MyNewSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<MyNewGA>(MyNewPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<MyNewGA>();
    }

    private IEnumerator MyNewPerformer(MyNewGA myNewGA)
    {
        // Implement your game logic here
        Debug.Log($"Performing action on {myNewGA.TargetCard.name}");
        
        // Do the actual work
        // ...
        
        yield return new WaitForSeconds(0.2f);
    }
}
```

### Step 3: Use in Game Code
```csharp
// Create the action
MyNewGA action = new MyNewGA(targetCard, 5);

// Perform it (3 lines or less!)
ActionSystem.Instance.Perform(action);
```

## Reaction System

Game Actions support reactions at three timing points:

### Adding Reactions
```csharp
DrawCardGA drawCard = new DrawCardGA(player, deck);
ModifyCardGA buffCard = new ModifyCardGA(someCard, powerMod: 1);

// Add buff as a POST reaction to drawing
drawCard.PostReactions.Add(buffCard);

// Now when drawCard performs, buffCard triggers after
ActionSystem.Instance.Perform(drawCard);
```

### Subscribing to Actions
```csharp
// Subscribe to all DiscardCard actions
ActionSystem.SubscribeReaction<DiscardCardGA>(
    (ga) => {
        Debug.Log("A card was discarded!");
        // Trigger other effects
    },
    ReactionTiming.POST
);
```

## Example Card Implementations

### Turn Starter Minion (ID: 8)
```csharp
// In BeginTurnSystem or a card effect handler
if (cardEffect == "BeginTurn: Draw 1 card")
{
    DrawCardGA drawCard = new DrawCardGA(playerTransform, deck);
    ActionSystem.Instance.Perform(drawCard);
}
```

### Forced Discard Action (ID: 9)
```csharp
// Get opponent's hand
HeldCards opponentHand = GetOpponentHand();
if (opponentHand.Cards.Count > 0)
{
    CardClick2 cardToDiscard = opponentHand.Cards[0];
    DiscardCardGA discard = new DiscardCardGA(cardToDiscard, opponentTransform);
    ActionSystem.Instance.Perform(discard);
}
```

### Assassin Action (ID: 10)
```csharp
// Target selection logic here
CardClick2 targetMinion = GetTargetMinion();
DestroyCardGA destroy = new DestroyCardGA(targetMinion);
ActionSystem.Instance.Perform(destroy);
```

### Power Surge Action (ID: 11)
```csharp
CardClick2 targetMinion = GetTargetMinion();
ModifyCardGA powerUp = new ModifyCardGA(targetMinion, powerMod: 3);
ActionSystem.Instance.Perform(powerUp);
```

### Reactive Defender Ability (ID: 12)
```csharp
// When player activates the ability
ActivateAbilityGA activate = new ActivateAbilityGA(defenderCard, "DefenseBuff");
ModifyCardGA gainDefense = new ModifyCardGA(defenderCard, defenseMod: 2);
activate.PostReactions.Add(gainDefense);
ActionSystem.Instance.Perform(activate);
```

### Shapeshifter Action (ID: 13)
```csharp
CardClick2 targetMinion = GetTargetMinion();
EditCardGA transform = new EditCardGA(targetMinion, newCardID: 2); // Transform to Basic Minion
ActionSystem.Instance.Perform(transform);
```

## Best Practices

### ✅ DO:
1. Keep Game Action classes simple data containers
2. Put all logic in System classes
3. Use ActionSystem.Instance.Perform() to execute actions
4. Validate player ownership before creating actions
5. Use reactions for triggered effects
6. Keep performer coroutines lightweight

### ❌ DON'T:
1. Add ServerRpc/ClientRpc to GA files
2. Put game logic in GameAction constructors
3. Directly manipulate game state without going through ActionSystem
4. Create circular reaction chains
5. Block the main thread in performers

## Testing

To test new Game Actions in Unity:
1. Add the System script to a GameObject in your scene
2. The OnEnable will register the performer automatically
3. Create and Perform the action from your game code
4. Check Debug.Log output to verify execution

Example test cards have been added to CardDataJSON.txt (IDs 8-13) for testing each new Game Action.

## Future Enhancements

Possible additions while maintaining architecture:
- **CombatGA**: Handle minion vs minion combat
- **ScoreBaseGA**: Calculate and award base scores
- **ShuffleDeckGA**: Shuffle discard pile back into deck
- **RevealCardGA**: Show hidden information to players
- **SwapCardGA**: Exchange cards between zones
- **CopyCardGA**: Create duplicates of existing cards

Each follows the same 3-line implementation pattern!

## Architecture Benefits

1. **No Network Code in Gameplay**: Developers never write RPCs
2. **Easy Testing**: Actions work identically in single-player
3. **Reusable Effects**: Compose complex behaviors from simple actions
4. **Type Safety**: Compile-time checking for action parameters
5. **Debugging**: Clear execution flow through ActionSystem.Flow()
6. **Extensible**: Add mechanics without touching core systems

---

**For questions or issues**: Review the existing GA/System pairs as reference implementations.
