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
        textMesh.text = effect.ConfirmText();
    }

    public void Show(Effect.IConfirmNetable effect)
    {
        this.effect = effect;
        textMesh.text = effect.ConfirmText();

        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        game.localPlayer.CmdConfirmEffect(effect.ToNet(), effect.GetState());
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
