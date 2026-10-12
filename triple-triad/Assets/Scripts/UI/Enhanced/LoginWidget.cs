using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWidget : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField UsernameInput;

	[SerializeField]
	private TMP_InputField PasswordInput;

	[SerializeField]
	private Button LoginButton;
	[SerializeField]
	private Button SignupButton;

	private Action<string> OnLogin;

	public void Activate(Action<string> onLogin)
	{
		OnLogin = onLogin;
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
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
		OnLogin(UsernameInput.text);
		Deactivate();
	}

	private void SignupClick()
	{
		OnLogin(UsernameInput.text);
		Deactivate();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
