# Game Actions Implementation Guide

## Overview

This document describes all implemented Game Actions in the Online Card Architecture system. Each game action follows a modular pattern that requires no network code to implement or use.

## Architecture Pattern

Every game action consists of two files:

1. **`<Action>GA.cs`** - The Game Action class (stores parameters)
2. **`<Action>System.cs`** - The System class (executes the action)

### Usage Pattern (3 Lines Maximum)

```csharp
// Create the game action with parameters
SomeGameAction action = new SomeGameAction(param1, param2);

// Execute it through the ActionSystem
ActionSystem.Instance.Perform(action);
```

### Adding as a Reaction

```csharp
// As a Pre-Reaction (executes before the main action)
myAction.PreReactions.Add(reactionAction);

// As a Post-Reaction (executes after the main action)
myAction.PostReactions.Add(reactionAction);
```

## Implemented Game Actions

### 1. DrawCardGA
**Purpose**: Draw a card from a deck to a player's hand

**Parameters**:
- `Transform player` - The player drawing the card
- `Deck deckComponent` - The deck to draw from

**Example Usage**:
```csharp
DrawCardGA drawCard = new DrawCardGA(playerTransform, deckComponent);
ActionSystem.Instance.Perform(drawCard);
```

**Test Card**: Generic Draw (ID: 5)

---

### 2. PlayCardGA
**Purpose**: Play a card from hand onto a base

**Parameters**:
- `CardClick2 card` - The card being played
- `Base baseTarget` - The base to play the card on

**Example Usage**:
```csharp
PlayCardGA playCard = new PlayCardGA(cardComponent, targetBase);
ActionSystem.Instance.Perform(playCard);
```

**Test Cards**: Generic Damage (ID: 6), Generic Buff (ID: 7)

---

### 3. EndTurnGA
**Purpose**: End the current player's turn and switch to the opponent

**Parameters**:
- `ulong PlayerID` - The player ending their turn

**Example Usage**:
```csharp
EndTurnGA endTurn = new EndTurnGA(OwnerClientId);
ActionSystem.Instance.Perform(endTurn);
```

---

### 4. BeginTurnGA ⭐ NEW
**Purpose**: Begin a player's turn, triggering start-of-turn effects

**Parameters**:
- `ulong PlayerID` - The player beginning their turn

**Example Usage**:
```csharp
BeginTurnGA beginTurn = new BeginTurnGA(newPlayerID);
ActionSystem.Instance.Perform(beginTurn);
```

**Test Card**: Dawn Ritualist (ID: 13) - "At the beginning of your turn, draw a card"

**Implementation Example**:
```csharp
// In a card's ability that triggers at turn start
ActionSystem.SubscribeReaction<BeginTurnGA>(ga => 
{
    BeginTurnGA beginTurn = (BeginTurnGA)ga;
    if (beginTurn.PlayerID == ownerID)
    {
        DrawCardGA draw = new DrawCardGA(player, deck);
        ActionSystem.Instance.AddReaction(draw);
    }
}, ReactionTiming.POST);
```

---

### 5. DiscardCardGA ⭐ NEW
**Purpose**: Discard a card from hand to the discard pile

**Parameters**:
- `CardClick2 card` - The card to discard
- `Transform discardPile` - The discard pile location
- `HeldCards sourceHand` - The hand containing the card

**Example Usage**:
```csharp
DiscardCardGA discard = new DiscardCardGA(card, discardPileTransform, handComponent);
ActionSystem.Instance.Perform(discard);
```

**Test Card**: Forced Discard (ID: 8) - "Force your opponent to discard 2 cards"

**Reaction Example**:
```csharp
// Card that triggers when any card is discarded
ActionSystem.SubscribeReaction<DiscardCardGA>(ga => 
{
    Debug.Log("A card was discarded!");
    // Trigger your ability here
}, ReactionTiming.POST);
```

---

### 6. DestroyCardGA ⭐ NEW
**Purpose**: Destroy a card (typically a minion on a base)

**Parameters**:
- `CardClick2 card` - The card to destroy
- `Base sourceBase` (optional) - The base the card is on

**Example Usage**:
```csharp
DestroyCardGA destroy = new DestroyCardGA(targetCard, currentBase);
ActionSystem.Instance.Perform(destroy);
```

**Test Card**: Assassination (ID: 9) - "Destroy target minion on any base"

**Reaction Example**:
```csharp
// "Whenever a minion is destroyed, draw a card"
ActionSystem.SubscribeReaction<DestroyCardGA>(ga => 
{
    DrawCardGA draw = new DrawCardGA(player, deck);
    ActionSystem.Instance.AddReaction(draw);
}, ReactionTiming.POST);
```

---

### 7. ModifyCardGA ⭐ NEW
**Purpose**: Modify a card's stats (power, attack, defense, health)

**Parameters**:
- `CardClick2 targetCard` - The card to modify
- `int powerModifier` (default: 0) - Power modification
- `int attackModifier` (default: 0) - Attack modification
- `int defenseModifier` (default: 0) - Defense modification
- `int healthModifier` (default: 0) - Health modification

**Example Usage**:
```csharp
// Give a minion +2 power
ModifyCardGA modify = new ModifyCardGA(targetCard, powerMod: 2);
ActionSystem.Instance.Perform(modify);

// Give a minion +1 attack and +1 defense
ModifyCardGA buff = new ModifyCardGA(targetCard, attackMod: 1, defenseMod: 1);
ActionSystem.Instance.Perform(buff);
```

**Test Card**: Rally Cry (ID: 10) - "Give all your minions +2 power until end of turn"

**Bulk Modification Example**:
```csharp
// Buff all your minions
foreach (var minion in myMinions)
{
    ModifyCardGA buff = new ModifyCardGA(minion, powerMod: 2);
    myAction.PostReactions.Add(buff);
}
```

