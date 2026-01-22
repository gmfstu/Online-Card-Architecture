using System;
using UnityEngine;

/// <summary>
/// Enum defining which card property to edit.
/// </summary>
public enum CardProperty
{
    Name,
    Power,
    AttackMod,
    DefenseMod,
    HealthMod,
    Effect,
    Type,
    CardID // Transform the card into a different card
}

/// <summary>
/// GameAction that represents permanently editing a card's data.
/// Unlike ModifyCard which applies temporary buffs/debuffs,
/// EditCard permanently changes the card's base properties.
/// Cards can attach reactions to trigger "on edit" effects.
/// </summary>
public class EditCardGA : GameAction
{
    /// <summary>
    /// The card being edited.
    /// </summary>
    public CardClick2 Card { get; private set; }

    /// <summary>
    /// The property being edited.
    /// </summary>
    public CardProperty Property { get; private set; }

    /// <summary>
    /// The new string value (for string properties like Name, Effect, Type).
    /// </summary>
    public string StringValue { get; private set; }

    /// <summary>
    /// The new integer value (for numeric properties like Power, AttackMod, etc.).
    /// </summary>
    public int IntValue { get; private set; }

    /// <summary>
    /// The player who made the edit.
    /// </summary>
    public ulong EditorPlayerID { get; private set; }

    /// <summary>
    /// The source of the edit (e.g., card name, ability name).
    /// </summary>
    public string EditSource { get; private set; }

    /// <summary>
    /// Creates a new EditCardGA instance for editing a string property.
    /// </summary>
    /// <param name="card">The card to edit.</param>
    /// <param name="property">The property to edit.</param>
    /// <param name="stringValue">The new string value.</param>
    /// <param name="editorPlayerID">The ID of the player editing.</param>
    /// <param name="editSource">The source of the edit (default: "effect").</param>
    public EditCardGA(CardClick2 card, CardProperty property, string stringValue, ulong editorPlayerID, string editSource = "effect")
    {
        this.Card = card;
        this.Property = property;
        this.StringValue = stringValue;
        this.IntValue = 0;
        this.EditorPlayerID = editorPlayerID;
        this.EditSource = editSource;
    }

    /// <summary>
    /// Creates a new EditCardGA instance for editing an integer property.
    /// </summary>
    /// <param name="card">The card to edit.</param>
    /// <param name="property">The property to edit.</param>
    /// <param name="intValue">The new integer value.</param>
    /// <param name="editorPlayerID">The ID of the player editing.</param>
    /// <param name="editSource">The source of the edit (default: "effect").</param>
    public EditCardGA(CardClick2 card, CardProperty property, int intValue, ulong editorPlayerID, string editSource = "effect")
    {
        this.Card = card;
        this.Property = property;
        this.StringValue = null;
        this.IntValue = intValue;
        this.EditorPlayerID = editorPlayerID;
        this.EditSource = editSource;
    }
}
