using System;

[Serializable]
public struct CardInfo
{
    public string name;
    public int combat;
    public int trade;

    public string publicName => name;
}