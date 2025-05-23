using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000504 RID: 1284
public class BuilderTableDataRenderData
{
	// Token: 0x040022CB RID: 8907
	public const int NUM_SPLIT_MESH_INSTANCE_GROUPS = 1;

	// Token: 0x040022CC RID: 8908
	public int texWidth;

	// Token: 0x040022CD RID: 8909
	public int texHeight;

	// Token: 0x040022CE RID: 8910
	public TextureFormat textureFormat;

	// Token: 0x040022CF RID: 8911
	public Dictionary<Material, int> materialToIndex;

	// Token: 0x040022D0 RID: 8912
	public List<Material> materials;

	// Token: 0x040022D1 RID: 8913
	public Material sharedMaterial;

	// Token: 0x040022D2 RID: 8914
	public Material sharedMaterialIndirect;

	// Token: 0x040022D3 RID: 8915
	public Dictionary<Texture2D, int> textureToIndex;

	// Token: 0x040022D4 RID: 8916
	public List<Texture2D> textures;

	// Token: 0x040022D5 RID: 8917
	public List<Material> perTextureMaterial;

	// Token: 0x040022D6 RID: 8918
	public List<MaterialPropertyBlock> perTexturePropertyBlock;

	// Token: 0x040022D7 RID: 8919
	public Texture2DArray sharedTexArray;

	// Token: 0x040022D8 RID: 8920
	public Dictionary<Mesh, int> meshToIndex;

	// Token: 0x040022D9 RID: 8921
	public List<Mesh> meshes;

	// Token: 0x040022DA RID: 8922
	public List<int> meshInstanceCount;

	// Token: 0x040022DB RID: 8923
	public NativeList<BuilderTableSubMesh> subMeshes;

	// Token: 0x040022DC RID: 8924
	public Mesh sharedMesh;

	// Token: 0x040022DD RID: 8925
	public BuilderTableDataRenderIndirectBatch dynamicBatch;

	// Token: 0x040022DE RID: 8926
	public BuilderTableDataRenderIndirectBatch staticBatch;

	// Token: 0x040022DF RID: 8927
	public JobHandle setupInstancesJobs;
}
