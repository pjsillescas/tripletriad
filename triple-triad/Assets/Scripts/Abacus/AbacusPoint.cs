using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AbacusPoint : MonoBehaviour
{
	const float SPEED = 6f;

	public enum PointColor { BLUE, RED }

	private int abacusIndex;
	private PointColor color;
	private Material material;

	public void SetAbacusIndex(int abacusIndex, Vector3 position)
	{
		var direction = Mathf.Sign(abacusIndex - this.abacusIndex);
		this.abacusIndex = abacusIndex;
		StartCoroutine(GoToPosition(position, direction));
	}

	public void SetAbacusIndexInstant(int abacusIndex, Vector3 position)
	{
		this.abacusIndex = abacusIndex;
		transform.position = position;
	}

	private IEnumerator GoToPosition(Vector3 position, float direction)
	{
		while (Vector3.SqrMagnitude(position - transform.position) > 0.1f)
		{
			var deltaX = direction * SPEED * Time.deltaTime;
			transform.position = transform.position + new Vector3(deltaX, 0, 0);
			yield return null;
		}
		transform.position = position;
		yield return null;
	}

	public int GetAbacusIndex() => abacusIndex;

	public void SetColor(PointColor color)
	{
		this.color = color;
		material.SetColor("_BaseColor", color == PointColor.RED ? Color.red : Color.blue);
	}

	public PointColor GetColor() => color;

	void Awake()
	{
		abacusIndex = 0;
	}

	private void Start()
	{
		material = GetComponent<MeshRenderer>().material;
	}
}
