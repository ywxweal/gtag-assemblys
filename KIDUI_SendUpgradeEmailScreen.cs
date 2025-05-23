using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200082F RID: 2095
public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	// Token: 0x0600334A RID: 13130 RVA: 0x000FCF88 File Offset: 0x000FB188
	public async void SendUpgradeEmail(List<string> requestedPermissions)
	{
		if (requestedPermissions.Count == 0)
		{
			Debug.Log("[KID] Tried requesting 0 permissions. Skipping upgrade email flow.");
			this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
		}
		else
		{
			base.gameObject.SetActive(true);
			this._animatedEllipsis.StartAnimation();
			UpgradeSessionData upgradeSessionData = await KIDManager.TryUpgradeSession(requestedPermissions);
			if (upgradeSessionData == null)
			{
				this.OnFailure();
			}
			else if (upgradeSessionData.status == SessionStatus.PASS)
			{
				this.OnSuccess();
			}
			else if (upgradeSessionData.status == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				TaskAwaiter<bool> taskAwaiter = KIDManager.TrySendUpgradeSessionChallengeEmail().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					this.OnSuccess();
				}
				else
				{
					this.OnFailure();
				}
			}
			else
			{
				Debug.LogError("[KID] Unexpected session status when upgrading session: " + upgradeSessionData.status.ToString());
				this.OnFailure();
			}
		}
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x000FCFC7 File Offset: 0x000FB1C7
	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x000FCFE1 File Offset: 0x000FB1E1
	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show();
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x000FCFFA File Offset: 0x000FB1FA
	private void OnFailure()
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show();
	}

	// Token: 0x04003A1E RID: 14878
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04003A1F RID: 14879
	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	// Token: 0x04003A20 RID: 14880
	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	// Token: 0x04003A21 RID: 14881
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
