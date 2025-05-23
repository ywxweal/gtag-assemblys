using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x02000C5D RID: 3165
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06004E8A RID: 20106 RVA: 0x00176CBA File Offset: 0x00174EBA
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06004E8B RID: 20107 RVA: 0x00176CC3 File Offset: 0x00174EC3
		// (set) Token: 0x06004E8C RID: 20108 RVA: 0x00176CCB File Offset: 0x00174ECB
		public bool IsReturningPlayer { get; private set; }

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06004E8D RID: 20109 RVA: 0x00176CD4 File Offset: 0x00174ED4
		// (set) Token: 0x06004E8E RID: 20110 RVA: 0x00176CDC File Offset: 0x00174EDC
		public bool postAuthSetSafety { get; private set; }

		// Token: 0x06004E8F RID: 20111 RVA: 0x00176CE8 File Offset: 0x00174EE8
		private void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (PlayFabAuthenticator.instance.photonAuthenticator == null)
			{
				PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
			}
			this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			if (PlayFabAuthenticator.instance.steamAuthenticator == null)
			{
				PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
			}
			this.platform.PlatformTag = "Steam";
			PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
			PlayFabSettings.DisableFocusTimeCollection = true;
			this.BeginLoginFlow();
		}

		// Token: 0x06004E90 RID: 20112 RVA: 0x00176DEC File Offset: 0x00174FEC
		public void BeginLoginFlow()
		{
			if (!MothershipClientApiUnity.IsEnabled())
			{
				this.AuthenticateWithPlayFab();
				return;
			}
			if (PlayFabAuthenticator.instance.mothershipAuthenticator == null)
			{
				PlayFabAuthenticator.instance.mothershipAuthenticator = MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>();
				MothershipAuthenticator mothershipAuthenticator = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator.OnLoginSuccess = (Action)Delegate.Combine(mothershipAuthenticator.OnLoginSuccess, new Action(delegate
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}));
				MothershipAuthenticator mothershipAuthenticator2 = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator2.OnLoginAttemptFailure = (Action<int>)Delegate.Combine(mothershipAuthenticator2.OnLoginAttemptFailure, new Action<int>(delegate(int attempts)
				{
					if (attempts == 1)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
				}));
				PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
			}
		}

		// Token: 0x06004E91 RID: 20113 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Start()
		{
		}

		// Token: 0x06004E92 RID: 20114 RVA: 0x00176ED7 File Offset: 0x001750D7
		private void OnEnable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse += this.OnCustomAuthenticationResponse;
		}

		// Token: 0x06004E93 RID: 20115 RVA: 0x00176EEF File Offset: 0x001750EF
		private void OnDisable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse -= this.OnCustomAuthenticationResponse;
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			SteamAuthTicket steamAuthTicket2 = this.steamAuthTicketForPlayFab;
			if (steamAuthTicket2 == null)
			{
				return;
			}
			steamAuthTicket2.Dispose();
		}

		// Token: 0x06004E94 RID: 20116 RVA: 0x00176F28 File Offset: 0x00175128
		public void RefreshSteamAuthTicketForPhoton(Action<string> successCallback, Action<EResult> failureCallback)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			this.steamAuthTicketForPhoton = this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x00176F5C File Offset: 0x0017515C
		private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			object obj;
			if (response.TryGetValue("SteamAuthIdForPhoton", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					this.steamAuthIdForPhoton = text;
					return;
				}
			}
			this.steamAuthIdForPhoton = null;
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x00176FA4 File Offset: 0x001751A4
		public void AuthenticateWithPlayFab()
		{
			Debug.Log("authenticating with playFab!");
			GorillaServer gorillaServer = GorillaServer.Instance;
			if (gorillaServer != null && gorillaServer.FeatureFlagsReady)
			{
				if (KIDManager.KidEnabled)
				{
					Debug.Log("[KID] Is Enabled - Enabling safeties by platform and age category");
					this.DefaultSafetiesByAgeCategory();
				}
			}
			else
			{
				this.postAuthSetSafety = true;
			}
			if (SteamManager.Initialized)
			{
				this.userID = SteamUser.GetSteamID().ToString();
				Debug.Log("trying to auth with steam");
				this.steamAuthTicketForPlayFab = this.steamAuthenticator.GetAuthTicket(delegate(string ticket)
				{
					Debug.Log("Got steam auth session ticket!");
					PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
					{
						CreateAccount = new bool?(true),
						SteamTicket = ticket
					}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
				}, delegate(EResult result)
				{
					base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
				});
				return;
			}
			base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x00177056 File Offset: 0x00175256
		private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
		{
			Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
			yield return new WaitUntil(() => getNewPlayerDateTimeTask.IsCompleted);
			DateTime? result = getNewPlayerDateTimeTask.Result;
			if (result != null && KIDManager.KidEnabled)
			{
				this.IsReturningPlayer = accountCreationDateTime < result;
			}
			yield break;
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x0017706C File Offset: 0x0017526C
		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x0017707C File Offset: 0x0017527C
		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform.ToString(),
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabPlayerIdCache,
				TitleId = PlayFabSettings.TitleId,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipToken = MothershipClientContext.Token,
				MothershipId = MothershipClientContext.MothershipId
			}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x00177114 File Offset: 0x00175314
		private void OnCachePlayFabIdRequest([CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
		{
			if (response != null)
			{
				this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
				DateTime dateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(dateTime));
				}
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x00177172 File Offset: 0x00175372
		private void AdvanceLogin()
		{
			this.LogMessage("PlayFab authenticated ... Getting Nonce");
			this.RefreshSteamAuthTicketForPhoton(delegate(string ticket)
			{
				this._nonce = ticket;
				Debug.Log("Got nonce!  Authenticating...");
				this.AuthenticateWithPhoton();
			}, delegate(EResult result)
			{
				Debug.LogWarning("Failed to get nonce!");
				this.AuthenticateWithPhoton();
			});
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x001771A0 File Offset: 0x001753A0
		private void AuthenticateWithPhoton()
		{
			this.photonAuthenticator.SetCustomAuthenticationParameters(new Dictionary<string, object>
			{
				{
					"AppId",
					PlayFabSettings.TitleId
				},
				{
					"AppVersion",
					NetworkSystemConfig.AppVersion ?? "-1"
				},
				{ "Ticket", this._sessionTicket },
				{ "Nonce", this._nonce },
				{
					"MothershipEnvId",
					MothershipClientApiUnity.EnvironmentId
				},
				{
					"MothershipToken",
					MothershipClientContext.Token
				}
			});
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			GorillaServer.Instance.AddOrRemoveDLCOwnership(delegate(ExecuteFunctionResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			});
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (PhotonNetworkController.Instance != null)
			{
				Debug.Log("Finish authenticating");
				NetworkSystem.Instance.FinishAuthenticating();
			}
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x001772EE File Offset: 0x001754EE
		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x00177300 File Offset: 0x00175500
		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.loginFailed = true;
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x000023F4 File Offset: 0x000005F4
		private void LogMessage(string message)
		{
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x00177538 File Offset: 0x00175738
		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x00177598 File Offset: 0x00175798
		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName))
			{
				UpdateUserTitleDisplayNameRequest updateUserTitleDisplayNameRequest = new UpdateUserTitleDisplayNameRequest();
				updateUserTitleDisplayNameRequest.DisplayName = playerName;
				PlayFabClientAPI.UpdateUserTitleDisplayName(updateUserTitleDisplayNameRequest, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError(error.GenerateErrorReport());
				}, null, null);
			}
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x00177638 File Offset: 0x00175838
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x00177664 File Offset: 0x00175864
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x00177676 File Offset: 0x00175876
		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 15;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData playfabAuthResponseData = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback(playfabAuthResponseData);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback(null);
				}
				if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
					Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSeconds((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x00177694 File Offset: 0x00175894
		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004EA6 RID: 20134 RVA: 0x00177748 File Offset: 0x00175948
		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 15;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse cachePlayFabIdResponse = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback(cachePlayFabIdResponse);
				}
			}
			else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else
			{
				retry = request.result != UnityWebRequest.Result.ConnectionError || true;
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSeconds((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform.ToString(),
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabPlayerIdCache,
						TitleId = PlayFabSettings.TitleId,
						MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
						MothershipToken = MothershipClientContext.Token,
						MothershipId = MothershipClientContext.MothershipId
					}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x00177765 File Offset: 0x00175965
		public void DefaultSafetiesByAgeCategory()
		{
			Debug.Log("[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
			this.SetSafety(false, true, false);
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x0017777C File Offset: 0x0017597C
		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			this.postAuthSetSafety = false;
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate(isSafety);
			}
			Debug.Log("[KID] Setting safety to: [" + isSafety.ToString() + "]");
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (!isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 0);
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 0);
				}
				PlayerPrefs.Save();
				return;
			}
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 1);
				this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				return;
			}
			PlayerPrefs.SetInt("optSafety", 1);
			this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x00177817 File Offset: 0x00175A17
		public string GetPlayFabSessionTicket()
		{
			return this._sessionTicket;
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x0017781F File Offset: 0x00175A1F
		public string GetPlayFabPlayerId()
		{
			return this._playFabPlayerIdCache;
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x00177827 File Offset: 0x00175A27
		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x0017782F File Offset: 0x00175A2F
		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		// Token: 0x06004EAD RID: 20141 RVA: 0x00177837 File Offset: 0x00175A37
		public string GetUserID()
		{
			return this.userID;
		}

		// Token: 0x040051AA RID: 20906
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x040051AB RID: 20907
		private string _playFabPlayerIdCache;

		// Token: 0x040051AC RID: 20908
		private string _sessionTicket;

		// Token: 0x040051AD RID: 20909
		private string _displayName;

		// Token: 0x040051AE RID: 20910
		private string _nonce;

		// Token: 0x040051AF RID: 20911
		public string userID;

		// Token: 0x040051B0 RID: 20912
		private string userToken;

		// Token: 0x040051B1 RID: 20913
		public PlatformTagJoin platform;

		// Token: 0x040051B2 RID: 20914
		private bool isSafeAccount;

		// Token: 0x040051B3 RID: 20915
		public Action<bool> OnSafetyUpdate;

		// Token: 0x040051B4 RID: 20916
		private PlayFabAuthenticator.SafetyType safetyType;

		// Token: 0x040051B5 RID: 20917
		private byte[] m_Ticket;

		// Token: 0x040051B6 RID: 20918
		private uint m_pcbTicket;

		// Token: 0x040051B7 RID: 20919
		public Text debugText;

		// Token: 0x040051B8 RID: 20920
		public bool screenDebugMode;

		// Token: 0x040051B9 RID: 20921
		public bool loginFailed;

		// Token: 0x040051BA RID: 20922
		[FormerlySerializedAs("loginDisplayID")]
		public GameObject emptyObject;

		// Token: 0x040051BB RID: 20923
		private int playFabAuthRetryCount;

		// Token: 0x040051BC RID: 20924
		private int playFabMaxRetries = 5;

		// Token: 0x040051BD RID: 20925
		private int playFabCacheRetryCount;

		// Token: 0x040051BE RID: 20926
		private int playFabCacheMaxRetries = 5;

		// Token: 0x040051BF RID: 20927
		public MetaAuthenticator metaAuthenticator;

		// Token: 0x040051C0 RID: 20928
		public SteamAuthenticator steamAuthenticator;

		// Token: 0x040051C1 RID: 20929
		public MothershipAuthenticator mothershipAuthenticator;

		// Token: 0x040051C2 RID: 20930
		public PhotonAuthenticator photonAuthenticator;

		// Token: 0x040051C3 RID: 20931
		[SerializeField]
		private bool dbg_isReturningPlayer;

		// Token: 0x040051C5 RID: 20933
		private SteamAuthTicket steamAuthTicketForPlayFab;

		// Token: 0x040051C6 RID: 20934
		private SteamAuthTicket steamAuthTicketForPhoton;

		// Token: 0x040051C7 RID: 20935
		private string steamAuthIdForPhoton;

		// Token: 0x02000C5E RID: 3166
		public enum SafetyType
		{
			// Token: 0x040051CA RID: 20938
			None,
			// Token: 0x040051CB RID: 20939
			Auto,
			// Token: 0x040051CC RID: 20940
			OptIn
		}

		// Token: 0x02000C5F RID: 3167
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x040051CD RID: 20941
			public string Platform;

			// Token: 0x040051CE RID: 20942
			public string SessionTicket;

			// Token: 0x040051CF RID: 20943
			public string PlayFabId;

			// Token: 0x040051D0 RID: 20944
			public string TitleId;

			// Token: 0x040051D1 RID: 20945
			public string MothershipEnvId;

			// Token: 0x040051D2 RID: 20946
			public string MothershipToken;

			// Token: 0x040051D3 RID: 20947
			public string MothershipId;
		}

		// Token: 0x02000C60 RID: 3168
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x040051D4 RID: 20948
			public string AppId;

			// Token: 0x040051D5 RID: 20949
			public string Nonce;

			// Token: 0x040051D6 RID: 20950
			public string OculusId;

			// Token: 0x040051D7 RID: 20951
			public string Platform;

			// Token: 0x040051D8 RID: 20952
			public string AgeCategory;

			// Token: 0x040051D9 RID: 20953
			public string MothershipEnvId;

			// Token: 0x040051DA RID: 20954
			public string MothershipToken;

			// Token: 0x040051DB RID: 20955
			public string MothershipId;
		}

		// Token: 0x02000C61 RID: 3169
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x040051DC RID: 20956
			public string SessionTicket;

			// Token: 0x040051DD RID: 20957
			public string EntityToken;

			// Token: 0x040051DE RID: 20958
			public string PlayFabId;

			// Token: 0x040051DF RID: 20959
			public string EntityId;

			// Token: 0x040051E0 RID: 20960
			public string EntityType;

			// Token: 0x040051E1 RID: 20961
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000C62 RID: 3170
		[Serializable]
		public class CachePlayFabIdResponse
		{
			// Token: 0x040051E2 RID: 20962
			public string PlayFabId;

			// Token: 0x040051E3 RID: 20963
			public string SteamAuthIdForPhoton;

			// Token: 0x040051E4 RID: 20964
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000C63 RID: 3171
		public class BanInfo
		{
			// Token: 0x040051E5 RID: 20965
			public string BanMessage;

			// Token: 0x040051E6 RID: 20966
			public string BanExpirationTime;
		}
	}
}
