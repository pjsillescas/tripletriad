using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserWidget : MonoBehaviour
{
	[SerializeField]
	private RawImage UserIcon;
	[SerializeField]
	private TextMeshProUGUI UserText;
	[SerializeField]
	private Button LoginButton;
	[SerializeField]
	private Button LogoutButton;
	[SerializeField]
	private LoginWidget LoginWidget;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		LoginWidget.OnLogin += OnLogin;
		LoginWidget.OnLogout += OnLogout;

		LoginButton.onClick.RemoveAllListeners();
		LoginButton.onClick.AddListener(LoginButtonClick);

		LogoutButton.onClick.RemoveAllListeners();
		LogoutButton.onClick.AddListener(LogoutButtonClick);

		LoginWidget.Deactivate();

		Disconnect();
	}

	private void OnDisable()
	{
		LoginWidget.OnLogin -= OnLogin;
		LoginWidget.OnLogout -= OnLogout;
	}

	private void OnLogin(object sender, string username)
	{
		Connect(username);
	}

	private void OnLogout(object sender, EventArgs args)
	{
		Disconnect();
	}

	private void LoginButtonClick()
	{
		LoginWidget.Activate();
	}

	private void LogoutButtonClick()
	{
		LoginWidget.Logout();
	}

	private void Disconnect()
	{
		UserIcon.gameObject.SetActive(false);
		UserText.text = "Disconnected";
		LoginButton.gameObject.SetActive(true);
		LogoutButton.gameObject.SetActive(false);
	}

	private void Connect(string userName)
	{
		UserIcon.gameObject.SetActive(true);
		UserText.text = userName;
		LoginButton.gameObject.SetActive(false);
		LogoutButton.gameObject.SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{

	}

}
