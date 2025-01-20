using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Search;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public int currentOrder = 1;
    public List<UIDisplay> displays;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrent();
        }
    }

    public void ShowNew(UIDisplay display)
    {
        displays.Add(display);
        display.Show(currentOrder);
        currentOrder += 1;
    }

    public void CloseCurrent()
    {
        if (displays.Count == 0)
        {
            return;
        }

        var display = displays.Last();

        displays.Remove(display);
        display.Close();
        currentOrder -= 1;
    }
}