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
}