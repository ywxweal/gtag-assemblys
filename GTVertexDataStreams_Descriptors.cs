using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200020B RID: 523
public static class GTVertexDataStreams_Descriptors
{
	// Token: 0x06000C1C RID: 3100 RVA: 0x00040140 File Offset: 0x0003E340
	public static void DoSetVertexBufferParams(ref Mesh.MeshData writeData, int totalVertexCount)
	{
		NativeArray<VertexAttributeDescriptor> nativeArray = new NativeArray<VertexAttributeDescriptor>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
		int num = 0;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.position;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.color;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.uv1;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.lightmapUv;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.normal;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.tangent;
		writeData.SetVertexBufferParams(totalVertexCount, nativeArray);
		nativeArray.Dispose();
	}

	// Token: 0x04000EDB RID: 3803
	public static readonly VertexAttributeDescriptor position = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0);

	// Token: 0x04000EDC RID: 3804
	public static readonly VertexAttributeDescriptor color = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0);

	// Token: 0x04000EDD RID: 3805
	public static readonly VertexAttributeDescriptor uv1 = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 4, 0);

	// Token: 0x04000EDE RID: 3806
	public static readonly VertexAttributeDescriptor lightmapUv = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float16, 2, 0);

	// Token: 0x04000EDF RID: 3807
	public static readonly VertexAttributeDescriptor normal = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1);

	// Token: 0x04000EE0 RID: 3808
	public static readonly VertexAttributeDescriptor tangent = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.SNorm8, 4, 1);
}
