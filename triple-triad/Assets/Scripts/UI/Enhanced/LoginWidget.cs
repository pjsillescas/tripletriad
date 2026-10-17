using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWidget : MonoBehaviour
{
	public static event EventHandler<string> OnLogin;
	public static event EventHandler OnLogout;

	[SerializeField]
	private TMP_InputField UsernameInput;

	[SerializeField]
	private TMP_InputField PasswordInput;

	[SerializeField]
	private Button LoginButton;
	[SerializeField]
	private Button SignupButton;

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void Logout()
	{
		OnLogout?.Invoke(this, EventArgs.Empty);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		LoginButton.onClick.RemoveAllListeners();
		LoginButton.onClick.AddListener(LoginClick);
		
		SignupButton.onClick.RemoveAllListeners();
		SignupButton.onClick.AddListener(SignupClick);
	}

	private void LoginClick()
	{
		OnLogin?.Invoke(this, UsernameInput.text);
		Deactivate();
	}

	private void SignupClick()
	{
		OnLogin?.Invoke(this, UsernameInput.text);
		Deactivate();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
