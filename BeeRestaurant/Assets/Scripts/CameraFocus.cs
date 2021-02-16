using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CameraFocus : NetworkBehaviour
{
    [SerializeField] private NetworkManager NM = null;
    [SerializeField] private float cameraReactionTimer = .5f;// Buffer before camera reacts to stop jitteriness
    [SerializeField] private float mainPlayerFocusDistance;// Distance from center of screen main this clients player can be before camera moves
    [SerializeField] private float otherPlayerZoomOutDistance; // Distance from edge of screen other players can be before the camera zooms outs
    [SerializeField] private float otherPlayerZoomInDistance; // Distance from edge of screen before camera zooms in
    [SerializeField] private Transform thisClientsPlayer;
    private List<Transform> otherPlayersTransform;
    private Camera cam;
    private bool mainPlayerIsOutOfCenter = false;
    private Vector3 playerOffest;

    [ClientCallback]
    void Start()
    {
        cam = Camera.main;
        FindPlayers();
    }

    public override void OnStartAuthority()
    {
        
    }
    
    [ClientCallback]
    void Update()
    {

        Vector2 mainPlayerScreenPostion = cam.WorldToViewportPoint(thisClientsPlayer.position);
        if (mainPlayerIsOutOfCenter)
        {
            MoveCamera();
        }
        if (mainPlayerScreenPostion.x > mainPlayerFocusDistance ||
            mainPlayerScreenPostion.x < mainPlayerFocusDistance ||
            mainPlayerScreenPostion.y > mainPlayerFocusDistance ||
            mainPlayerScreenPostion.y < mainPlayerFocusDistance
            )
        {
            if (!mainPlayerIsOutOfCenter) { Invoke(nameof(ReactionTimeCheck), cameraReactionTimer); }           
        }
        else
        {
            mainPlayerIsOutOfCenter = false;
        }

        foreach (Transform otherPlayerTransform in otherPlayersTransform)
        {
                Vector2 screenPointPosition = cam.WorldToViewportPoint(otherPlayerTransform.position);                
        }
    }

    [Client]
    private void MoveCamera()
    {
        
    }
    [Client]
    private void ReactionTimeCheck()
    {
        Vector2 mainPlayerScreenPostion = cam.WorldToViewportPoint(thisClientsPlayer.position);
        if (mainPlayerScreenPostion.x > mainPlayerFocusDistance ||
        mainPlayerScreenPostion.x < mainPlayerFocusDistance ||
        mainPlayerScreenPostion.y > mainPlayerFocusDistance ||
        mainPlayerScreenPostion.y < mainPlayerFocusDistance)        
        {
            playerOffest = new Vector3;
            mainPlayerIsOutOfCenter = true; 
        }
    }
    [Client]
    private void ZoomCamera(int thisCase)
    {
        switch (thisCase)
        {
            case 1:
                break;
            case 2:
                break;

        }
    }
    [Client]
    public void FindPlayers()
    {
           foreach(Transform transform in otherPlayersTransform)
        {
            otherPlayersTransform.Remove(transform);
        }
        GameObject[] allPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in allPlayerObjects)
        {
            otherPlayersTransform.Add(player.transform);
        }
    }
}
