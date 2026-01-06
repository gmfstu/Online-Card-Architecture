using System;
using UnityEngine;

public class EndTurnGA : GameAction
{
    public ulong PlayerID { get; private set; }

    public EndTurnGA(ulong playerID)
    {
        this.PlayerID = playerID;
    }
}
