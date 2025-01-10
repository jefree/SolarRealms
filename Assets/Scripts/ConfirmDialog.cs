using UnityEngine;

public class ConfirmDialog : MonoBehaviour
{
    public Game game;

    public TMPro.TextMeshProUGUI textMesh;
    Effect.IConfirmNetable effect;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Show(Effect.IConfirmNetable effect, string text)
    {
        this.effect = effect;
        textMesh.text = text;

        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        game.localPlayer.CmdConfirmEffect(effect.ToNet());
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        var ef = (Effect.Base)effect;
        game.localPlayer.CmdCancelEffect(effect.ToNet());

        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
