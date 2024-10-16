using System;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public const int INITIAL_HAND_SIZE = 5;
    public const float CARD_HAND_PADDING = 0.15f;

    [SyncVar] public string playerName;
    [HideInInspector] public DiscardPile discardPile;
    [HideInInspector, SyncVar] public int combat;
    [HideInInspector, SyncVar] public int trade;
    [HideInInspector, SyncVar] public new int authority;

    public Game game;

    [HideInInspector] public Deck deck;
    [HideInInspector] public Hand hand;
    [HideInInspector] public PlayArea playArea;
    [HideInInspector] public Player enemy;
    [HideInInspector] public TMPro.TextMeshProUGUI authorityScoreText;

    // Start is called before the first frame update
    void Start()
    {
        deck = transform.Find("Deck").GetComponent<Deck>();
        discardPile = transform.Find("DiscardPile").GetComponent<DiscardPile>();
        playArea = transform.Find("PlayArea").GetComponent<PlayArea>();
        hand = transform.Find("Hand").GetComponent<Hand>();

        combat = 0;
        trade = 0;
        authority = 50;
        authorityScoreText.text = $"{authority}";
    }

    public override void OnStartServer()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        deck = transform.Find("Deck").GetComponent<Deck>();
        hand = transform.Find("Hand").GetComponent<Hand>();
    }

    [Client]
    public override void OnStartClient()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public override void OnStartLocalPlayer()
    {
        game.localPlayer = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isClient)
            return;

        authorityScoreText.text = $"{authority}";
    }

    [Server]
    public void DrawCard()
    {
        ShuffleDiscard();

        if (deck.Count() == 0) { return; }

        var card = deck.Pop();

        hand.AddCard(card);
    }

    [Server]
    public void DrawNewHand()
    {
        for (int i = 0; i < INITIAL_HAND_SIZE; i++)
        {
            DrawCard();
        }
    }

    [Server]
    public void ShuffleDiscard()
    {
        if (deck.Count() > 0)
            return;

        var cards = discardPile.RemoveAllCards();

        foreach (var card in cards)
        {
            deck.Push(card);
        }
    }

    [Command]
    public void CmdPlayCard(Card card)
    {
        game.PlayCard(card);
    }

    [Command]
    public void CmdAttackPlayer(Player player)
    {

        game.AttackPlayer(player);
    }

    [Command]
    public void CmdAttackBase(Card card)
    {
        game.AttackBase(card);
    }

    public void PlayCard(Card card)
    {
        if (card.location == Location.HAND)
        {
            hand.RemoveCard(card);
            playArea.AddCard(card);
        }
    }

    public bool CanBuyCard(Card card)
    {
        return trade >= card.cost;
    }

    [Command]
    public void CmdBuyCard(Card card)
    {
        game.BuyCard(card);
    }

    [Command]
    public void CmdChooseCard(Card card)
    {
        if (game.activePlayer != this)
            return;

        game.ChooseCard(card);
    }

    [Command]
    public void CmdResolveAction(Card card, string actionName, string effectID, bool isManual)
    {
        var action = card.FindAction(actionName);
        var effect = action.FindEffect(effectID, isManual: isManual);

        game.ResolveManualEffect(card, action, effect);
    }

    [Server]
    public void BuyCard(Card card)
    {
        trade -= card.cost;

        card.player = this;
        discardPile.AddCard(card);
    }

    public void ScrapCard(Card card)
    {
        Debug.Log("removing card");

        switch (card.location)
        {
            case Location.HAND:
                hand.RemoveCard(card);
                break;

            case Location.DISCARD_PILE:
                discardPile.RemoveCard(card);
                break;

            case Location.PLAY_AREA:
                playArea.RemoveCard(card);
                break;

            default:
                throw new ArgumentException("Scrapped card does not belong to player");
        }
    }

    public void StartTurn()
    {
        playArea.ActivateBases();
    }

    [Command]
    public void CmdEndTurn()
    {
        // probably a cheater
        // why did i do this ? idk i'm obsessed with server authority
        if (game.activePlayer != this)
            return;

        combat = 0;
        trade = 0;

        //TODO: improve to let hand handle this removing logic
        while (hand.Count() > 0)
        {
            var card = hand.FirstCard();
            hand.RemoveCard(card);
            discardPile.AddCard(card);
        }

        var discardedShips = playArea.DiscardShips();
        discardedShips.ForEach(card => discardPile.AddCard(card));

        DrawNewHand();

        game.StartTurn();
    }

    public void Attacked()
    {
        game.AttackPlayer(this);
    }

    public bool HasOutpost()
    {
        return playArea.HasOutpost();
    }

    public void SpendCombat(int value)
    {
        combat -= value;
    }

    public void DestroyBase(Card card)
    {
        playArea.DestroyBase(card);
        discardPile.AddCard(card);
    }

    public bool IsOurTurn()
    {
        return game.activePlayer == game.localPlayer;
    }
}

