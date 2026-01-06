using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    private Vector3 originalLocation;
    private float originalFOV;
    private Camera cam;
    public float speed;

    [Header("Cam Positions")]
    public Transform views;
    public List<Transform> fullBoardViews;
    public List<Transform> fullBaseViews;
    public List<Transform> narrowBaseViews;
    public List<Transform> opponentViews;
    private enum CamPositions {
        boardView,
        fullBaseView,
        narrowBaseView,
        opponentView
    };
    private CamPositions currentPosition = CamPositions.boardView;
    private int currentBase = 0;


    void Start()
    {
        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;
        originalLocation = transform.position;

        // for (int i = 0; i < views.childCount; i++) { // todo: change this when redoing camera stuff
        //     if (i < 3) {
        //         fullBoardViews[i] = views.GetChild(i);
        //     } else if (i < 6) {
        //         fullBaseViews[i - 3] = views.GetChild(i);
        //     } else if (i < 9) {
        //         narrowBaseViews[i - 6] = views.GetChild(i);
        //     } else if (i < 12) {
        //         opponentViews[i - 9] = views.GetChild(i);
        //     }
        // }

        // fullBoardView = Views.Instance.views[0, 0];
        // fullBaseView = Views.Instance.views[1, 1];
        // opponentView = Views.Instance.views[3, 2];
        // baseViews = new List<Transform> {
        //     Views.Instance.views[2, 0],
        //     Views.Instance.views[2, 1],
        //     Views.Instance.views[2, 2]
        // };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.position += speed * Time.deltaTime * transform.forward;
        }
        if (Input.GetKey(KeyCode.Tab)) {
            transform.position -= speed * Time.deltaTime * transform.forward;
        }


        if (Input.GetKeyDown(KeyCode.A)) {
            if (currentBase > 0) {
                currentBase--;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            if (currentBase < narrowBaseViews.Count - 1) {
                currentBase++;
            }
        }

        Transform targetTransform = transform;

        if (currentPosition == CamPositions.boardView) {
            targetTransform = fullBoardViews[currentBase];
            // transform.rotation = fullBoardView.rotation;
            if (Input.GetKeyDown(KeyCode.W)) {
                currentPosition = CamPositions.fullBaseView;
            }
        }
        else
        if (currentPosition == CamPositions.fullBaseView) {
            targetTransform = fullBaseViews[currentBase];
            // transform.rotation = fullBaseView.rotation;
            if (Input.GetKeyDown(KeyCode.W)) {
                currentPosition = CamPositions.narrowBaseView;
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                currentPosition = CamPositions.boardView;
            }
        } 
        else
        if (currentPosition == CamPositions.narrowBaseView) {
            targetTransform = narrowBaseViews[currentBase];
            // transform.rotation = baseViews[currentBase].rotation;
            if (Input.GetKeyDown(KeyCode.W)) {
                currentPosition = CamPositions.opponentView;
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                currentPosition = CamPositions.fullBaseView;
            }
        }
        else 
        if (currentPosition == CamPositions.opponentView) {
            targetTransform = opponentViews[currentBase];
            // transform.rotation = opponentView.rotation;
            if (Input.GetKeyDown(KeyCode.S)) {
                currentPosition = CamPositions.narrowBaseView;
            }
        }

        if (transform.position != targetTransform.position) {
            // Vector3.SmoothDamp(transform.position, targetTransform.position, ref , 0.5f); // LEFT OFF tf how do i do this
            transform.DOMove(targetTransform.position, 0.5f).SetEase(Ease.OutQuad);
            transform.DORotate(targetTransform.rotation.eulerAngles, 0.5f).SetEase(Ease.OutQuad);
        }

        //       opponent cards/health
        // base 1     base 2      base 3
        //           all bases
        //           own cards

        // cards up/down
    }
}
