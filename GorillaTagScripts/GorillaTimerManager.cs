using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B18 RID: 2840
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x060045E0 RID: 17888 RVA: 0x0014BD5B File Offset: 0x00149F5B
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x0014BD8B File Offset: 0x00149F8B
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0014BDA1 File Offset: 0x00149FA1
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0014BDBC File Offset: 0x00149FBC
		public static void RegisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (!GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x0014BDE2 File Offset: 0x00149FE2
		public static void UnregisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x0014BE0C File Offset: 0x0014A00C
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x0400486C RID: 18540
		public static GorillaTimerManager instance;

		// Token: 0x0400486D RID: 18541
		public static bool hasInstance = false;

		// Token: 0x0400486E RID: 18542
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
