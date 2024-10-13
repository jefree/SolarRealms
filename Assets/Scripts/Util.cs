using System;
using System.Collections.Generic;
using Mirror;

public static class Util
{
    public static void PopulateCardInfo(ref CardInfo info)
    {
        if (info.card != null)
            return;

        while (info.card == null)
        {
            if (NetworkClient.spawned.TryGetValue(info.cardGOID, out NetworkIdentity identity))
                info.card = identity.gameObject.GetComponent<Card>();
        }
    }

    public static void Shuffle<T>(List<T> list)
    {
        List<T> temp = new();
        Random random = new();

        while (list.Count > 0)
        {
            var pos = random.Next(list.Count);
            temp.Add(list[pos]);
            list.RemoveAt(pos);
        }

        foreach (var elm in temp)
        {
            list.Add(elm);
        }
    }
}