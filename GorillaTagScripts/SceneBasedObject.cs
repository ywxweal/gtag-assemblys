using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B28 RID: 2856
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x06004655 RID: 18005 RVA: 0x0014E61C File Offset: 0x0014C81C
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x040048EF RID: 18671
		public GTZone zone;
	}
}
