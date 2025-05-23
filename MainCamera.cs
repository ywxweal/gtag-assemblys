using System;
using UnityEngine;

// Token: 0x02000770 RID: 1904
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x06002F69 RID: 12137 RVA: 0x000EC6F1 File Offset: 0x000EA8F1
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x04003605 RID: 13829
	public Camera camera;
}
