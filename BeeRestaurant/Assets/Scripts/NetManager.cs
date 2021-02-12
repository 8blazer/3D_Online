using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        NetPlayer player = conn.identity.GetComponent<NetPlayer>();
        
        player.SetColor(new Color(
            Random.Range(0.00f, 1.00f),
            Random.Range(0.00f, 1.00f),
            Random.Range(0.00f, 1.00f)));
    }
}
