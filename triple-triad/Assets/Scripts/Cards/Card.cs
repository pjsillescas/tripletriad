using System;

namespace cards
{
	[Serializable]
	public class Card
	{
		public enum Element { earth, fire, holy, ice, poison, thunder, water, wind, };

		public string name;
		public Element element;
		public int level;
		public string values;

		public override string ToString()
		{
			return $"{name}";
		}
	}
}