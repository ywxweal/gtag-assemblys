using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B18 RID: 2840
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x060045E1 RID: 17889 RVA: 0x0014BE33 File Offset: 0x0014A033
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0014BE63 File Offset: 0x0014A063
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0014BE79 File Offset: 0x0014A079
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x0014BE94 File Offset: 0x0014A094
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

		// Token: 0x060045E5 RID: 17893 RVA: 0x0014BEBA File Offset: 0x0014A0BA
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

		// Token: 0x060045E6 RID: 17894 RVA: 0x0014BEE4 File Offset: 0x0014A0E4
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x0400486D RID: 18541
		public static GorillaTimerManager instance;

		// Token: 0x0400486E RID: 18542
		public static bool hasInstance = false;

		// Token: 0x0400486F RID: 18543
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
