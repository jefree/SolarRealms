using System;
using Mirror;

[Serializable]
public struct CardInfo
{
    public uint cardGOID;
    [System.NonSerialized]
    public Card card;

    public CardInfo(Card card)
    {
        cardGOID = card.netId;
        this.card = card;
    }

}

public class SyncListCardInfo : SyncList<CardInfo> { }