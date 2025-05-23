using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

// Token: 0x020008E2 RID: 2274
public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003743 RID: 14147 RVA: 0x0010B5EC File Offset: 0x001097EC
	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x0010B680 File Offset: 0x00109880
	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x0010B692 File Offset: 0x00109892
	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure();
		});
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x0010B6BE File Offset: 0x001098BE
	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				string nonce2 = nonce;
				Action<LoginResponse> action;
				if ((action = <>9__4) == null)
				{
					action = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess();
					});
				}
				Action<MothershipError, int> action2;
				if ((action2 = <>9__5) == null)
				{
					action2 = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.Log(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure(1);
						}
						Action onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure();
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce2, ticket, action, action2);
			}, delegate(EResult error)
			{
				Debug.Log(string.Format("Couldn't get an auth ticket for logging into Mothership with Steam {0}", error));
				Action<int> onLoginAttemptFailure2 = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure2 != null)
				{
					onLoginAttemptFailure2(1);
				}
				Action onLoginFailure2 = this.OnLoginFailure;
				if (onLoginFailure2 == null)
				{
					return;
				}
				onLoginFailure2();
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
			Action<int> onLoginAttemptFailure3 = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure3 != null)
			{
				onLoginAttemptFailure3(1);
			}
			Action onLoginFailure3 = this.OnLoginFailure;
			if (onLoginFailure3 == null)
			{
				return;
			}
			onLoginFailure3();
		});
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x0010B6DE File Offset: 0x001098DE
	public void SliceUpdate()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			MothershipClientApiUnity.Tick(Time.deltaTime);
		}
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04003CDE RID: 15582
	public static volatile MothershipAuthenticator Instance;

	// Token: 0x04003CDF RID: 15583
	public MetaAuthenticator MetaAuthenticator;

	// Token: 0x04003CE0 RID: 15584
	public SteamAuthenticator SteamAuthenticator;

	// Token: 0x04003CE1 RID: 15585
	public string TestNickname;

	// Token: 0x04003CE2 RID: 15586
	public string TestAccountId;

	// Token: 0x04003CE3 RID: 15587
	public bool UseConstantTestAccountId = true;

	// Token: 0x04003CE4 RID: 15588
	public int MaxMetaLoginAttempts = 5;

	// Token: 0x04003CE5 RID: 15589
	public Action OnLoginSuccess;

	// Token: 0x04003CE6 RID: 15590
	public Action OnLoginFailure;

	// Token: 0x04003CE7 RID: 15591
	public Action<int> OnLoginAttemptFailure;
}
