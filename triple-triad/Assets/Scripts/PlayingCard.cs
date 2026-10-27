using cards;
using Enums;
using System;
using System.Linq;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
	private readonly Color BLUE_BACKGROUND_COLOR = Color.blue;
	private readonly Color RED_BACKGROUND_COLOR = Color.red;

	[SerializeField]
	private CardSide CardSideFront;
	[SerializeField]
	private CardSide CardSideBack;

	private int north;
	private int east;
	private int south;
	private int west;
	private int modifier;
	private Card.Element element;
	private Team team;

	private Team currentTeam;
	private CardFlip cardFlip;
	private CardTravel cardTravel;
	private bool isBackShown;
	private bool isSelected;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		cardFlip = GetComponent<CardFlip>();
		cardTravel = GetComponent<CardTravel>();
		isBackShown = false;
	}

	public void Flip(Action onEndFlip)
	{
		CardSideBack.Display(CardSide.DisplayMode.FRONT);
		cardFlip.Flip(onEndFlip);
	}

	public void Travel(Vector3 endPosition, Vector3 middlePosition, Action onEndTravel)
	{
		CardSideBack.Display(CardSide.DisplayMode.FRONT);
		cardTravel.Travel(endPosition, middlePosition, onEndTravel);
	}

	public PlayingCard Load(Card card, Team team, bool useBackImage)
	{
		element = card.elemental;

		var chars = card.values.ToCharArray();
		var values = chars.Select(c => c == 'A' ? 10 : c - '0').ToList();
		north = values[0];
		east = values[1];
		south = values[2];
		west = values[3];

		/*
		CardSideFront.Load(card, (team == Team.Blue) ? BLUE_BACKGROUND_COLOR : RED_BACKGROUND_COLOR);
		CardSideFront.Display(CardSide.DisplayMode.FRONT);

		CardSideBack.Load(card, (team == Team.Blue) ? RED_BACKGROUND_COLOR : BLUE_BACKGROUND_COLOR);
		CardSideBack.Display(useBackImage ? CardSide.DisplayMode.BACK : CardSide.DisplayMode.FRONT);
		*/

		CardSideFront.Load(card, BLUE_BACKGROUND_COLOR);
		CardSideFront.Display(CardSide.DisplayMode.FRONT);

		CardSideBack.Load(card, RED_BACKGROUND_COLOR);
		CardSideBack.Display(useBackImage ? CardSide.DisplayMode.BACK : CardSide.DisplayMode.FRONT);

		this.team = team;
		SetCurrentTeam(team);
		SetIsSelected(false);

		return this;
	}

	public void Play()
	{
		SetCurrentTeam(team);

		CardSideFront.Display(CardSide.DisplayMode.FRONT);

		SetIsSelected(false);
	}

	public void SetModifier(int modifier)
	{
		this.modifier = modifier;

		CardSideFront.SetModifier(modifier);
		CardSideBack.SetModifier(modifier);
	}

	public int GetNorth() => north + modifier;
	public int GetEast() => east + modifier;
	public int GetSouth() => south + modifier;
	public int GetWest() => west + modifier;

	public Team GetCurrentTeam() => currentTeam;
	public Card.Element GetElement() => element;
	public void SetCurrentTeam(Team newTeam)
	{
		currentTeam = newTeam;
	}

	public void SetIsSelected(bool isSelected)
	{
		this.isSelected = isSelected;

		CardSideFront.SetIsSelected(isSelected);
		CardSideBack.SetIsSelected(isSelected);
	}

	public void SetIsHovered(bool isHovered)
	{
		if (CanBeHovered())
		{
			CardSideFront.SetIsHovered(isHovered);
			CardSideBack.SetIsHovered(isHovered);
		}
	}

	private bool CanBeHovered()
	{
		return !isBackShown && !isSelected && team.Equals(Team.Blue);
	}
}