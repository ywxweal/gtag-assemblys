using System;
using UnityEngine;

// Token: 0x020006DA RID: 1754
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002BAA RID: 11178 RVA: 0x000D749D File Offset: 0x000D569D
	// (set) Token: 0x06002BAB RID: 11179 RVA: 0x000D74A5 File Offset: 0x000D56A5
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

	// Token: 0x06002BAC RID: 11180 RVA: 0x000D74BD File Offset: 0x000D56BD
	public void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x040031B3 RID: 12723
	public DevButtonType Type;

	// Token: 0x040031B4 RID: 12724
	public LogType levelType;

	// Token: 0x040031B5 RID: 12725
	public DevConsoleInstance targetConsole;

	// Token: 0x040031B6 RID: 12726
	public int lineNumber;

	// Token: 0x040031B7 RID: 12727
	public bool repeatIfHeld;

	// Token: 0x040031B8 RID: 12728
	public float holdForSeconds;

	// Token: 0x040031B9 RID: 12729
	private Coroutine pressCoroutine;
}
