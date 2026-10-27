using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	[SerializeField]
	private Transform BlueMiddlePosition;
	[SerializeField]
	private Transform RedMiddlePosition;

	private GameConfiguration gameConfiguration;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameConfiguration = FindAnyObjectByType<GameConfiguration>();

		GameManager.GetInstance().OnEnhancedFinishGame += OnFinishGame;

		InitializeGame();
	}

	private void OnDestroy()
	{
		GameManager.GetInstance().OnEnhancedFinishGame -= OnFinishGame;
	}

	private void OnFinishGame(object sender, Score score)
	{
		InitializeGame();
	}

	private void InitializeGame()
	{
		int randomInteger = Random.Range(0, 9);

		// Debug.Log($"random {randomInteger}");
		if (randomInteger % 2 == 0)
		{
			gameConfiguration.SetConfiguration(Enums.Team.Blue, GameConfiguration.ControllerType.LOCAL_PLAYER, BlueMiddlePosition);
			gameConfiguration.SetConfiguration(Enums.Team.Red, GameConfiguration.ControllerType.IA, RedMiddlePosition);
		}
		else
		{
			gameConfiguration.SetConfiguration(Enums.Team.Blue, GameConfiguration.ControllerType.IA, BlueMiddlePosition);
			gameConfiguration.SetConfiguration(Enums.Team.Red, GameConfiguration.ControllerType.LOCAL_PLAYER, RedMiddlePosition);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
