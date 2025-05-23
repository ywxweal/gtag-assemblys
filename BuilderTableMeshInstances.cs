using System;
using Unity.Collections;
using UnityEngine.Jobs;

// Token: 0x02000501 RID: 1281
public struct BuilderTableMeshInstances
{
	// Token: 0x040022B5 RID: 8885
	public TransformAccessArray transforms;

	// Token: 0x040022B6 RID: 8886
	public NativeList<int> texIndex;

	// Token: 0x040022B7 RID: 8887
	public NativeList<float> tint;
}
