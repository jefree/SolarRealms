using UnityEngine;

public class PlayerAuthority : MonoBehaviour
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (player == player.game.activePlayer)
            return;

        if (player.HasOutpost())
        {
            player.game.ShowMessage("Primero destruye las bases protectoras");
            return;
        }

        player.game.localPlayer.CmdAttackPlayer(player);
    }
}
