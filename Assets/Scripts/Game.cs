using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameState
{
    PLAY_CARD,
    RESOLVING_EFFECT
}

public class Game : MonoBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;
    public const float CARD_SIZE = 2.14f;

    public GameObject cardPrefab;
    public GameObject playerOneGO;
    public GameObject tradeRowGO;
    public TMPro.TextMeshProUGUI messageText;


    public Player playerOne;
    public TradeRow tradeRow;
    public Player activePlayer;
    public GameState state;
    public Card currentCard;
    public Effect currentEffect;
    // Start is called before the first frame update
    void Start()
    {
        playerOne = playerOneGO.GetComponent<Player>();
        activePlayer = playerOne;

        tradeRow = tradeRowGO.GetComponent<TradeRow>();

        state = GameState.PLAY_CARD;

        messageText.text = "Juega o compra cartas";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCard(Card card)
    {
        state = GameState.RESOLVING_EFFECT;
        currentCard = card;
        currentEffect = card.primaryMainEffect;

        currentEffect.Resolve(this);
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

    public void EffectResolved(Effect effect)
    {
        if (effect == currentEffect)
        {
            var card = effect.card;
            activePlayer.Discard(card);

            PlayNewCard();
        }

    }

    public void PlayNewCard()
    {
        state = GameState.PLAY_CARD;
        currentEffect = null;
    }

    public void EndTurn()
    {

        activePlayer.EndTurn();

    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
    }
}
