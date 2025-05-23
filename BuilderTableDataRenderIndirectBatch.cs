using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000503 RID: 1283
public class BuilderTableDataRenderIndirectBatch
{
	// Token: 0x040022BB RID: 8891
	public int totalInstances;

	// Token: 0x040022BC RID: 8892
	public TransformAccessArray instanceTransform;

	// Token: 0x040022BD RID: 8893
	public NativeArray<int> instanceTransformIndexToDataIndex;

	// Token: 0x040022BE RID: 8894
	public NativeArray<Matrix4x4> instanceObjectToWorld;

	// Token: 0x040022BF RID: 8895
	public NativeArray<int> instanceTexIndex;

	// Token: 0x040022C0 RID: 8896
	public NativeArray<float> instanceTint;

	// Token: 0x040022C1 RID: 8897
	public NativeArray<int> instanceLodLevel;

	// Token: 0x040022C2 RID: 8898
	public NativeArray<int> instanceLodLevelDirty;

	// Token: 0x040022C3 RID: 8899
	public NativeList<BuilderTableMeshInstances> renderMeshes;

	// Token: 0x040022C4 RID: 8900
	public GraphicsBuffer commandBuf;

	// Token: 0x040022C5 RID: 8901
	public GraphicsBuffer matrixBuf;

	// Token: 0x040022C6 RID: 8902
	public GraphicsBuffer texIndexBuf;

	// Token: 0x040022C7 RID: 8903
	public GraphicsBuffer tintBuf;

	// Token: 0x040022C8 RID: 8904
	public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

	// Token: 0x040022C9 RID: 8905
	public int commandCount;

	// Token: 0x040022CA RID: 8906
	public RenderParams rp;
}
