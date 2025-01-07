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
        game.AddPlayer(newPlayer.GetComponent<Player>());
    }
}
