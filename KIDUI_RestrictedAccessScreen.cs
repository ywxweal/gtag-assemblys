using System;
using UnityEngine;

// Token: 0x0200082E RID: 2094
public class KIDUI_RestrictedAccessScreen : MonoBehaviour
{
	// Token: 0x06003347 RID: 13127 RVA: 0x000FCEE4 File Offset: 0x000FB0E4
	public void ShowRestrictedAccessScreen(SessionStatus? sessionStatus)
	{
		base.gameObject.SetActive(true);
		this._pendingStatusIndicator.SetActive(false);
		this._prohibitedStatusIndicator.SetActive(false);
		if (sessionStatus == null)
		{
			return;
		}
		if (sessionStatus != null)
		{
			switch (sessionStatus.GetValueOrDefault())
			{
			case SessionStatus.PASS:
			case SessionStatus.CHALLENGE:
			case SessionStatus.CHALLENGE_SESSION_UPGRADE:
				break;
			case SessionStatus.PROHIBITED:
				this._prohibitedStatusIndicator.SetActive(true);
				return;
			case SessionStatus.PENDING_AGE_APPEAL:
				this._pendingStatusIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x000FCF64 File Offset: 0x000FB164
	public void OnChangeAgePressed()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		base.gameObject.SetActive(false);
		this._ageAppealScreen.ShowAgeAppealScreen();
	}

	// Token: 0x04003A1B RID: 14875
	[SerializeField]
	private KIDAgeAppeal _ageAppealScreen;

	// Token: 0x04003A1C RID: 14876
	[SerializeField]
	private GameObject _pendingStatusIndicator;

	// Token: 0x04003A1D RID: 14877
	[SerializeField]
	private GameObject _prohibitedStatusIndicator;
}
