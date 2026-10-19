using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
	enum SceneTarget { GO_TO_ENHANCED, GO_TO_CLASSIC }

	[SerializeField]
	private SceneTarget Target;

	[SerializeField]
	private Button GoToButton;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		GoToButton.onClick.RemoveAllListeners();
		GoToButton.onClick.AddListener(GoToClick);
	}

	private void GoToClick()
	{
		var sceneToLoad = Target == SceneTarget.GO_TO_ENHANCED ? "EnhancedScene" : "ClassicScene";
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
	}
}
