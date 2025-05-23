using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CB RID: 1227
public class MonkeBallTeamSelector : MonoBehaviour
{
	// Token: 0x06001DC5 RID: 7621 RVA: 0x00090CDE File Offset: 0x0008EEDE
	public void Awake()
	{
		this._setTeamButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x00090CFC File Offset: 0x0008EEFC
	public void OnDestroy()
	{
		this._setTeamButton.onPressButton.RemoveListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x00090D1A File Offset: 0x0008EF1A
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestSetTeam(this.teamId);
	}

	// Token: 0x040020E6 RID: 8422
	public int teamId;

	// Token: 0x040020E7 RID: 8423
	[SerializeField]
	private GorillaPressableButton _setTeamButton;
}
