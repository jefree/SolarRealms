using UnityEngine;

public class ConfirmDialog : MonoBehaviour
{
    public Game game;
    public TMPro.TextMeshProUGUI textMesh;
    public GameObject cancelButtonGO;

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

    public void Show(Effect.IConfirmNetable effect, bool showCancel = true)
    {
        this.effect = effect;
        textMesh.text = effect.ConfirmText();

        cancelButtonGO.SetActive(showCancel);
        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        game.localPlayer.CmdConfirmEffect(effect.ToNet(), effect.GetState());
    }

    public void Cancel()
    {
        game.localPlayer.CmdCancelEffect(effect.ToNet());

        Close();
    }

    public void Close()
    {
        effect.CleanUp();
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
