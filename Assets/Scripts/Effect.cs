public class Effect {
    public string text;
    public int combat;
    public int trade;
    public int authority;
    bool on_scrap;
    bool on_ally;
    bool on_double_ally;
    bool optional; // card text says "You may ..."
    bool return_after_scrap; // this card return to discard pile after activate scrap effect

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

class DrawEffect {
    int count;
    bool up_to;
    bool then_discard;
}

class DestroyEffect {
    int count;
}

class ScrapEffect {
    bool from_hand;
    bool from_discard_pile;
    int count;
    bool up_to;
}

class OpponentDiscardEffect {
    int count;
}

class AcquireEffect {
    int count;
    int type; // Ship or Base
    int cost;
    bool to_deck;
}

class DiscardCombatEffect {
    int combat;
}

class RecoverEffect {
    int count; // how many cards to recover
    int type; // Ship or Base
}

class NextToDeck {
    int count;
    int type; // Ship or Base
}