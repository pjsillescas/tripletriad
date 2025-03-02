using UnityEngine;

public class BoardTile : MonoBehaviour
{
	[SerializeField]
	private int Row;
	[SerializeField]
	private int Col;

	public Vector2 GetTileRow() => new Vector2(Row, Col);
	public Vector3 GetTile() => transform.position;
}
