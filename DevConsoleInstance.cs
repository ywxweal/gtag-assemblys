using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x06000A99 RID: 2713 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000D05 RID: 3333
	public GorillaDevButton[] buttons;

	// Token: 0x04000D06 RID: 3334
	public GameObject[] disableWhileActive;

	// Token: 0x04000D07 RID: 3335
	public GameObject[] enableWhileActive;

	// Token: 0x04000D08 RID: 3336
	public float maxHeight;

	// Token: 0x04000D09 RID: 3337
	public float lineHeight;

	// Token: 0x04000D0A RID: 3338
	public int targetLogIndex = -1;

	// Token: 0x04000D0B RID: 3339
	public int currentLogIndex;

	// Token: 0x04000D0C RID: 3340
	public int expandAmount = 20;

	// Token: 0x04000D0D RID: 3341
	public int expandedMessageIndex = -1;

	// Token: 0x04000D0E RID: 3342
	public bool canExpand = true;

	// Token: 0x04000D0F RID: 3343
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04000D10 RID: 3344
	public HashSet<LogType> selectedLogTypes = new HashSet<LogType>
	{
		LogType.Error,
		LogType.Exception,
		LogType.Log,
		LogType.Warning,
		LogType.Assert
	};

	// Token: 0x04000D11 RID: 3345
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x04000D12 RID: 3346
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x04000D13 RID: 3347
	public float lineStartHeight;

	// Token: 0x04000D14 RID: 3348
	public float lineStartZ;

	// Token: 0x04000D15 RID: 3349
	public float textStartHeight;

	// Token: 0x04000D16 RID: 3350
	public float lineStartTextWidth;

	// Token: 0x04000D17 RID: 3351
	public double textScale = 0.5;

	// Token: 0x04000D18 RID: 3352
	public bool isEnabled = true;

	// Token: 0x04000D19 RID: 3353
	[SerializeField]
	private GameObject ConsoleLineExample;
}
