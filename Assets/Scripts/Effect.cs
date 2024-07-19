public class Effect {
    public Card card;
    public int combat;
    public int trade;
    public int authority;
    bool onScrap;
    bool onAlly;
    bool onDoubleAlly;
    bool optional; // card text says "You may ..."
    bool returnAfterScrap; // this card return to discard pile after activate scrap effect

    public void Resolve(Game game) 
    {
        Player activePlayer = game.activePlayer;

        activePlayer.combat += combat;
        activePlayer.trade += trade;
        activePlayer.authority += authority;

        game.EffectResolved(this);
    }    
    /*
    Next is the list of found effects and where it could belong to in terms of Effect types

    // DrawEffect
    int draw_count; // draw that number of cards
    int max_draw_count; // draw up to that number of cards
    int draw_and_discard_count;


    // DestroyEffect
    int destroy_count; // destroy n enemy bases


    // ScrapEffect
    int scrap_discard_pile_count;
    int scrap_hand_count;
    int scrap_hand_or_discard_pile_count;

    // OpponentDiscardEffect
    int opponent_discard_count;

    // AcquireEffect
    int acquire_ship_count;
    int acquire_ship_max_cost;
    int acquire_base_count;
    int acquire_base_max_cost;
    bool acquire_to_deck; // acquired card must be sent to top of the deck


    // DiscardCombatEffect
    int self_discard_count; // player discard n cards to play effect
    int combat_per_discard_amount; // cobat gain per card discarded

    // RecoverEffect
    int discard_card_to_deck_count;

    // NexToDeckEffect
    bool next_ship_to_deck;
    bool next_base_to_deck;

    // Condition
    int min_base_count; // to activate the effect you need n or more bases in play    
    */
}

// Condition is NOT an Effect
class Condition {
    int count;
    int type; // Ship or Base
}

class DrawEffect : Effect {
    int count;
    bool upToo;
    bool thenDiscard;
}

class DestroyEffect : Effect  {
    int count;
}

class ScrapEffect : Effect {
    bool fromHand;
    bool fromDiscardPile;
    int count;
    bool upTo;
}

class OpponentDiscardEffect : Effect {
    int count;
}

class AcquireEffect : Effect {
    int count;
    int type; // Ship or Base
    int cost;
    bool toDeck;
}

class DiscardCombatEffect : Effect {
    int combatPerCard;
}

class RecoverEffect : Effect {
    int count; // how many cards to recover
    int type; // Ship or Base
}

class NextToDeck : Effect {
    int count;
    int type; // Ship or Base
}