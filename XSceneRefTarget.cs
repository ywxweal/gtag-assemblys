using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class XSceneRefTarget : MonoBehaviour
{
	// Token: 0x06000D3B RID: 3387 RVA: 0x000457A7 File Offset: 0x000439A7
	private void Awake()
	{
		this.Register(false);
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x000457B0 File Offset: 0x000439B0
	private void Reset()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x000457BD File Offset: 0x000439BD
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Register(false);
		}
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x000457D0 File Offset: 0x000439D0
	public void Register(bool force = false)
	{
		if (this.UniqueID == this.lastRegisteredID && !force)
		{
			return;
		}
		if (this.lastRegisteredID != -1)
		{
			XSceneRefGlobalHub.Unregister(this.lastRegisteredID, this);
		}
		XSceneRefGlobalHub.Register(this.UniqueID, this);
		this.lastRegisteredID = this.UniqueID;
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0004581C File Offset: 0x00043A1C
	private void OnDestroy()
	{
		XSceneRefGlobalHub.Unregister(this.UniqueID, this);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0004582A File Offset: 0x00043A2A
	private void AssignNewID()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
		this.Register(false);
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00045840 File Offset: 0x00043A40
	public static int CreateNewID()
	{
		int num = (int)((DateTime.Now - XSceneRefTarget.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
		if (num <= XSceneRefTarget.lastAssignedID)
		{
			XSceneRefTarget.lastAssignedID++;
			return XSceneRefTarget.lastAssignedID;
		}
		XSceneRefTarget.lastAssignedID = num;
		return num;
	}

	// Token: 0x040010CB RID: 4299
	public int UniqueID;

	// Token: 0x040010CC RID: 4300
	[NonSerialized]
	private int lastRegisteredID = -1;

	// Token: 0x040010CD RID: 4301
	private static DateTime epoch = new DateTime(2024, 1, 1);

	// Token: 0x040010CE RID: 4302
	private static int lastAssignedID;
}
