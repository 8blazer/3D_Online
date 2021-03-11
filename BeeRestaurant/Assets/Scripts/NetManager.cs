using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
    public static NetManager netManager;
    public int connections;
    public bool isGameStarted = false;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        connections++;
        NetPlayer player = conn.identity.GetComponent<NetPlayer>();
        player.TargetGetPlayerPrefColor();
        CameraFocus playerCamera = conn.identity.GetComponent<CameraFocus>();
    }

   
}
