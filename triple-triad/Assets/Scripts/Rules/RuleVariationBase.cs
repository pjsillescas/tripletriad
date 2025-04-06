
public class RuleVariationBase : IRuleVariation
{
	public virtual bool UseCardBack()
	{
		return true;
	}

	public virtual bool ImplementsWinsDirection()
	{
		return false;
	}

	public virtual bool WinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction)
	{
		return false;
	}

	public virtual void Initialize()
	{
		;
	}
}
