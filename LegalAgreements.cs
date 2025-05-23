using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.CloudScriptModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000844 RID: 2116
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06003398 RID: 13208 RVA: 0x000FE7A6 File Offset: 0x000FC9A6
	// (set) Token: 0x06003399 RID: 13209 RVA: 0x000FE7AD File Offset: 0x000FC9AD
	public static LegalAgreements instance { get; private set; }

	// Token: 0x0600339A RID: 13210 RVA: 0x000FE7B8 File Offset: 0x000FC9B8
	private void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	// Token: 0x0600339B RID: 13211 RVA: 0x000FE81C File Offset: 0x000FCA1C
	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (this.controllerBehaviour.IsUpStick || this.controllerBehaviour.IsDownStick)
		{
			if (this.controllerBehaviour.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(this.controllerBehaviour.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x000FE9D4 File Offset: 0x000FCBD4
	public async Task StartLegalAgreements()
	{
		if (!this.legalAgreementsStarted)
		{
			this.legalAgreementsStarted = true;
			while (!PlayFabClientAPI.IsClientLoggedIn())
			{
				if (PlayFabAuthenticator.instance && PlayFabAuthenticator.instance.loginFailed)
				{
					return;
				}
				await Task.Yield();
			}
			Dictionary<string, string> agreementResults = await this.GetAcceptedAgreements(this.legalAgreementScreens);
			foreach (LegalAgreementTextAsset screen in this.legalAgreementScreens)
			{
				string latestVersion = await this.GetTitleDataAsync(screen.latestVersionKey);
				if (!string.IsNullOrEmpty(latestVersion))
				{
					latestVersion = latestVersion.Substring(1, latestVersion.Length - 2);
					string empty = string.Empty;
					if (agreementResults == null || !agreementResults.TryGetValue(screen.playFabKey, out empty) || !(latestVersion == empty))
					{
						base.enabled = true;
						PrivateUIRoom.ForceStartOverlay();
						if (!screen.confirmString.IsNullOrEmpty())
						{
							this._pressAndHoldToConfirmButton.SetText(screen.confirmString);
						}
						PrivateUIRoom.AddUI(this.uiParent);
						HandRayController.Instance.EnableHandRays();
						TaskAwaiter<bool> taskAwaiter = this.UpdateText(screen, latestVersion).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							for (;;)
							{
								await Task.Yield();
							}
						}
						else
						{
							await this.WaitForAcknowledgement();
							this.scrollBar.value = 1f;
							PrivateUIRoom.RemoveUI(this.uiParent);
							if (agreementResults == null)
							{
								agreementResults = new Dictionary<string, string>();
							}
							agreementResults.AddOrUpdate(screen.playFabKey, latestVersion);
							if (this.optIn)
							{
								LegalAgreementTextAsset.PostAcceptAction optInAction = screen.optInAction;
							}
							latestVersion = null;
							screen = null;
						}
					}
				}
			}
			LegalAgreementTextAsset[] array = null;
			base.enabled = false;
			await this.SubmitAcceptedAgreements(agreementResults);
		}
	}

	// Token: 0x0600339D RID: 13213 RVA: 0x000FEA17 File Offset: 0x000FCC17
	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	// Token: 0x0600339E RID: 13214 RVA: 0x000FEA20 File Offset: 0x000FCC20
	private async Task WaitForAcknowledgement()
	{
		this._accepted = false;
		while (!this._accepted)
		{
			await Task.Yield();
		}
		this._accepted = false;
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x000FEA64 File Offset: 0x000FCC64
	private async Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		this.optional = asset.optional;
		this.tmpTitle.text = asset.title;
		bool flag = await this.UpdateTextFromPlayFabTitleData(asset.playFabKey, version, this.tmpBody);
		if (!flag)
		{
			this.tmpBody.text = asset.errorMessage + "\n\nPlease restart the game and try again.";
			this.scrollBar.value = 0f;
			this.scrollBar.size = 1f;
		}
		return flag;
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x000FEAB8 File Offset: 0x000FCCB8
	public async Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		string text = key + "_" + version;
		this.state = 0;
		PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(this.OnTitleDataReceived), new Action<PlayFabError>(this.OnPlayFabError));
		while (this.state == 0)
		{
			await Task.Yield();
		}
		bool flag;
		if (this.state == 1)
		{
			target.text = Regex.Unescape(this.cachedText.Substring(1, this.cachedText.Length - 2));
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x000FEB13 File Offset: 0x000FCD13
	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x000FEB1C File Offset: 0x000FCD1C
	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x000FEB2C File Offset: 0x000FCD2C
	private async Task<string> GetTitleDataAsync(string key)
	{
		int state = 0;
		string result = null;
		PlayFabTitleDataCache.Instance.GetTitleData(key, delegate(string res)
		{
			result = res;
			state = 1;
		}, delegate(PlayFabError err)
		{
			result = null;
			state = -1;
			Debug.LogError(err.ErrorMessage);
		});
		while (state == 0)
		{
			await Task.Yield();
		}
		return (state == 1) ? result : null;
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x000FEB70 File Offset: 0x000FCD70
	private async Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		int state = 0;
		Dictionary<string, string> returnValue = new Dictionary<string, string>();
		string[] array = agreements.Select((LegalAgreementTextAsset x) => x.playFabKey).ToArray<string>();
		GorillaServer.Instance.GetAcceptedAgreements(new GetAcceptedAgreementsRequest
		{
			AgreementKeys = array
		}, delegate(Dictionary<string, string> result)
		{
			state = 1;
			returnValue = result;
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.ErrorMessage);
			state = -1;
		});
		while (state == 0)
		{
			await Task.Yield();
		}
		return returnValue;
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x000FEBB4 File Offset: 0x000FCDB4
	private async Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		int state = 0;
		GorillaServer.Instance.SubmitAcceptedAgreements(new SubmitAcceptedAgreementsRequest
		{
			Agreements = agreements
		}, delegate(ExecuteFunctionResult result)
		{
			state = 1;
		}, delegate(PlayFabError error)
		{
			state = -1;
		});
		while (state == 0)
		{
			await Task.Yield();
		}
	}

	// Token: 0x04003A8B RID: 14987
	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	// Token: 0x04003A8C RID: 14988
	[Header("Scroll Behavior")]
	[SerializeField]
	private float _minScrollSpeed = 0.02f;

	// Token: 0x04003A8D RID: 14989
	[SerializeField]
	private float _maxScrollSpeed = 3f;

	// Token: 0x04003A8E RID: 14990
	[SerializeField]
	private float _scrollInterpTime = 3f;

	// Token: 0x04003A8F RID: 14991
	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003A90 RID: 14992
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x04003A92 RID: 14994
	[SerializeField]
	private Transform uiParent;

	// Token: 0x04003A93 RID: 14995
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x04003A94 RID: 14996
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x04003A95 RID: 14997
	[SerializeField]
	private Scrollbar scrollBar;

	// Token: 0x04003A96 RID: 14998
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x04003A97 RID: 14999
	[SerializeField]
	private KIDUIButton _pressAndHoldToConfirmButton;

	// Token: 0x04003A98 RID: 15000
	[SerializeField]
	private TMP_Text _scrollToBottomText;

	// Token: 0x04003A99 RID: 15001
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04003A9A RID: 15002
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04003A9B RID: 15003
	private float stickHeldDuration;

	// Token: 0x04003A9C RID: 15004
	private float scrollSpeed;

	// Token: 0x04003A9D RID: 15005
	private float scrollTime;

	// Token: 0x04003A9E RID: 15006
	private bool legalAgreementsStarted;

	// Token: 0x04003A9F RID: 15007
	private bool _accepted;

	// Token: 0x04003AA0 RID: 15008
	private string cachedText;

	// Token: 0x04003AA1 RID: 15009
	private int state;

	// Token: 0x04003AA2 RID: 15010
	private bool optIn;

	// Token: 0x04003AA3 RID: 15011
	private bool optional;
}
