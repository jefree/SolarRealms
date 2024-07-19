using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Effect;
using UnityEngine;

public enum GameState
{
    PLAY_CARD,
    RESOLVING_CARD,
    CHOOSE_CARD
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
    public Effect.IEffect currentEffect;
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
        state = GameState.RESOLVING_CARD;
        currentCard = card;
        ResolveEffect(card.effects.First<Effect.IEffect>());
    }

    public void ResolveEffect(Effect.IEffect effect)
    {
        currentEffect = effect;
        currentEffect.Activate();
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
        Destroy(card.gameObject);
    }

    public void EffectResolved(Effect.IEffect effect)
    {
        if (currentEffect == effect)
        {
            currentCard.effects.Remove(effect);

            if (currentCard.effects.Count == 0)
            {
                activePlayer.Discard(currentCard);
                currentCard = null;
                StartPlayNewCard();
            }
            else
            {
                var nextEffect = currentCard.effects.First<Effect.IEffect>();
                ResolveEffect(nextEffect);
            }
        }
    }

    public void StartPlayNewCard()
    {
        state = GameState.PLAY_CARD;
        currentEffect = null;
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
        var cardReceiver = (Effect.ICardReceiver)currentEffect;
        cardReceiver.SetCard(card);
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
    }
}
