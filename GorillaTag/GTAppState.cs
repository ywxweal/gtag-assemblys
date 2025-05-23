using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D0F RID: 3343
	public static class GTAppState
	{
		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060053D1 RID: 21457 RVA: 0x0019678C File Offset: 0x0019498C
		// (set) Token: 0x060053D2 RID: 21458 RVA: 0x00196793 File Offset: 0x00194993
		[OnEnterPlay_Set(false)]
		public static bool isQuitting { get; private set; }

		// Token: 0x060053D3 RID: 21459 RVA: 0x0019679C File Offset: 0x0019499C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		// Token: 0x060053D4 RID: 21460 RVA: 0x000023F4 File Offset: 0x000005F4
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
