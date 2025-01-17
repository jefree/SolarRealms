public enum CardLocation
{
    UNDEFINED = 1,
    TRADE_DECK = 2,
    HAND = 4,
    PLAY_AREA = 8,
    TRADE_ROW = 16,
    DECK = 32,
    DISCARD_PILE = 64,
    TRASH = 128,
    HAND_OR_DISCARD = HAND | DISCARD_PILE
}