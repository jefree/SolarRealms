using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using Org.BouncyCastle.Crypto.Engines;
using System;

public enum GameState
{
    DO_BASIC, //play card from hand, activate ship or base, buy card, check discard pile
    RESOLVING_CARD,
    CHOOSE_CARD
}

public class Game : NetworkBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;
    public const float CARD_WIDTH = 2.14f;
    public const float CARD_PADDING = 0.07f;


    [HideInInspector, SyncVar]
    public Player activePlayer;

    public readonly SyncList<Player> players = new();
    [HideInInspector, SyncVar] public GameState state;
    [HideInInspector, SyncVar] public Card currentCard;

    public GameObject cardPrefab;
    public TMPro.TextMeshProUGUI messageText;

    [HideInInspector] public TMPro.TextMeshProUGUI tradeScoreText;
    [HideInInspector] public TMPro.TextMeshProUGUI combatScoreText;
    public TradeRow tradeRow;
    public EffectListUI actionListUI;
    public DiscardPileList discardPileList;
    public Player localPlayer;

    [HideInInspector]
    int currentPlayerIndex = 0;
    [HideInInspector]
    Queue<Card> playCardsQueue = new();

    void Start()
    {
        tradeScoreText = GameObject.Find("TradeText").GetComponent<TMPro.TextMeshProUGUI>();
        combatScoreText = GameObject.Find("CombatText").GetComponent<TMPro.TextMeshProUGUI>();

        state = GameState.DO_BASIC;
        ShowLocalMessage("Esperando otro jugador");
    }

    void Update()
    {
        if (activePlayer == null)
            return;

        tradeScoreText.text = $"{activePlayer.trade}";
        combatScoreText.text = $"{activePlayer.combat}";
    }

    [Server]
    public void AddPlayer(Player player)
    {
        players.Add(player);
        player.deck.Init(player);

        if (players.Count == 2)
        {
            player.playerName = "Enemy";
            tradeRow.CreateTradeDeck();

            Invoke("StartGame", 2.0f);
        }
        else
        {
            activePlayer = player;
            player.playerName = "Player";
        }
    }

    [Server]
    void StartGame()
    {
        TargetSetPlayerTwoView(players[1].GetComponent<NetworkIdentity>().connectionToClient);

        tradeRow.Init();

        foreach (var player in players)
        {
            player.DrawNewHand();
        }
    }

    public void PlayCard(Card card)
    {
        activePlayer.PlayCard(card);

        var queueWasEmpty = playCardsQueue.Count == 0;

        // enqueue cards with pending effects so remaining effects has a chance to activate if valid
        activePlayer.playArea.PendingCards().ForEach(card => playCardsQueue.Enqueue(card));

        if (queueWasEmpty)
        {
            ResolveCard(playCardsQueue.Dequeue());
        }
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

    public void ScrapCard(Card card)
    {
        if (card.location == Location.TRADE_ROW)
        {
            tradeRow.ScrapCard(card);
        }
        else
        {
            activePlayer.ScrapCard(card);
        }

        Destroy(card.gameObject);
    }


    // ResolveCard -> card.Activate -> ResolveAction -> action.Activate -> effect.Activate -> EffectResolved
    void ResolveCard(Card card)
    {
        state = GameState.RESOLVING_CARD;
        currentCard = card;

        currentCard.Activate();
    }

    public void CardResolved(Card card)
    {
        state = GameState.DO_BASIC;
        currentCard = null;

        if (playCardsQueue.Count > 0)
        {
            ResolveCard(playCardsQueue.Dequeue());
        }
    }

    public void ResolveManualEffect(Card card, Action action, Effect.Base effect)
    {
        if (currentCard != null)
            throw new ArgumentException("There is already a card being played");

        currentCard = card;
        currentCard.currentAction = action;
        action.currentEffect = effect;
        effect.action.ActivateEffect(effect);
    }

    public void StartPlayNewCard()
    {
        if (playCardsQueue.Count > 0)
        {
            ResolveCard(playCardsQueue.Dequeue());
            return;
        }

        state = GameState.DO_BASIC;


        ShowNetMessage("Compra o juega una carta");
    }

    public void StartTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        activePlayer = players[currentPlayerIndex];

        ShowNetMessage("Next player turn");

        state = GameState.DO_BASIC;

        activePlayer.StartTurn();
    }

    [Client]
    public void EndTurn()
    {
        if (activePlayer != localPlayer)
            return;

        activePlayer.CmdEndTurn();
    }

    public void StartChooseCard()
    {
        state = GameState.CHOOSE_CARD;
        ShowNetMessage("Escoge un carta");
    }

    public void ChooseCard(Card card)
    {
        var cardReceiver = (Effect.ICardReceiver)currentCard.currentAction.currentEffect;
        cardReceiver.SetCard(card);
    }

    public void ShowEffectList(Card card)
    {
        if (!card.HasPendingActions(manual: true))
        {
            ShowLocalMessage("Carta sin efectos manuales");
            return;
        }

        actionListUI.Show(card);
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
    public void ShowNetMessage(string message)
    {
        var conn = activePlayer.GetComponent<NetworkIdentity>().connectionToClient;
        TargetShowMessage(conn, message);
    }

    public void ShowLocalMessage(string message)
    {
        StartCoroutine(ShowMessageAndClean(message));
    }

    [TargetRpc]
    void TargetShowMessage(NetworkConnectionToClient conn, string message)
    {
        StartCoroutine(ShowMessageAndClean(message));
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
    }

    IEnumerator ShowMessageAndClean(string message)
    {
        messageText.text = message;

        yield return new WaitForSeconds(1.5f);

        messageText.text = "Juega o compra cartas";
    }
}
