using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FACTION {
    UNALIGNED,
    TRADE_FEDERATION,
    THE_BLOBS,
    STAR_EMPIRE,
    MACHINE_CULT
}

public class Card : MonoBehaviour
{
    public string cardName;
    public Effect primaryMainEffect;
    public Effect primaryOptionalEffect;
    public Effect allyEffect;
    public Effect doubleAllyEffect;
    public Effect scrapEffect;

    string secondaryText; // ??
    public int cost;
    public int defense;
    public bool outpost;
    public FACTION faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    public int type; // Ship or Base


    public void Start() {
        
    }
    void OnMouseDown() {
        Debug.Log(cardName);
    }
}