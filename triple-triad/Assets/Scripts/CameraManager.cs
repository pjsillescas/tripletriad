using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private CinemachineCamera InitialCamera;
	[SerializeField]
	private CinemachineCamera GameCamera;


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
		InitialCamera.Priority = 10;
		GameCamera.Priority = 0;
	}
}
