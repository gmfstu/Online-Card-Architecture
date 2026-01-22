# Network Architecture & Security Documentation

## Overview

This document describes the network architecture and security considerations for the Online Card Architecture multiplayer game. The system is designed for small-scale peer-to-peer connections (2 players) using Unity.Netcode.

## Network Infrastructure

### Core Technology
- **Unity.Netcode**: The exclusive networking dependency used for all multiplayer functionality
- **NetworkVariable**: Used for synchronized state (e.g., `playerTurn`)
- **NetworkBehaviour**: Base class for networked components (Singleton, Base, Deck, HeldCards)

### Connection Model
- **Peer-to-Peer**: Designed for direct connections between friends
- **Maximum Players**: 2 (configured in ReferenceManager)
- **Host-Client Model**: One player hosts, the other connects as client

## Security Considerations

### Current Security Measures

1. **Turn Validation**: Actions are validated against `playerTurn` to prevent out-of-turn actions
   ```csharp
   if (ActionSystem.Instance.playerTurn.Value != OwnerClientId) return;
   ```

2. **Ownership Checks**: NetworkBehaviour components use `IsOwner` to validate local player actions
   ```csharp
   if (IsOwner) { /* Only process for owning player */ }
   ```

3. **NetworkVariable Permissions**: Turn tracking uses appropriate permissions
   ```csharp
   NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
   ```

### Security Recommendations for Future Development

1. **Server Authority**: For larger scale deployment, consider moving to a server-authoritative model
2. **Action Validation**: Validate all GameActions on the host before executing
3. **Rate Limiting**: Implement action rate limiting to prevent spam
4. **Checksum Verification**: Add checksums for card data integrity

### Data Transmission

The system transmits minimal data over the network:
- Player turn state (ulong)
- Action triggers (via RPC calls when implemented)
- Card positions and states

This lightweight approach ensures playable connections even with high latency.

## GameAction Architecture

### Overview

The GameAction system provides a modular, reactive architecture for all game mechanics. Each action follows a consistent pattern:

1. **GameAction Class** (`*GA.cs`): Defines the action data
2. **System Class** (`*System.cs`): Implements the performer logic

### Flow Execution

```
Perform(GameAction) 
    → PreReactions
    → Performer (actual logic)
    → PerformReactions
    → PostReactions
```

### Available GameActions

| GameAction | Purpose | Example Usage |
|------------|---------|---------------|
| `DrawCardGA` | Draw cards from deck | `new DrawCardGA(player, deck)` |
| `PlayCardGA` | Play a card to a base | `new PlayCardGA(card, base)` |
| `BeginTurnGA` | Start of turn triggers | `new BeginTurnGA(playerId)` |
| `EndTurnGA` | End of turn triggers | `new EndTurnGA(playerId)` |
| `DiscardCardGA` | Discard from hand | `new DiscardCardGA(card, playerId)` |
| `DestroyCardGA` | Destroy card on board | `new DestroyCardGA(card, base, playerId)` |
| `ModifyCardGA` | Temporary stat changes | `new ModifyCardGA(card, type, value, playerId)` |
| `EditCardGA` | Permanent card changes | `new EditCardGA(card, property, value, playerId)` |
| `ActivateAbilityGA` | Trigger card abilities | `new ActivateAbilityGA(card, effect, playerId)` |

### Implementing New GameActions

Each new GameAction can be implemented in **3 lines or less**:

```csharp
// Create and perform an action
var action = new MyNewGA(parameters);
ActionSystem.Instance.Perform(action);

// Or add as a reaction within another action
ActionSystem.Instance.AddReaction(new MyNewGA(parameters));
```

### Subscribing to Reactions

Cards can react to any GameAction type:

```csharp
// Subscribe to get notified before an action
ActionSystem.SubscribeReaction<DiscardCardGA>(OnCardDiscarded, ReactionTiming.PRE);

// Subscribe to get notified after an action
ActionSystem.SubscribeReaction<DestroyCardGA>(OnCardDestroyed, ReactionTiming.POST);
```

## Modularity Guidelines

### Adding New Mechanics

1. **Create GameAction Class**: Define properties needed for the action
2. **Create System Class**: Implement performer with `OnEnable`/`OnDisable` pattern
3. **Add Static Helpers**: Provide convenience methods for common usage
4. **Add to Scene**: Attach System component to a scene GameObject

### Best Practices

1. **Keep GameActions Simple**: Each action should do one thing well
2. **Use Reactions**: Complex effects should chain multiple simple actions
3. **Avoid Network Code in Actions**: Let the ActionSystem handle networking
4. **Document Effect Strings**: Define standard effect formats for ActivateAbilitySystem

## Example: Creating a New "DamagePlayerGA"

```csharp
// DamagePlayerGA.cs
public class DamagePlayerGA : GameAction
{
    public ulong TargetPlayerID { get; private set; }
    public int Damage { get; private set; }
    public string Source { get; private set; }

    public DamagePlayerGA(ulong targetPlayerID, int damage, string source = "effect")
    {
        this.TargetPlayerID = targetPlayerID;
        this.Damage = damage;
        this.Source = source;
    }
}

// DamagePlayerSystem.cs
public class DamagePlayerSystem : MonoBehaviour
{
    private void OnEnable() => ActionSystem.AttachPerformer<DamagePlayerGA>(DamagePerformer);
    private void OnDisable() => ActionSystem.DetachPerformer<DamagePlayerGA>();

    private IEnumerator DamagePerformer(DamagePlayerGA ga)
    {
        Debug.Log($"Dealing {ga.Damage} damage to player {ga.TargetPlayerID}");
        // Damage logic here
        yield return new WaitForSeconds(0.1f);
    }
}
```

## Performance Considerations

- **Lightweight Actions**: Actions contain only necessary data (no large objects)
- **Coroutine-Based**: Performers use coroutines for non-blocking execution
- **Dictionary Lookups**: O(1) performer lookup by type
- **Minimal Network Traffic**: Only state changes are synchronized

## Future Considerations

1. **Action Serialization**: For replay systems or network sync
2. **Undo System**: Track action history for undo functionality
3. **AI Integration**: Actions provide clear interface for AI decision-making
4. **Animation Hooks**: System already supports delays for visual feedback
