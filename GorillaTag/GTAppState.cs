using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D0F RID: 3343
	public static class GTAppState
	{
		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x060053D2 RID: 21458 RVA: 0x00196864 File Offset: 0x00194A64
		// (set) Token: 0x060053D3 RID: 21459 RVA: 0x0019686B File Offset: 0x00194A6B
		[OnEnterPlay_Set(false)]
		public static bool isQuitting { get; private set; }

		// Token: 0x060053D4 RID: 21460 RVA: 0x00196874 File Offset: 0x00194A74
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

		// Token: 0x060053D5 RID: 21461 RVA: 0x000023F4 File Offset: 0x000005F4
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
