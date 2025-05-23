using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public static class GlobalDeactivatedSpawnRoot
{
	// Token: 0x06000A7A RID: 2682 RVA: 0x00039C38 File Offset: 0x00037E38
	public static Transform GetOrCreate()
	{
		if (!GlobalDeactivatedSpawnRoot._xform)
		{
			GlobalDeactivatedSpawnRoot._xform = new GameObject("GlobalDeactivatedSpawnRoot").transform;
			GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
			Object.DontDestroyOnLoad(GlobalDeactivatedSpawnRoot._xform.gameObject);
		}
		GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
		return GlobalDeactivatedSpawnRoot._xform;
	}

	// Token: 0x04000CB9 RID: 3257
	private static Transform _xform;
}
