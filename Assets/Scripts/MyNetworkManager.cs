using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyNetworkPlayer myNetworkPlayer = conn.identity.GetComponent<MyNetworkPlayer>();
        myNetworkPlayer.SetDisplayName($"Player {numPlayers}");

        SetPlayerColor(myNetworkPlayer);
    }

    private void SetPlayerColor(MyNetworkPlayer myNetworkPlayer)
    {
        Color randomColor = UnityEngine.Random.ColorHSV();
        myNetworkPlayer.SetColor(randomColor);
    }
}
