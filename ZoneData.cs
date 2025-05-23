using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
[Serializable]
public class ZoneData
{
	// Token: 0x040010AA RID: 4266
	public GTZone zone;

	// Token: 0x040010AB RID: 4267
	public string sceneName;

	// Token: 0x040010AC RID: 4268
	public float CameraFarClipPlane = 500f;

	// Token: 0x040010AD RID: 4269
	public GameObject[] rootGameObjects;

	// Token: 0x040010AE RID: 4270
	[NonSerialized]
	public bool active;
}
