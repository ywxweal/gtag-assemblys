using System;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using ModIO;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000481 RID: 1153
public class GameEvents
{
	// Token: 0x04001F6B RID: 8043
	public static UnityEvent<GorillaKeyboardBindings> OnGorrillaKeyboardButtonPressedEvent = new UnityEvent<GorillaKeyboardBindings>();

	// Token: 0x04001F6C RID: 8044
	public static UnityEvent<GorillaATMKeyBindings> OnGorrillaATMKeyButtonPressedEvent = new UnityEvent<GorillaATMKeyBindings>();

	// Token: 0x04001F6D RID: 8045
	internal static UnityEvent<string> ScreenTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04001F6E RID: 8046
	internal static UnityEvent<Material[]> ScreenTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04001F6F RID: 8047
	internal static UnityEvent<string> FunctionSelectTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04001F70 RID: 8048
	internal static UnityEvent<Material[]> FunctionTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04001F71 RID: 8049
	internal static UnityEvent<string> ScoreboardTextChangedEvent = new UnityEvent<string>();

	// Token: 0x04001F72 RID: 8050
	internal static UnityEvent<Material[]> ScoreboardMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x04001F73 RID: 8051
	public static UnityEvent OnModIOLoggedIn = new UnityEvent();

	// Token: 0x04001F74 RID: 8052
	public static UnityEvent OnModIOLoggedOut = new UnityEvent();

	// Token: 0x04001F75 RID: 8053
	public static UnityEvent<CustomMapsTerminalButton.ModIOKeyboardBindings> OnModIOKeyboardButtonPressedEvent = new UnityEvent<CustomMapsTerminalButton.ModIOKeyboardBindings>();

	// Token: 0x04001F76 RID: 8054
	public static UnityEvent<ModManagementEventType, ModId, Result> ModIOModManagementEvent = new UnityEvent<ModManagementEventType, ModId, Result>();

	// Token: 0x04001F77 RID: 8055
	public static UnityEvent<SharedBlocksKeyboardBindings> OnSharedBlocksKeyboardButtonPressedEvent = new UnityEvent<SharedBlocksKeyboardBindings>();
}
