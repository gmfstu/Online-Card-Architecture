using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCamera : NetworkBehaviour { // currently a placeholder for the script that controls the player at large
    public GameObject cam;
    public enum ScrappedIDOLD {
        Good = 1,
        Evil = 2
    }
    public ScrappedIDOLD playerID;
    private Vector3 goodPosition = new Vector3(0, 3, 2);
    private Vector3 evilPosition = new Vector3(0, 3, -2);
    
    override public void OnNetworkSpawn() {
        if (IsOwner) {
            cam.SetActive(true);
        }

        Debug.Log(OwnerClientId);

        if (OwnerClientId % 2 == 1) { // i fear this is not the best approach cus what about spectators
            transform.position = goodPosition;
            playerID = ScrappedIDOLD.Good;
            cam.transform.localRotation = Quaternion.LookRotation(-Vector3.forward);
        } else {
            transform.position = evilPosition;
            playerID = ScrappedIDOLD.Evil;
            cam.transform.localRotation = Quaternion.LookRotation(Vector3.forward);
        }
        cam.transform.localPosition = Vector3.zero;
        cam.transform.Rotate(new Vector3(35, 0, 0));
    }
}
