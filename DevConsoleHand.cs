using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B7 RID: 439
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x04000CF4 RID: 3316
	public List<GameObject> otherButtonsList;

	// Token: 0x04000CF5 RID: 3317
	public bool isStillEnabled = true;

	// Token: 0x04000CF6 RID: 3318
	public bool isLeftHand;

	// Token: 0x04000CF7 RID: 3319
	public ConsoleMode mode;

	// Token: 0x04000CF8 RID: 3320
	public double debugScale;

	// Token: 0x04000CF9 RID: 3321
	public double inspectorScale;

	// Token: 0x04000CFA RID: 3322
	public double componentInspectorScale;

	// Token: 0x04000CFB RID: 3323
	public List<GameObject> consoleButtons;

	// Token: 0x04000CFC RID: 3324
	public List<GameObject> inspectorButtons;

	// Token: 0x04000CFD RID: 3325
	public List<GameObject> componentInspectorButtons;

	// Token: 0x04000CFE RID: 3326
	public GorillaDevButton consoleButton;

	// Token: 0x04000CFF RID: 3327
	public GorillaDevButton inspectorButton;

	// Token: 0x04000D00 RID: 3328
	public GorillaDevButton componentInspectorButton;

	// Token: 0x04000D01 RID: 3329
	public GorillaDevButton showNonStarItems;

	// Token: 0x04000D02 RID: 3330
	public GorillaDevButton showPrivateItems;

	// Token: 0x04000D03 RID: 3331
	public Text componentInspectionText;

	// Token: 0x04000D04 RID: 3332
	public DevInspector selectedInspector;
}
