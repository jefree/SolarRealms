using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using Org.BouncyCastle.Crypto.Engines;

public enum GameState
{
    DO_BASIC, //play card from hand, activate ship or base, buy card, check discard pile
    RESOLVING_CARD,
    CHOOSE_CARD,
    CHOOSE_EFFECT
}

public class Game : NetworkBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;
    public const float CARD_SIZE = 2.14f;


    [HideInInspector, SyncVar]
    public Player activePlayer;

    public readonly SyncList<Player> players = new();
    [HideInInspector, SyncVar]
    public GameState state;

    public GameObject cardPrefab;
    public TMPro.TextMeshProUGUI messageText;

    [HideInInspector]
    public TMPro.TextMeshProUGUI tradeScoreText;
    [HideInInspector]
    public TMPro.TextMeshProUGUI combatScoreText;
    public TradeRow tradeRow;
    public EffectListUI actionListUI;
    public DiscardPileList discardPileList;
    public Player localPlayer;

    [HideInInspector]
    int currentPlayerIndex = 0;
    [HideInInspector]
    public Card currentCard;
    [HideInInspector]
    Action currentAction;
    [HideInInspector]
    Queue<Card> playCardsQueue = new();

    void Start()
    {
        tradeScoreText = GameObject.Find("TradeText").GetComponent<TMPro.TextMeshProUGUI>();
        combatScoreText = GameObject.Find("CombatText").GetComponent<TMPro.TextMeshProUGUI>();

        state = GameState.DO_BASIC;
        ShowMessage("Esperando otro jugador");
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

        activePlayer.playArea.PendingCards().ForEach(card => playCardsQueue.Enqueue(card));

        if (queueWasEmpty)
        {
            ResolveCard(playCardsQueue.Dequeue());
        }
    }

    void ResolveCard(Card card)
    {
        state = GameState.RESOLVING_CARD;
        currentCard = card;

        currentCard.Activate();
    }

    public void BuyCard(Card card)
    {
        if (activePlayer.CanBuyCard(card))
        {
            tradeRow.RemoveCard(card);
            activePlayer.BuyCard(card);
            ShowMessage($"Compraste {card.cardName}");
        }
        else
        {
            ShowMessage("No tienes suficiente comercio");
        }
    }

    public void ScrapCard(Card card)
    {
        activePlayer.ScrapCard(card);
        card.gameObject.SetActive(false); // avoiding destroy the game object so animations can work
    }

    public void ResolveAction(Action action)
    {
        currentAction = action;
        currentAction.Activate();
    }

    public void ResolveAction(Action action, Effect.Base effect)
    {
        currentAction = action;
        currentAction.ActivateEffect(effect);
    }

    public void EffectResolved(Effect.Base effect)
    {
        currentAction.EffectResolved(effect);

        effect.Animate(currentCard);

        if (effect.isManual)
        {
            StartPlayNewCard();
            return;
        }

        var nextAction = currentCard.NextAction();

        Debug.Log($"Next Action: {nextAction}");

        if (nextAction != null)
        {
            ResolveAction(nextAction);
        }
        else
        {
            StartPlayNewCard();
        }
    }

    public void StartPlayNewCard()
    {
        if (playCardsQueue.Count > 0)
        {
            ResolveCard(playCardsQueue.Dequeue());
            return;
        }

        state = GameState.DO_BASIC;
        currentCard = null;
        currentAction = null;
    }

    public void StartTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        activePlayer = players[currentPlayerIndex];

        ShowMessage("Next player turn");

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
        ShowMessage("Escoge un carta del mercado para deshuesarla");
    }

    public void ChooseCard(Card card)
    {
        var cardReceiver = (Effect.ICardReceiver)currentAction.currentEffect;
        cardReceiver.SetCard(card);
    }

    public void ChooseEffect(Card card)
    {
        state = GameState.CHOOSE_EFFECT;
        currentCard = card;
        actionListUI.Show(card);
    }

    public void AttackPlayer(Player player)
    {
        if (player == activePlayer)
            return;

        if (player.HasOutpost())
        {
            ShowMessage("Primero destruye las bases protectoras");
            return;
        }

        player.authority -= activePlayer.combat;
        activePlayer.combat = 0;
    }

    public void AttackBase(Card card)
    {

        if (!card.outpost && card.player.HasOutpost())
        {
            ShowMessage("Primero destruye las bases protectoras");
            return;
        }

        if (activePlayer.combat < card.defense)
        {
            ShowMessage("no tienes suficiente combate");
            return;
        }

        activePlayer.SpendCombat(card.defense);
        card.player.DestroyBase(card);
    }

    public void ShowDiscardCards(Player player)
    {
        discardPileList.Show(player.discardPile);
    }

    public void ShowMessage(string message)
    {
        StartCoroutine(ShowMessageAndClean(message));
    }

    [TargetRpc]
    public void TargetSetPlayerTwoView(NetworkConnectionToClient _target)
    {
        Debug.Log("Set Player Two View");

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
