using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x0003A36B File Offset: 0x0003856B
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindObjectOfType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x04000D2D RID: 3373
	private static DevInspectorManager _instance;
}
