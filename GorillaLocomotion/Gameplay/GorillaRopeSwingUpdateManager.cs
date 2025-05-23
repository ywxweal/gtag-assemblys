using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CDD RID: 3293
	public class GorillaRopeSwingUpdateManager : MonoBehaviour
	{
		// Token: 0x060051B4 RID: 20916 RVA: 0x0018CB7A File Offset: 0x0018AD7A
		protected void Awake()
		{
			if (GorillaRopeSwingUpdateManager.hasInstance && GorillaRopeSwingUpdateManager.instance != null && GorillaRopeSwingUpdateManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaRopeSwingUpdateManager.SetInstance(this);
		}

		// Token: 0x060051B5 RID: 20917 RVA: 0x0018CBAA File Offset: 0x0018ADAA
		public static void CreateManager()
		{
			GorillaRopeSwingUpdateManager.SetInstance(new GameObject("GorillaRopeSwingUpdateManager").AddComponent<GorillaRopeSwingUpdateManager>());
		}

		// Token: 0x060051B6 RID: 20918 RVA: 0x0018CBC0 File Offset: 0x0018ADC0
		private static void SetInstance(GorillaRopeSwingUpdateManager manager)
		{
			GorillaRopeSwingUpdateManager.instance = manager;
			GorillaRopeSwingUpdateManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060051B7 RID: 20919 RVA: 0x0018CBDB File Offset: 0x0018ADDB
		public static void RegisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (!GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Add(ropeSwing);
			}
		}

		// Token: 0x060051B8 RID: 20920 RVA: 0x0018CC01 File Offset: 0x0018AE01
		public static void UnregisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Remove(ropeSwing);
			}
		}

		// Token: 0x060051B9 RID: 20921 RVA: 0x0018CC28 File Offset: 0x0018AE28
		public void Update()
		{
			for (int i = 0; i < GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Count; i++)
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings[i].InvokeUpdate();
			}
		}

		// Token: 0x040055DA RID: 21978
		public static GorillaRopeSwingUpdateManager instance;

		// Token: 0x040055DB RID: 21979
		public static bool hasInstance = false;

		// Token: 0x040055DC RID: 21980
		public static List<GorillaRopeSwing> allGorillaRopeSwings = new List<GorillaRopeSwing>();
	}
}
