using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{

    public UIManager ui;
    public Image image;
    public Transform imgTransform;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(Card card)
    {
        ui.ShowNew(GetComponent<UIDisplay>());

        var zRotation = card.type == CardType.BASE ? 90 : 0;

        image.sprite = card.GetComponent<SpriteRenderer>().sprite;
        imgTransform.rotation = Quaternion.Euler(0, 0, zRotation);

        gameObject.SetActive(true);
    }

    public void Close()
    {
        image.sprite = null;
        gameObject.SetActive(false);
    }
}
