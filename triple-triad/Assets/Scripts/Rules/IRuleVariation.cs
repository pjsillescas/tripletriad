
public interface IRuleVariation
{
	bool UseCardBack();
	bool ImplementsWinsDirection();

	bool WinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction);
}