---

### 8. ActivateAbilityGA ⭐ NEW
**Purpose**: Activate a card's ability (for minions with activated abilities)

**Parameters**:
- `CardClick2 sourceCard` - The card with the ability
- `string abilityEffect` - The effect to activate
- `GameObject target` (optional) - The target of the ability

**Example Usage**:
```csharp
ActivateAbilityGA activate = new ActivateAbilityGA(minionCard, "Deal 2 damage", targetMinion);
ActionSystem.Instance.Perform(activate);
```

**Test Card**: Ember Spirit (ID: 11) - "Activate: Deal 1 damage to target minion"

**Advanced Example**:
```csharp
// Player clicks on a minion with an activated ability
if (Input.GetMouseButtonDown(0) && minionHasActivatedAbility)
{
    ActivateAbilityGA activate = new ActivateAbilityGA(
        selectedMinion, 
        selectedMinion.cardEffect, 
        clickedTarget
    );
    ActionSystem.Instance.Perform(activate);
}
```

---

### 9. EditCardGA ⭐ NEW
**Purpose**: Transform a card into a different card (change its ID)

**Parameters**:
- `CardClick2 targetCard` - The card to transform
- `int newCardID` - The ID of the card to transform into

**Example Usage**:
```csharp
// Transform a card into a Basic Minion (ID 2)
EditCardGA transform = new EditCardGA(targetCard, 2);
ActionSystem.Instance.Perform(transform);
```

**Test Card**: Polymorph (ID: 12) - "Transform target minion into a Basic Minion"

**Use Cases**:
- Polymorph effects
- Card evolution mechanics
- Card copying effects
- Temporary transformations (with duration tracking)

---

## Advanced Usage

### Chaining Actions with Reactions

```csharp
// Create a main action
PlayCardGA playCard = new PlayCardGA(card, base);

// Add reactions that execute after the card is played
DrawCardGA draw1 = new DrawCardGA(player, deck);
DrawCardGA draw2 = new DrawCardGA(player, deck);

playCard.PostReactions.Add(draw1);
playCard.PostReactions.Add(draw2);

// Execute - will play the card, then draw 2 cards
ActionSystem.Instance.Perform(playCard);
```

### Subscribing to Global Events

```csharp
// In a System's OnEnable method
void OnEnable()
{
    // Subscribe to all discard events
    ActionSystem.SubscribeReaction<DiscardCardGA>(OnCardDiscarded, ReactionTiming.POST);
}

void OnCardDiscarded(GameAction ga)
{
    DiscardCardGA discard = (DiscardCardGA)ga;
    Debug.Log($"{discard.card.name} was discarded!");
    // Implement your mechanic here
}

void OnDisable()
{
    ActionSystem.UnsubscribeReaction<DiscardCardGA>(OnCardDiscarded, ReactionTiming.POST);
}
```

### Conditional Reactions

```csharp
// Only trigger if the destroyed card belongs to the opponent
ActionSystem.SubscribeReaction<DestroyCardGA>(ga => 
{
    DestroyCardGA destroy = (DestroyCardGA)ga;
    if (destroy.card.deck.OwnerClientId != myPlayerID)
    {
        // Opponent's card was destroyed
        DrawCardGA draw = new DrawCardGA(myPlayer, myDeck);
        ActionSystem.Instance.AddReaction(draw);
    }
}, ReactionTiming.POST);
```

## Testing Cards Reference

All new game actions have test cards in `CardDataJSON.txt`:

| ID | Name | Type | Game Action Used |
|----|------|------|-----------------|
| 8 | Forced Discard | Action | DiscardCardGA |
| 9 | Assassination | Action | DestroyCardGA |
| 10 | Rally Cry | Action | ModifyCardGA |
| 11 | Ember Spirit | Minion | ActivateAbilityGA |
| 12 | Polymorph | Action | EditCardGA |
| 13 | Dawn Ritualist | Minion | BeginTurnGA |

## Best Practices

1. **Always check for null** in your performer methods
2. **Use Post-Reactions** for effects that trigger after an action completes
3. **Use Pre-Reactions** for effects that should happen before validation
4. **Keep constructors simple** - max 5 parameters for ease of use
5. **Log important actions** for debugging
6. **Subscribe/Unsubscribe properly** in OnEnable/OnDisable to prevent memory leaks

## Performance Notes

- Game actions are lightweight (< 1KB in memory)
- Turn-based nature means no performance concerns with action chaining
- Reaction chains are resolved recursively, so avoid infinite loops
- All actions execute through coroutines, allowing for animations

## Future Extensibility

To add a new game action:

1. Create `YourActionGA.cs` extending `GameAction`
2. Create `YourActionSystem.cs` extending `MonoBehaviour`
3. Implement `AttachPerformer` in OnEnable
4. Add the System component to your scene's ActionSystem GameObject
5. That's it! No network code needed.

Example template:

```csharp
// YourActionGA.cs
public class YourActionGA : GameAction
{
    public SomeType parameter { get; private set; }
    
    public YourActionGA(SomeType parameter)
    {
        this.parameter = parameter;
    }
}

// YourActionSystem.cs
public class YourActionSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<YourActionGA>(YourActionPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<YourActionGA>();
    }

    private IEnumerator YourActionPerformer(YourActionGA action)
    {
        // Your logic here
        yield return new WaitForSeconds(0.1f);
    }
}
```

## Support

For issues or questions about the game action system, refer to:
- `SECURITY_ANALYSIS.md` - Network security details
- `ActionSystem.cs` - Core implementation
- `golden_tests/GameActionTests.cs` - Validation tests
