using UnityEngine;

public class UIDisplay : MonoBehaviour
{

    public Canvas canvas;
    public UnityEngine.UI.Button.ButtonClickedEvent onClose;

    public void Show(int order)
    {
        canvas.sortingOrder = order;
    }

    public virtual void Close()
    {
        onClose.Invoke();
    }
}