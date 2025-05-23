using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200075E RID: 1886
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct GlobalObjectRef
{
	// Token: 0x06002F07 RID: 12039 RVA: 0x000EB4D8 File Offset: 0x000E96D8
	public static GlobalObjectRef ObjectToRefSlow(Object target)
	{
		return default(GlobalObjectRef);
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x00045F91 File Offset: 0x00044191
	public static Object RefToObjectSlow(GlobalObjectRef @ref)
	{
		return null;
	}

	// Token: 0x0400358F RID: 13711
	[FieldOffset(0)]
	public ulong targetObjectId;

	// Token: 0x04003590 RID: 13712
	[FieldOffset(8)]
	public ulong targetPrefabId;

	// Token: 0x04003591 RID: 13713
	[FieldOffset(16)]
	public Guid assetGUID;

	// Token: 0x04003592 RID: 13714
	[HideInInspector]
	[FieldOffset(32)]
	public int identifierType;

	// Token: 0x04003593 RID: 13715
	[NonSerialized]
	[FieldOffset(32)]
	private GlobalObjectRefType refType;
}
