using System;
using Unity.Android.Gradle;

public class Card
{
	public enum Element { earth, fire, holy, ice, poison, thunder, water, wind, };
	
	public string name;
	public Element element;
	public int level;
	public string values;
}
