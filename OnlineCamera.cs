using UnityEngine;

public class OnlineCamera : MonoBehaviour
{
    public void OnAwake()
    { // I SWITCHED THESE WITH THE ONE EXCLAMATION MARK. probably not worth all caps lol
        if (Views.Instance.player1joined)
        {
            transform.position = Views.Instance.views[1, 1].position;
        }
        else 
        {
            Views.Instance.player1joined = true;
            transform.position = Views.Instance.views[0, 0].position;
            
        }

    }
}
