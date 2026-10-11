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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		LoginButton.onClick.RemoveAllListeners();
		LoginButton.onClick.AddListener(LoginButtonClick);

		LogoutButton.onClick.RemoveAllListeners();
		LogoutButton.onClick.AddListener(LogoutButtonClick);

		Disconnect();
	}

	private void LoginButtonClick()
	{
		Connect("testuser");
	}

	private void LogoutButtonClick()
	{
		Disconnect();
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
