using System;
using UnityEngine;

// Token: 0x020006DA RID: 1754
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002BA9 RID: 11177 RVA: 0x000D73F9 File Offset: 0x000D55F9
	// (set) Token: 0x06002BAA RID: 11178 RVA: 0x000D7401 File Offset: 0x000D5601
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x000D7419 File Offset: 0x000D5619
	public void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x040031B1 RID: 12721
	public DevButtonType Type;

	// Token: 0x040031B2 RID: 12722
	public LogType levelType;

	// Token: 0x040031B3 RID: 12723
	public DevConsoleInstance targetConsole;

	// Token: 0x040031B4 RID: 12724
	public int lineNumber;

	// Token: 0x040031B5 RID: 12725
	public bool repeatIfHeld;

	// Token: 0x040031B6 RID: 12726
	public float holdForSeconds;

	// Token: 0x040031B7 RID: 12727
	private Coroutine pressCoroutine;
}
