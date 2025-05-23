using System;
using UnityEngine;

// Token: 0x02000770 RID: 1904
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x06002F6A RID: 12138 RVA: 0x000EC795 File Offset: 0x000EA995
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x04003607 RID: 13831
	public Camera camera;
}
