using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B28 RID: 2856
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x06004656 RID: 18006 RVA: 0x0014E6F4 File Offset: 0x0014C8F4
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x040048F0 RID: 18672
		public GTZone zone;
	}
}
