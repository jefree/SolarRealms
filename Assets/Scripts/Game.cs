using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Effect;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    DO_BASIC, //play card from hand, activate ship or base, buy card, check discard pile
    RESOLVING_CARD,
    CHOOSE_CARD,
    CHOOSE_EFFECT
}

public class Game : MonoBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;
    public const float CARD_SIZE = 2.14f;

    public GameObject cardPrefab;
    public TMPro.TextMeshProUGUI messageText;

    public TradeRow tradeRow;
    public EffectListUI actionListUI;
    public DiscardPileList discardPileList;
    public Player[] players;
    [HideInInspector]
    int currentPlayerIndex = 0;
    [HideInInspector]
    public Player activePlayer;
    [HideInInspector]
    public GameState state;
    [HideInInspector]
    public Card currentCard;
    [HideInInspector]
    Action currentAction;
    [HideInInspector]
    Queue<Card> playCardsQueue = new();

    void Start()
    {
        activePlayer = players[currentPlayerIndex];

        state = GameState.DO_BASIC;

        messageText.text = "Juega o compra cartas";
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

        Debug.Log($"Resolving: {card.cardName}");

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
        state = GameState.DO_BASIC;
        activePlayer.StartTurn();
    }

    public void EndTurn()
    {
        activePlayer.EndTurn();

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        activePlayer = players[currentPlayerIndex];

        StartTurn();
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
        {
            return;
        }

        if (player.HasOutpost())
        {
            messageText.text = "Primero destruye las bases protectoras";
            return;
        }

        player.authority -= activePlayer.combat;
        activePlayer.combat = 0;
    }

    public void AttackBase(Card card)
    {

        if (!card.outpost && card.player.HasOutpost())
        {
            messageText.text = "Primero destruye las bases protectoras";
            return;
        }

        if (activePlayer.combat < card.defense)
        {
            messageText.text = "no tienes suficiente combate";
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
        messageText.text = message;
    }
}
