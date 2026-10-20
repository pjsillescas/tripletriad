using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private CinemachineCamera InitialCamera;
	[SerializeField]
	private CinemachineCamera GameCamera;
	[SerializeField]
	private float TravelingTimeout = 2f;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		GameManager.GetInstance().OnStartGame += OnStartGame;
		GameManager.GetInstance().OnFinishGame += OnFinishGame;
	}

	private void OnDisable()
	{
		GameManager.GetInstance().OnStartGame -= OnStartGame;
		GameManager.GetInstance().OnFinishGame -= OnFinishGame;
	}

	private void OnStartGame(object sender, EventArgs e)
	{
		InitialCamera.Priority = 0;
		GameCamera.Priority = 10;
	}

	private void OnFinishGame(object sender, EventArgs e)
	{
		StartCoroutine(DelayFinish());
	}

	private IEnumerator DelayFinish()
	{
		yield return new WaitForSeconds(TravelingTimeout);

		InitialCamera.Priority = 10;
		GameCamera.Priority = 0;
	}
}
