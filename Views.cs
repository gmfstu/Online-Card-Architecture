using UnityEngine;

public class Views : Singleton<Views>
{
    public Transform[,] views {get; private set;} = new Transform[4,3];
    public Transform deckPosition;
    public bool player1joined = false;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                views[i, j] = transform.GetChild(i * 3 + j);
            }
        }
    }
}
