using System;

namespace cards
{
	[Serializable]
	public class Card
	{
		public enum Element { none, earth, fire, holy, ice, poison, thunder, water, wind, };

		public string name;
		public Element element;
		public int level;
		public string values;
		public string set;
		public string nameFormat;

		public override string ToString()
		{
			return $"{name}";
		}
	}
}