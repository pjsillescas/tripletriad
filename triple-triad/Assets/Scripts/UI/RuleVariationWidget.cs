using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RuleVariationWidget : MonoBehaviour
{
	private List<Toggle> toggles;

	private Dictionary<string, IRuleVariation> rules;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		rules = new() {
			{ "OpenRuleToggle", new OpenRule() },
			{ "SameRuleToggle", new SameRule() },
			{ "SameWallRuleToggle", new SameWallRule() },
			{ "SuddenDeathRuleToggle", new SuddenDeathRule() },
			{ "RandomRuleToggle", new RandomRule() },
			{ "PlusRuleToggle", new PlusRule() },
			{ "ComboRuleToggle", new ComboRule() },
			{ "ElementalRuleToggle", new ElementalRule() },
	};
		toggles = new(GetComponentsInChildren<Toggle>());

		toggles.ForEach(toggle => toggle.onValueChanged.AddListener((value) => SoundManager.GetInstance().Click()));
	}

	public List<IRuleVariation> GetRuleVariations()
	{
		return toggles.Where(toggle => toggle.isOn).Select(toggle => rules[toggle.name]).ToList();
	}
}
