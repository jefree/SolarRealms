using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using GameStates;
using Effect;

public enum TurnEffect
{
    Scrap
}

public class Game : NetworkBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;
    public const float CARD_WIDTH = 2.14f;
    public const float CARD_PADDING = 0.07f;

    [HideInInspector, SyncVar] public Player activePlayer;
    public readonly SyncList<Player> players = new();
    [HideInInspector] public Card currentCard;
    [SyncVar] public List<TurnEffect> turnEffects = new();

    public GameObject cardPrefab;
    public TMPro.TextMeshProUGUI messageText;

    [HideInInspector] public TMPro.TextMeshProUGUI tradeScoreText;
    [HideInInspector] public TMPro.TextMeshProUGUI combatScoreText;
    public TradeRow tradeRow;
    public EffectListUI actionListUI;
    public DiscardPileList discardPileList;
    public ConfirmDialog confirmDialog;
    public CardView cardView;
    public Player localPlayer;
    List<Card> scrapHeap = new();

    [HideInInspector]
    int currentPlayerIndex = 0;
    [HideInInspector]
    //Queue<Card> playCardsQueue = new();

    GameStates.Handler stateHandler;
    Effect.ICardReceiver currentReceiver;

    void Start()
    {
        tradeScoreText = GameObject.Find("TradeText").GetComponent<TMPro.TextMeshProUGUI>();
        combatScoreText = GameObject.Find("CombatText").GetComponent<TMPro.TextMeshProUGUI>();

        state = GameStates.DO_BASIC;
        stateHandler = new(this);
        ShowLocalMessage("Esperando otro jugador", persist: true);
    }

    void Update()
    {
        if (activePlayer == null)
            return;

        stateHandler.Update();

        tradeScoreText.text = $"{activePlayer.trade}";
        combatScoreText.text = $"{activePlayer.combat}";
    }

    [Server]
    public void AddPlayer(Player player)
    {
        players.Add(player);
        player.deck.Init(player);

        //Temporary logic to allow 1 player game
        activePlayer = player;
        player.playerName = "Player";
        tradeRow.CreateTradeDeck();
        Invoke("StartGame", 2.0f);

        // if (players.Count == 2)
        // {
        //     player.playerName = "Enemy";
        //     tradeRow.CreateTradeDeck();
        //     Invoke("StartGame", 2.0f);
        // }
        // else
        // {
        //     activePlayer = player;
        //     player.playerName = "Player";
        // }
    }

    [Server]
    void StartGame()
    {
        //TODO: enable this for 2 players game
        // TargetSetPlayerTwoView(players[1].GetComponent<NetworkIdentity>().connectionToClient);

        tradeRow.Init();

        foreach (var player in players)
        {
            player.DrawNewHand();
        }

        ShowLocalMessage("Juega una o mas cartas", persist: true);
    }

    [Server]
    public void PlayCard(Card card)
    {
        activePlayer.PlayCard(card);

        // enqueue cards with pending effects so remaining effects has a chance to activate if valid
        // activePlayer.playArea.PendingCards().ForEach(card => playCardsQueue.Enqueue(card));
        // activePlayer.playArea.PendingCards().ForEach(card => stateHandler.ProcessCard(card));

        // if (playCardsQueue.Count > 0)
        // {
        //     ResolveCard(playCardsQueue.Dequeue());
        // }
    }

    public void BuyCard(Card card)
    {
        if (activePlayer.CanBuyCard(card))
        {
            tradeRow.RemoveCard(card);
            activePlayer.BuyCard(card);
            ShowNetMessage($"Compraste {card.cardName}");
        }
        else
        {
            ShowNetMessage("No tienes suficiente comercio");
        }
    }

    [Server]
    public void ScrapCard(Card card)
    {
        if (card.location == CardLocation.TRADE_ROW)
        {
            tradeRow.ScrapCard(card);
        }
        else
        {
            activePlayer.ScrapCard(card);
        }

        RecordEffect(TurnEffect.Scrap);

        scrapHeap.Add(card);
        card.location = CardLocation.SCRAP_HEAP;
        card.gameObject.SetActive(false);
    }

    [Server]
    public void AcquireCard(Card card, Deck.Location location)
    {
        tradeRow.RemoveCard(card);
        activePlayer.AcquireCard(card, location);
    }

    [Server]
    public void FreeCard(Card card)
    {
        tradeRow.RemoveCard(card);
        activePlayer.FreeCard(card);
    }

    // ResolveCard -> card.Activate -> ResolveAction -> action.Activate -> effect.Activate -> EffectResolved
    void ResolveCard(Card card)
    {
        state = GameStates.RESOLVING_CARD;
        currentCard = card;

        currentCard.Activate();
    }

    [Server]
    public void OnCardResolved(Card card)
    {
        state = GameStates.DO_BASIC;
        currentCard = null;

        // if (playCardsQueue.Count > 0)
        // {
        //     ResolveCard(playCardsQueue.Dequeue());
        // }
    }

    [Server]
    public void SetCurrentEffect(Effect.Base effect)
    {
        if (currentCard != null)
            throw new ArgumentException("There is already a card being played");

        currentCard = effect.action.card;
        currentCard.currentAction = effect.action;
        effect.action.currentEffect = effect;
    }

    [Client]
    void SetLocalCurrentEffect(Effect.Base effect)
    {
        if (isServer) { return; }

        currentCard = effect.action.card;
        currentCard.currentAction = effect.action;
        effect.action.currentEffect = effect;
    }

    [Server]
    public void ResolveManualEffect(Effect.Base effect)
    {
        // SetCurrentEffect(effect);
        // effect.action.ActivateEffect(effect);

        if (!effect.action.SatisfyConditions())
        {
            throw new ArgumentException("Action do not satisfy conditions");
        }

        stateHandler.Add(effect.action, effect);
    }

    [Server]
    public void ResolveIsolatedEffect(Effect.Base effect)
    {
        effect.Apply(this);
    }

    // public void StartPlayNewCard()
    // {
    //     if (playCardsQueue.Count > 0)
    //     {
    //         ResolveCard(playCardsQueue.Dequeue());
    //         return;
    //     }

    //     state = GameState.DO_BASIC;


    //     ShowNetMessage("Compra o juega una carta");
    // }

    [Server]
    public void StartTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        activePlayer = players[currentPlayerIndex];
        ClearTurnEffects();

        ShowNetMessage("Tu turno");

        state = GameStates.DO_BASIC;

        activePlayer.StartTurn();
    }

    [Client]
    public void EndTurn()
    {
        if (activePlayer != localPlayer)
            return;

        activePlayer.CmdEndTurn();
        ShowLocalMessage("Turno Oponente", persist: true);
    }

    [Server]
    public void StartChooseCard()
    {
        stateHandler.StartChooseCard();
        ShowNetMessage("Escoge un carta");
    }

    public void ChooseCard(Card card)
    {
        if (currentReceiver == null)
        {
            throw new InvalidOperationException("there is no current receiver");
        }

        currentReceiver.SetCard(this, card);
    }

    [Client]
    public void StartConfirmEffect(Effect.IConfirmNetable effect, bool showCancel = true)
    {
        var baseEffect = (Effect.Base)effect;

        SetLocalCurrentEffect(baseEffect);
        confirmDialog.Show(effect, showCancel);
    }

    [Client]
    public void ShowEffectList(Card card)
    {
        if (!card.HasPendingActions(manual: true))
        {
            ShowLocalMessage("Carta sin efectos manuales");
            return;
        }

        actionListUI.Show(card);
    }

    [Client]
    public void ShowCard(Card card)
    {
        if (card != null)
        {
            cardView.Show(card);
        }
        else
        {
            cardView.Close();
        }
    }

    [Server]
    public void CloseNetConfirm()
    {
        var conn = activePlayer.GetComponent<NetworkIdentity>().connectionToClient;
        TargetCloseConfirm(conn);
    }

    public void AttackPlayer(Player player)
    {
        if (player == activePlayer)
            return;

        if (player.HasOutpost())
        {
            ShowNetMessage("Primero destruye las bases protectoras");
            return;
        }

        player.authority -= activePlayer.combat;
        activePlayer.combat = 0;
    }

    public void AttackBase(Card card)
    {
        if (!card.outpost && card.player.HasOutpost())
        {
            ShowNetMessage("Primero destruye las bases protectoras");
            return;
        }

        if (activePlayer.combat < card.defense)
        {
            ShowNetMessage("no tienes suficiente combate");
            return;
        }

        activePlayer.SpendCombat(card.defense);
        card.player.DestroyBase(card);
    }

    public void ShowDiscardCards(Player player)
    {
        discardPileList.Show(player.discardPile);
    }

    [Server]
    public void ShowNetMessage(string message, bool persist = false)
    {
        var conn = activePlayer.netIdentity.connectionToClient;
        TargetShowMessage(conn, message, persist);
    }

    public void ShowLocalMessage(string message, bool persist = false)
    {
        if (persist)
        {
            ShowMessage(message);
        }
        else
        {
            StartCoroutine(ShowMessageAndClean(message));
        }
    }

    [TargetRpc]
    void TargetShowMessage(NetworkConnectionToClient conn, string message, bool persist)
    {
        ShowLocalMessage(message, persist);
    }

    [TargetRpc]
    public void TargetStartConfirm(NetworkConnectionToClient conn, Effect.NetEffect netEffect)
    {
        var effect = (Effect.Manual)netEffect.GetEffect();
        currentReceiver = (Effect.ICardReceiver)effect;
        effect.ManualActivate(this);
    }

    [TargetRpc]
    public void TargetCloseConfirm(NetworkConnectionToClient conn)
    {
        currentReceiver = null;
        confirmDialog.Close();
    }

    [TargetRpc]
    public void TargetSetPlayerTwoView(NetworkConnectionToClient _target)
    {
        var one = players[0];
        var two = players[1];

        (one.transform.position, two.transform.position) = (two.transform.position, one.transform.position);
        (one.playArea.transform.localPosition, two.playArea.transform.localPosition) = (two.playArea.transform.localPosition, one.playArea.transform.localPosition);
        (one.hand.transform.localPosition, two.hand.transform.localPosition) = (two.hand.transform.localPosition, one.hand.transform.localPosition);
        (one.deck.transform.localPosition, two.deck.transform.localPosition) = (two.deck.transform.localPosition, one.deck.transform.localPosition);
        (one.discardPile.transform.localPosition, two.discardPile.transform.localPosition) = (two.discardPile.transform.localPosition, one.discardPile.transform.localPosition);


        var aOne = one.transform.Find("Authority/Button").GetComponent<RectTransform>();
        var aTwo = two.transform.Find("Authority/Button").GetComponent<RectTransform>();

        (aOne.anchoredPosition, aTwo.anchoredPosition) = (aTwo.anchoredPosition, aOne.anchoredPosition);

        var dOne = one.transform.Find("Deck/Canvas/DeckCountText").GetComponent<RectTransform>();
        var dTwo = two.transform.Find("Deck/Canvas/DeckCountText").GetComponent<RectTransform>();

        (dOne.anchoredPosition, dTwo.anchoredPosition) = (dTwo.anchoredPosition, dOne.anchoredPosition);

        ShowLocalMessage("Turno Oponente", persist: true);
    }

    IEnumerator ShowMessageAndClean(string message)
    {
        messageText.text = message;

        yield return new WaitForSeconds(1.5f);

        messageText.text = "Juega o compra cartas";
    }

    void ShowMessage(string message)
    {
        messageText.text = message;
    }

    void RecordEffect(TurnEffect effect)
    {
        turnEffects.Add(effect);
    }

    void ClearTurnEffects()
    {
        turnEffects.Clear();
    }
}
