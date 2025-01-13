using Mirror;
using Mono.CecilX.Cil;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameObject newPlayer;

        if (NetworkServer.connections.Count == 2)
        {
            newPlayer = GameObject.Find("Enemy");
        }
        else
        {
            newPlayer = GameObject.Find("Player");
        }

        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        var player = newPlayer.GetComponent<Player>();
        var hand = player.hand.netIdentity;
        var playArea = player.playArea.netIdentity;

        hand.AssignClientAuthority(conn);
        playArea.AssignClientAuthority(conn);

        game.AddPlayer(newPlayer.GetComponent<Player>());
    }
}
