using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class CameraFocus : NetworkBehaviour
{
    [SerializeField] private float cameraReactionTimer = 2f;// Buffer before camera reacts to stop jitteriness

    [SerializeField] private float mainPlayerFocusDistance = .2f;// Distance from center of screen main this clients player can be before camera moves horizantally(Uses fraction of current screen size)

    [SerializeField] private float otherPlayerZoomOutDistance = .9f; // Distance from edge of screen other players can be before the camera zooms outs(Uses fraction of current screen size)

    [SerializeField] private float otherPlayerZoomInDistance = .6f; // Distance from edge of screen before camera zooms in(Uses fraction of current screen size)

    [SerializeField] private float zoomDistance = .05f; //Distance Camera moves when zooming every frame

    [SerializeField] private Transform thisClientsPlayer;

    [SerializeField] private float MinDistanceFromPlayer = 8;
    [SerializeField] private float MaxDistanceFromPlayer = 15;


    private List<Transform> otherPlayersTransform = new List<Transform>();
    private Camera cam;
    private bool mainPlayerIsOutOfCenter = false;
    private bool mainPlayerStopInvoking = false;
    private int MPOOCcaseNumber = -1;

    private int OPZoomCaseNumber;
    private Transform OPThatNeedsZoom = null;
    private bool otherPlayerNeedsZoomOut;
    private bool otherPlayerZoomOutStopInvoking = false;
    private bool otherPlayerNeedsZoomIn;
    private bool otherPlayerZoomInStopInvoking = false;


    private Vector3 playerOffest;
    private Vector3 previousMainPlayerScreenPosition;

    private float MPHorizontalFD;
    private float MPVerticalFD;

    private float OPZoomOutDHorizontal;
    private float OPZoomOutDVertical;
    private float OPZoomInDHorizontal;
    private float OPZoomInDVertical;

    private float OPZoomInOutMiddleVertical;

    private float OPZoomInOutMiddleHorizontal;

    private float timer;
    public bool runCheck = true;
    [ClientCallback]
    void Start()
    {
        if (!hasAuthority) { return; }
        cam = Camera.main;
        Vector3 transfer = new Vector3(thisClientsPlayer.transform.position.x, thisClientsPlayer.transform.position.y + 10, cam.transform.position.z);
        cam.transform.position = transfer;

        FindPlayers();

        MPHorizontalFD = mainPlayerFocusDistance * Screen.width;
        MPVerticalFD = mainPlayerFocusDistance * Screen.height;
        OPZoomOutDHorizontal = otherPlayerZoomOutDistance * Screen.width;
        OPZoomOutDVertical = otherPlayerZoomOutDistance * Screen.height;
        OPZoomInDHorizontal = otherPlayerZoomInDistance * Screen.width;
        OPZoomInDVertical = otherPlayerZoomInDistance * Screen.height;

        OPZoomInOutMiddleVertical = (OPZoomInDVertical + OPZoomOutDVertical) / 2;
        OPZoomInOutMiddleHorizontal = (OPZoomInDHorizontal + OPZoomOutDHorizontal) / 2;
        
    }
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority || !runCheck) { return; }
        timer += Time.deltaTime;
        if(timer > 3)
        {
            timer = 0;
            FindPlayers();
        }
        Vector2 mainPlayerScreenPostion = cam.WorldToScreenPoint(thisClientsPlayer.position);
        if (mainPlayerIsOutOfCenter)
        {
            Debug.Log("check for player");
            MoveCameraCheck(MPOOCcaseNumber);
        }
        if (!mainPlayerIsOutOfCenter && (mainPlayerScreenPostion.x > (Screen.width / 2 + MPHorizontalFD) ||
            mainPlayerScreenPostion.x < (Screen.width / 2 - MPHorizontalFD) ||
            mainPlayerScreenPostion.y > (Screen.height / 2 + MPVerticalFD) ||
            mainPlayerScreenPostion.y < (Screen.height / 2 - MPVerticalFD) ))

        {  
            if (!mainPlayerStopInvoking) { Invoke(nameof(ReactionTimeCheckMoveCamera), cameraReactionTimer);
                Debug.Log("StartInvoke");
                mainPlayerStopInvoking = true;
            }
        }
        if (otherPlayerNeedsZoomOut && Vector3.Distance(cam.transform.position, thisClientsPlayer.transform.position) < MaxDistanceFromPlayer)
        {
            ZoomCamera(0);
        }
        if (otherPlayerNeedsZoomIn && Vector3.Distance(cam.transform.position, thisClientsPlayer.transform.position) > MinDistanceFromPlayer)
        {
            ZoomCamera(1);
        }
        if (!otherPlayerNeedsZoomOut && Vector3.Distance(cam.transform.position, thisClientsPlayer.transform.position) < MaxDistanceFromPlayer)
        {
            if(otherPlayersTransform != null)
            {
                foreach (Transform otherPlayerTransform in otherPlayersTransform)
                {
                    Vector2 screenPointPosition = cam.WorldToScreenPoint(otherPlayerTransform.position);
                    if (screenPointPosition.x > Screen.width / 2 + OPZoomOutDHorizontal ||
                        screenPointPosition.x < Screen.width / 2 - OPZoomOutDHorizontal ||
                        screenPointPosition.y > Screen.height / 2 + OPZoomOutDVertical ||
                        screenPointPosition.y < Screen.height / 2 - OPZoomOutDVertical)
                    {
                        if (!otherPlayerZoomOutStopInvoking) { Invoke(nameof(ReactionTimeCheckZoomOutCamera), cameraReactionTimer); }
                        otherPlayerZoomOutStopInvoking = true;
                        OPThatNeedsZoom = otherPlayerTransform;
                        break;
                    }
                    else
                    {
                        otherPlayerNeedsZoomOut = false;
                    }
                }
            }           
        }
        if (!otherPlayerNeedsZoomIn && Vector3.Distance(cam.transform.position, thisClientsPlayer.transform.position) > MinDistanceFromPlayer)
        {
            if(otherPlayersTransform != null)
            {
                foreach (Transform otherPlayerTransform in otherPlayersTransform)
                {
                    Vector2 screenPointPosition = cam.WorldToScreenPoint(otherPlayerTransform.position);
                    if (screenPointPosition.x > Screen.width / 2 + OPZoomInDHorizontal ||
                        screenPointPosition.x < Screen.width / 2 - OPZoomInDHorizontal ||
                        screenPointPosition.y > Screen.height / 2 + OPZoomInDVertical ||
                        screenPointPosition.y < Screen.height / 2 - OPZoomInDVertical)
                    {
                        otherPlayerNeedsZoomIn = false;
                        break;
                    }
                    else
                    {
                        if (!otherPlayerZoomInStopInvoking) { Invoke(nameof(ReactionTimeCheckZoomInCamera), cameraReactionTimer); }
                        otherPlayerZoomInStopInvoking = true;
                        OPThatNeedsZoom = otherPlayerTransform;
                    }
                }
            }         
        }
    }
    [Client]
    private void MoveCameraCheck(int caseNumber)
    {
        Debug.Log(caseNumber);
        Vector3 CurrentScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
        Vector2 mainPlayerScreenPostion = cam.WorldToScreenPoint(thisClientsPlayer.position);
        switch (caseNumber)
        {

            case 0:
                if (!(CurrentScreenPosition.x < previousMainPlayerScreenPosition.x))
                {
                    MoveCamera();
                }
                else
                {
                    if(!(mainPlayerScreenPostion.x > (Screen.width / 2 + MPHorizontalFD)))
                    {
                        mainPlayerIsOutOfCenter = false;
                        Debug.Log("incenter");
                    }
                    playerOffest = new Vector3(cam.transform.position.x - thisClientsPlayer.transform.position.x,
                    cam.transform.position.y - thisClientsPlayer.transform.position.y,
                    cam.transform.position.z);
                    previousMainPlayerScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
                    return;
                }
                break;
            case 1:
                if (!(CurrentScreenPosition.x > previousMainPlayerScreenPosition.x))
                {
                    MoveCamera();
                }
                else
                {
                    if (!(mainPlayerScreenPostion.x < (Screen.width / 2 - MPHorizontalFD)))
                    {
                        mainPlayerIsOutOfCenter = false;
                        Debug.Log("incenter");
                    }
                    playerOffest = new Vector3(cam.transform.position.x - thisClientsPlayer.transform.position.x,
                    cam.transform.position.y - thisClientsPlayer.transform.position.y,
                    cam.transform.position.z);
                    previousMainPlayerScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
                    return;
                }
                break;
            case 2:
                if (!(CurrentScreenPosition.y < previousMainPlayerScreenPosition.y))
                {
                    MoveCamera();
                }
                else
                {
                    if (!(mainPlayerScreenPostion.y > (Screen.height / 2 + MPVerticalFD)))
                    {
                        mainPlayerIsOutOfCenter = false;
                    }
                    playerOffest = new Vector3(cam.transform.position.x - thisClientsPlayer.transform.position.x,
                    cam.transform.position.y - thisClientsPlayer.transform.position.y,
                    cam.transform.position.z);
                    previousMainPlayerScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
                    return;
                }
                break;
            case 3:
                if (!(CurrentScreenPosition.y > previousMainPlayerScreenPosition.y))
                {
                    MoveCamera();
                }
                else
                {
                    if (!(mainPlayerScreenPostion.y < (Screen.height / 2 - MPVerticalFD)))
                    {
                        mainPlayerIsOutOfCenter = false;
                    }
                    playerOffest = new Vector3(cam.transform.position.x - thisClientsPlayer.transform.position.x,
                    cam.transform.position.y - thisClientsPlayer.transform.position.y,
                    cam.transform.position.z);
                    previousMainPlayerScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
                    return;
                }
                break;
        }
    }
    [Client]
    private void MoveCamera()
    {
        Debug.Log("Mves camera");
        Vector3 newCamPosition = new Vector3(thisClientsPlayer.transform.position.x + playerOffest.x,
            thisClientsPlayer.transform.position.y + playerOffest.y, 
            cam.transform.position.z);
        cam.transform.position = newCamPosition;

    }
    [Client]
    private void ReactionTimeCheckMoveCamera()
    {
        Vector2 mainPlayerScreenPostion = cam.WorldToScreenPoint(thisClientsPlayer.position);
        if (mainPlayerScreenPostion.x > (Screen.width / 2 + MPHorizontalFD) ||
            mainPlayerScreenPostion.x < (Screen.width / 2 - MPHorizontalFD) ||
            mainPlayerScreenPostion.y > (Screen.height / 2 + MPVerticalFD) ||
            mainPlayerScreenPostion.y < (Screen.height / 2 - MPVerticalFD))
        {
            Debug.Log("Detects out of center");
            if (mainPlayerScreenPostion.x > (Screen.width / 2 + MPHorizontalFD))
            {
                MPOOCcaseNumber = 0;
            }
            if (mainPlayerScreenPostion.x < (Screen.width / 2 - MPHorizontalFD))
            {
                MPOOCcaseNumber = 1;
            }
            if (mainPlayerScreenPostion.y > (Screen.height / 2 + MPVerticalFD))
            {
                MPOOCcaseNumber = 2;
            }
            if (mainPlayerScreenPostion.y < (Screen.height / 2 - MPVerticalFD))
            {
                MPOOCcaseNumber = 3;
            }
            playerOffest = new Vector3(cam.transform.position.x - thisClientsPlayer.transform.position.x,
            cam.transform.position.y - thisClientsPlayer.transform.position.y,
            cam.transform.position.z);
            mainPlayerIsOutOfCenter = true;
            previousMainPlayerScreenPosition = cam.WorldToScreenPoint(thisClientsPlayer.transform.position);
        }
        mainPlayerStopInvoking = false;
    }
    [Client]
    private void ReactionTimeCheckZoomOutCamera()
    {
        Vector2 screenPointPosition = cam.WorldToScreenPoint(OPThatNeedsZoom.transform.position);
        if (screenPointPosition.x > Screen.width / 2 + OPZoomOutDHorizontal ||
            screenPointPosition.x < Screen.width / 2 - OPZoomOutDHorizontal ||
            screenPointPosition.y > Screen.height / 2 + OPZoomOutDVertical ||
            screenPointPosition.y < Screen.height / 2 - OPZoomOutDVertical
        )
        {
            if(screenPointPosition.x > Screen.width / 2 + OPZoomOutDHorizontal)
            {
                OPZoomCaseNumber = 0;
            }
            else if (screenPointPosition.x < Screen.width / 2 - OPZoomOutDHorizontal)
            {
                OPZoomCaseNumber = 1;
            }
            else if (screenPointPosition.y > Screen.height / 2 + OPZoomOutDVertical)
            {
                OPZoomCaseNumber = 2;
            }
            else if (screenPointPosition.y < Screen.height / 2 - OPZoomOutDVertical)
            {
                OPZoomCaseNumber = 3;
            }
            otherPlayerNeedsZoomOut = true;
        }
            otherPlayerZoomOutStopInvoking = false;      
    }
    [Client]
    private void ReactionTimeCheckZoomInCamera()
    {
        foreach (Transform otherPlayerTransform in otherPlayersTransform)
        {
            Vector2 screenPointPosition = cam.WorldToScreenPoint(otherPlayerTransform.position);
            if (screenPointPosition.x > Screen.width / 2 + OPZoomInDHorizontal ||
                screenPointPosition.x < Screen.width / 2 - OPZoomInDHorizontal ||
                screenPointPosition.y > Screen.height / 2 + OPZoomInDVertical ||
                screenPointPosition.y < Screen.height / 2 - OPZoomInDVertical)
            {
                otherPlayerNeedsZoomIn = false;
                otherPlayerZoomInStopInvoking = false;
                return;
            }
            else
            {
                otherPlayerNeedsZoomIn = true;
            }
        }
    }
    [Client]
    private void ZoomCamera(int thisCase)
    {
        Vector3 OPScreenPosition = cam.WorldToScreenPoint(OPThatNeedsZoom.transform.position);
        float objectErrorDistance = 20;
        switch (thisCase)
        {
            case 0:
                switch (OPZoomCaseNumber)
                {
                    case 0:
                        if (OPScreenPosition.x > (Screen.width/2 + OPZoomInOutMiddleHorizontal - objectErrorDistance) || 
                            OPScreenPosition.x < (Screen.width / 2 + OPZoomInOutMiddleHorizontal + objectErrorDistance))
                        {
                            otherPlayerNeedsZoomOut = false;
                        }
                        cam.transform.position = cam.transform.position + (-cam.transform.forward * zoomDistance * Time.deltaTime);
                        break;
                    case 1:
                        if (OPScreenPosition.x > (Screen.width / 2 - OPZoomInOutMiddleHorizontal - objectErrorDistance) ||
                        OPScreenPosition.x < (Screen.width / 2 - OPZoomInOutMiddleHorizontal + objectErrorDistance))
                        {
                            otherPlayerNeedsZoomOut = false;
                        }
                        cam.transform.position = cam.transform.position + (-cam.transform.forward * zoomDistance * Time.deltaTime);
                        break;
                    case 2:
                        if (OPScreenPosition.x > (Screen.height / 2 + OPZoomInOutMiddleVertical - objectErrorDistance) ||
                        OPScreenPosition.x < (Screen.height / 2 + OPZoomInOutMiddleVertical + objectErrorDistance))
                        {
                            otherPlayerNeedsZoomOut = false;
                        }
                        cam.transform.position = cam.transform.position + (-cam.transform.forward * zoomDistance * Time.deltaTime);
                        break;
                    case 3:
                        if (OPScreenPosition.x > (Screen.height / 2 - OPZoomInOutMiddleVertical - objectErrorDistance) ||
                        OPScreenPosition.x < (Screen.height / 2 - OPZoomInOutMiddleVertical + objectErrorDistance))
                        {
                            otherPlayerNeedsZoomOut = false;
                        }
                        cam.transform.position = cam.transform.position + (-cam.transform.forward * zoomDistance * Time.deltaTime);
                        break;
                }
                break;
            case 1:
                foreach (Transform otherPlayerTransform in otherPlayersTransform)
                {
                    Vector2 screenPointPosition = cam.WorldToScreenPoint(otherPlayerTransform.position);
                    if (screenPointPosition.x > Screen.width / 2 + OPZoomInDHorizontal ||
                        screenPointPosition.x < Screen.width / 2 - OPZoomInDHorizontal ||
                        screenPointPosition.y > Screen.height / 2 + OPZoomInDVertical ||
                        screenPointPosition.y < Screen.height / 2 - OPZoomInDVertical)
                    {
                        otherPlayerNeedsZoomIn = false;
                        return;
                    }
                    else
                    {
                        cam.transform.position = cam.transform.position + (cam.transform.forward * zoomDistance * Time.deltaTime);
                    }
                }
                break;
        }
    }
    
    public void FindPlayers()
    {
        Debug.Log("Check");
        if (!hasAuthority) { return; }
        GameObject[] allPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in allPlayerObjects)
        {
            if (otherPlayersTransform.Contains(player.transform)) { Debug.Log("Vonyinr"); continue; }
            else if (player.transform != thisClientsPlayer.transform)
            {
                otherPlayersTransform.Add(player.transform);
            }           
        }
        Debug.Log(otherPlayersTransform);
    }
}