using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FireworksSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject FireworksPrefab;
	[SerializeField]
	private int NumberOfFireworks = 6;
	[SerializeField]
	private float SpawningLapse = 0.4f;

	private BoxCollider box;

	private Vector3 RandomPointInBounds(Bounds bounds)
	{
		return new Vector3(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y),
			Random.Range(bounds.min.z, bounds.max.z)
		);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		box = GetComponent<BoxCollider>();
		GameManager.GetInstance().OnEnhancedFinishGame += OnFinishGame;
	}

	private void OnDestroy()
	{
		GameManager.GetInstance().OnEnhancedFinishGame -= OnFinishGame;
	}

	private void OnFinishGame(object sender, GameManager.Score score)
	{
		if (score.player > score.adversary)
		{
			Spawn();
		}
	}

	public void Spawn()
	{
		StartCoroutine(SpawnFireworks());
	}

	private IEnumerator SpawnFireworks()
	{
		int k = 0;
		while(k < NumberOfFireworks)
		{
			var position = RandomPointInBounds(box.bounds);
			Instantiate(FireworksPrefab, position, Quaternion.identity);
			k++;
			yield return new WaitForSeconds(SpawningLapse);
		}
	}
}
