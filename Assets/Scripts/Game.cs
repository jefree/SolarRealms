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
    PLAY_CARD,
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
    public Player playerOne;
    [HideInInspector]
    public Player activePlayer;
    [HideInInspector]
    public GameState state;
    [HideInInspector]
    public Card currentCard;
    [HideInInspector]
    Action currentAction;
    // Start is called before the first frame update
    void Start()
    {
        activePlayer = playerOne;

        state = GameState.PLAY_CARD;

        messageText.text = "Juega o compra cartas";
    }

    public void PlayCard(Card card)
    {
        state = GameState.RESOLVING_CARD;
        currentCard = card;

        activePlayer.PlayCard(card);
        card.Play();
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
        Debug.Log(card);
        Destroy(card.gameObject);
    }

    public void ResolveAction(Action action)
    {
        currentAction = action;
    }

    public void EffectResolved(Effect.Base effect)
    {
        currentAction.EffectResolved(effect);

        if (effect.isManual)
        {
            StartPlayNewCard();
            return;
        }

        var nextAction = currentCard.NextAction();

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
        state = GameState.PLAY_CARD;
        currentCard = null;
        currentAction = null;
    }

    public void EndTurn()
    {
        activePlayer.EndTurn();
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

    public void ShowMessage(string message)
    {
        messageText.text = message;
    }
}
