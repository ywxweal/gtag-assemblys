using System;
using System.Threading;
using UnityEngine;

// Token: 0x020007FC RID: 2044
public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	// Token: 0x06003220 RID: 12832 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x06003221 RID: 12833 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnEnable()
	{
	}

	// Token: 0x06003222 RID: 12834 RVA: 0x000F7B26 File Offset: 0x000F5D26
	public void ShowRestrictedAccessScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	public void OnChangeAgePressed()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040038D5 RID: 14549
	[SerializeField]
	private KIDUIButton _changeAgeButton;

	// Token: 0x040038D6 RID: 14550
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x040038D7 RID: 14551
	private string _submittedEmailAddress;

	// Token: 0x040038D8 RID: 14552
	private CancellationTokenSource _cancellationTokenSource;
}
