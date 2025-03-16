using UnityEngine;

public class Spinner : MonoBehaviour
{
	[SerializeField]
	private float RotationSpeed = 80f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//transform.rotation = Quaternion.Euler(transform.rotation.x , transform.rotation.y + Time.deltaTime * RotationSpeed,
		//   transform.rotation.z );
		transform.Rotate(0, 0, Time.deltaTime * RotationSpeed);
	}
}
