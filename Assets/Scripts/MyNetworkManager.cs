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
            Debug.Log("Enemy arrives");
            newPlayer = GameObject.Find("Enemy");
        }
        else
        {
            Debug.Log("Player arrives");
            newPlayer = GameObject.Find("Player");
        }

        NetworkServer.AddPlayerForConnection(conn, newPlayer);
        game.AddPlayer(newPlayer.GetComponent<Player>());
    }
}
