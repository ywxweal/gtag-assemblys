using System;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000711 RID: 1809
public class ModIOAccountLinkingTerminal : MonoBehaviour
{
	// Token: 0x06002D1D RID: 11549 RVA: 0x000DEF00 File Offset: 0x000DD100
	public void Start()
	{
		this.modioUsernameLabelText.gameObject.SetActive(false);
		this.modioUsernameText.gameObject.SetActive(false);
		this.loggingInText.gameObject.SetActive(false);
		this.linkAccountPromptText.gameObject.SetActive(true);
		this.linkAccountLabelText.gameObject.SetActive(false);
		this.linkAccountURLLabelText.gameObject.SetActive(false);
		this.linkAccountURLText.gameObject.SetActive(false);
		this.linkAccountCodeLabelText.gameObject.SetActive(false);
		this.linkAccountCodeText.gameObject.SetActive(false);
		this.alreadyLinkedAccountText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		GameEvents.OnModIOKeyboardButtonPressedEvent.AddListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(ModIOAccountLinkingTerminal.PressButton));
		GameEvents.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOAccountLinkingTerminal.OnButtonPress.AddListener(new UnityAction(this.ResetScreen));
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000DF00C File Offset: 0x000DD20C
	public void OnDestroy()
	{
		GameEvents.OnModIOKeyboardButtonPressedEvent.RemoveListener(new UnityAction<CustomMapsTerminalButton.ModIOKeyboardBindings>(ModIOAccountLinkingTerminal.PressButton));
		GameEvents.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOAccountLinkingTerminal.OnButtonPress.RemoveListener(new UnityAction(this.ResetScreen));
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000DF05B File Offset: 0x000DD25B
	public void OnModIOLoggedIn()
	{
		if (!this.isLoggedIn)
		{
			ModIOUnity.GetCurrentUser(delegate(ResultAnd<UserProfile> result)
			{
				if (!result.result.Succeeded())
				{
					return;
				}
				this.isLoggedIn = true;
				this.errorText.gameObject.SetActive(false);
				this.loggingInText.gameObject.SetActive(false);
				this.linkAccountLabelText.gameObject.SetActive(false);
				this.linkAccountURLLabelText.gameObject.SetActive(false);
				this.linkAccountURLText.gameObject.SetActive(false);
				this.linkAccountCodeLabelText.gameObject.SetActive(false);
				this.linkAccountCodeText.gameObject.SetActive(false);
				this.linkAccountPromptText.gameObject.SetActive(false);
				this.alreadyLinkedAccountText.gameObject.SetActive(false);
				this.linkAccountCodeText.text = "";
				this.linkAccountURLText.text = "";
				this.errorText.text = "";
				this.modioUsernameText.text = result.value.username;
				this.modioUsernameLabelText.gameObject.SetActive(true);
				this.modioUsernameText.gameObject.SetActive(true);
				if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
				{
					this.linkAccountPromptText.gameObject.SetActive(true);
				}
				else
				{
					this.alreadyLinkedAccountText.gameObject.SetActive(true);
				}
				GameEvents.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
				this.processingAccountLink = false;
			}, false);
		}
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000DF078 File Offset: 0x000DD278
	private void OnModIOLoggedOut()
	{
		if (this.isLoggedIn)
		{
			this.isLoggedIn = false;
			this.processingAccountLink = false;
			ModIOManager.CancelExternalAuthentication();
			this.modioUsernameLabelText.gameObject.SetActive(false);
			this.modioUsernameText.gameObject.SetActive(false);
			this.modioUsernameText.text = "";
			this.loggingInText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.linkAccountLabelText.gameObject.SetActive(false);
			this.linkAccountURLLabelText.gameObject.SetActive(false);
			this.linkAccountURLText.gameObject.SetActive(false);
			this.linkAccountCodeLabelText.gameObject.SetActive(false);
			this.linkAccountCodeText.gameObject.SetActive(false);
			this.linkAccountPromptText.gameObject.SetActive(false);
			this.alreadyLinkedAccountText.gameObject.SetActive(false);
			this.linkAccountPromptText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x000DF180 File Offset: 0x000DD380
	private static void PressButton(CustomMapsTerminalButton.ModIOKeyboardBindings pressedButton)
	{
		if (pressedButton == CustomMapsTerminalButton.ModIOKeyboardBindings.option2)
		{
			if (ModIOManager.IsLoggedIn())
			{
				ModIOManager.LogoutFromModIO();
			}
			else
			{
				UnityEvent onButtonPress = ModIOAccountLinkingTerminal.OnButtonPress;
				if (onButtonPress != null)
				{
					onButtonPress.Invoke();
				}
			}
		}
		if (pressedButton == CustomMapsTerminalButton.ModIOKeyboardBindings.option4)
		{
			if (!ModIOManager.IsLoggedIn())
			{
				ModIOManager.CancelExternalAuthentication();
				ModIOManager.RequestPlatformLogin(null);
				return;
			}
			UnityEvent onButtonPress2 = ModIOAccountLinkingTerminal.OnButtonPress;
			if (onButtonPress2 == null)
			{
				return;
			}
			onButtonPress2.Invoke();
		}
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x000DF1D7 File Offset: 0x000DD3D7
	public void LinkButtonPressed()
	{
		if (!this.processingAccountLink)
		{
			this.processingAccountLink = true;
			ModIOManager.IsAuthenticated(delegate(Result result)
			{
				if (result.Succeeded())
				{
					if (ModIOManager.GetLastAuthMethod() == ModIOManager.ModIOAuthMethod.LinkedAccount)
					{
						this.processingAccountLink = false;
						this.alreadyLinkedAccountText.gameObject.SetActive(true);
						this.linkAccountPromptText.gameObject.SetActive(false);
						return;
					}
					GameEvents.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
					ModIOManager.LogoutFromModIO();
					this.isLoggedIn = false;
				}
				this.errorText.gameObject.SetActive(false);
				this.errorText.text = "";
				this.modioUsernameLabelText.gameObject.SetActive(false);
				this.modioUsernameText.gameObject.SetActive(false);
				this.modioUsernameText.text = "";
				ModIOManager.RequestAccountLinkCode(delegate(ModIORequestResult result, string linkURL, string linkCode)
				{
					this.linkAccountPromptText.gameObject.SetActive(false);
					this.linkAccountLabelText.gameObject.SetActive(true);
					this.linkAccountURLLabelText.gameObject.SetActive(true);
					this.linkAccountURLText.text = linkURL;
					this.linkAccountURLText.gameObject.SetActive(true);
					this.linkAccountCodeLabelText.gameObject.SetActive(true);
					this.linkAccountCodeText.text = linkCode;
					this.linkAccountCodeText.gameObject.SetActive(true);
				}, delegate(ModIORequestResult result)
				{
					if (!result.success)
					{
						this.linkAccountLabelText.gameObject.SetActive(false);
						this.linkAccountURLLabelText.gameObject.SetActive(false);
						this.linkAccountURLText.gameObject.SetActive(false);
						this.linkAccountCodeLabelText.gameObject.SetActive(false);
						this.linkAccountCodeText.gameObject.SetActive(false);
						this.linkAccountCodeText.text = "";
						this.linkAccountURLText.text = "";
						this.errorText.text = "Failed to authorize with Mod.io. Error:\n " + result.message + "\n\n Press the LINK button to try again.";
						this.errorText.gameObject.SetActive(true);
						this.processingAccountLink = false;
					}
				});
			});
			return;
		}
		this.ResetScreen();
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x000DF200 File Offset: 0x000DD400
	public void NotifyLoggingIn()
	{
		ModIOManager.CancelExternalAuthentication();
		this.modioUsernameLabelText.gameObject.SetActive(false);
		this.modioUsernameText.gameObject.SetActive(false);
		this.linkAccountPromptText.gameObject.SetActive(false);
		this.linkAccountLabelText.gameObject.SetActive(false);
		this.linkAccountURLLabelText.gameObject.SetActive(false);
		this.linkAccountURLText.gameObject.SetActive(false);
		this.linkAccountCodeLabelText.gameObject.SetActive(false);
		this.linkAccountCodeText.gameObject.SetActive(false);
		this.alreadyLinkedAccountText.gameObject.SetActive(false);
		this.errorText.text = "";
		this.errorText.gameObject.SetActive(false);
		this.loggingInText.gameObject.SetActive(true);
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x000DF2E0 File Offset: 0x000DD4E0
	public void DisplayLoginError(string errorMessage)
	{
		ModIOManager.CancelExternalAuthentication();
		this.modioUsernameLabelText.gameObject.SetActive(false);
		this.modioUsernameText.gameObject.SetActive(false);
		this.loggingInText.gameObject.SetActive(false);
		this.linkAccountPromptText.gameObject.SetActive(false);
		this.linkAccountLabelText.gameObject.SetActive(false);
		this.linkAccountURLLabelText.gameObject.SetActive(false);
		this.linkAccountURLText.gameObject.SetActive(false);
		this.linkAccountCodeLabelText.gameObject.SetActive(false);
		this.linkAccountCodeText.gameObject.SetActive(false);
		this.alreadyLinkedAccountText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x000DF3BC File Offset: 0x000DD5BC
	private void ResetScreen()
	{
		this.processingAccountLink = false;
		ModIOManager.CancelExternalAuthentication();
		if (this.isLoggedIn)
		{
			this.modioUsernameLabelText.gameObject.SetActive(true);
			this.modioUsernameText.gameObject.SetActive(true);
		}
		else
		{
			this.modioUsernameLabelText.gameObject.SetActive(false);
			this.modioUsernameText.gameObject.SetActive(false);
			this.modioUsernameText.text = "";
		}
		this.loggingInText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.linkAccountLabelText.gameObject.SetActive(false);
		this.linkAccountURLLabelText.gameObject.SetActive(false);
		this.linkAccountURLText.gameObject.SetActive(false);
		this.linkAccountCodeLabelText.gameObject.SetActive(false);
		this.linkAccountCodeText.gameObject.SetActive(false);
		this.linkAccountPromptText.gameObject.SetActive(false);
		this.alreadyLinkedAccountText.gameObject.SetActive(false);
		if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
		{
			this.linkAccountPromptText.gameObject.SetActive(true);
			return;
		}
		this.alreadyLinkedAccountText.gameObject.SetActive(true);
	}

	// Token: 0x04003363 RID: 13155
	[SerializeField]
	private TMP_Text modioUsernameLabelText;

	// Token: 0x04003364 RID: 13156
	[SerializeField]
	private TMP_Text modioUsernameText;

	// Token: 0x04003365 RID: 13157
	[SerializeField]
	private TMP_Text linkAccountPromptText;

	// Token: 0x04003366 RID: 13158
	[SerializeField]
	private TMP_Text alreadyLinkedAccountText;

	// Token: 0x04003367 RID: 13159
	[SerializeField]
	private TMP_Text linkAccountLabelText;

	// Token: 0x04003368 RID: 13160
	[SerializeField]
	private TMP_Text linkAccountURLLabelText;

	// Token: 0x04003369 RID: 13161
	[SerializeField]
	private TMP_Text linkAccountURLText;

	// Token: 0x0400336A RID: 13162
	[SerializeField]
	private TMP_Text linkAccountCodeLabelText;

	// Token: 0x0400336B RID: 13163
	[SerializeField]
	private TMP_Text linkAccountCodeText;

	// Token: 0x0400336C RID: 13164
	[SerializeField]
	private TMP_Text loggingInText;

	// Token: 0x0400336D RID: 13165
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x0400336E RID: 13166
	private bool processingAccountLink;

	// Token: 0x0400336F RID: 13167
	private bool isLoggedIn;

	// Token: 0x04003370 RID: 13168
	private static UnityEvent OnButtonPress = new UnityEvent();
}
