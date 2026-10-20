using UnityEngine;

public class Fireworks : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem rocket;
	[SerializeField]
	private ParticleSystem burst;

	void Start()
	{
		burst.Play();
	}

	void Update()
	{
		if (!burst.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}
