using UnityEngine;

public class BeginTurnGA : GameAction
{
    public ulong PlayerID { get; private set; }

    public BeginTurnGA(ulong playerID)
    {
        this.PlayerID = playerID;
    }
}
