using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
    public static NetManager netManager;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        NetPlayer player = conn.identity.GetComponent<NetPlayer>();
        player.TargetGetPlayerPrefColor();
        conn.identity.GetComponent<CameraFocus>().TargetFindPlayers(conn);
    }

   
}
