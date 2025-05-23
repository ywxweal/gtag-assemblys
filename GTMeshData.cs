using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000A17 RID: 2583
public class GTMeshData
{
	// Token: 0x06003DB5 RID: 15797 RVA: 0x00124778 File Offset: 0x00122978
	public GTMeshData(Mesh m)
	{
		this.mesh = m;
		this.subMeshCount = m.subMeshCount;
		this.vertices = m.vertices;
		this.triangles = m.triangles;
		this.normals = m.normals;
		this.tangents = m.tangents;
		this.colors32 = m.colors32;
		this.boneWeights = m.boneWeights;
		this.uv = m.uv;
		this.uv2 = m.uv2;
		this.uv3 = m.uv3;
		this.uv4 = m.uv4;
		this.uv5 = m.uv5;
		this.uv6 = m.uv6;
		this.uv7 = m.uv7;
		this.uv8 = m.uv8;
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x00124848 File Offset: 0x00122A48
	public Mesh ExtractSubmesh(int subMeshIndex, bool optimize = false)
	{
		if (subMeshIndex < 0 || subMeshIndex >= this.subMeshCount)
		{
			throw new IndexOutOfRangeException("subMeshIndex");
		}
		SubMeshDescriptor subMesh = this.mesh.GetSubMesh(subMeshIndex);
		int firstVertex = subMesh.firstVertex;
		int vertexCount = subMesh.vertexCount;
		MeshTopology topology = subMesh.topology;
		int[] indices = this.mesh.GetIndices(subMeshIndex, false);
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] -= firstVertex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = ((vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
		mesh.SetVertices(this.vertices, firstVertex, vertexCount);
		mesh.SetIndices(indices, topology, 0);
		mesh.SetNormals(this.normals, firstVertex, vertexCount);
		mesh.SetTangents(this.tangents, firstVertex, vertexCount);
		if (!this.uv.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(0, this.uv, firstVertex, vertexCount);
		}
		if (!this.uv2.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(1, this.uv2, firstVertex, vertexCount);
		}
		if (!this.uv3.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(2, this.uv3, firstVertex, vertexCount);
		}
		if (!this.uv4.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(3, this.uv4, firstVertex, vertexCount);
		}
		if (!this.uv5.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(4, this.uv5, firstVertex, vertexCount);
		}
		if (!this.uv6.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(5, this.uv6, firstVertex, vertexCount);
		}
		if (!this.uv7.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(6, this.uv7, firstVertex, vertexCount);
		}
		if (!this.uv8.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(7, this.uv8, firstVertex, vertexCount);
		}
		if (optimize)
		{
			mesh.Optimize();
			mesh.OptimizeIndexBuffers();
		}
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x00124A16 File Offset: 0x00122C16
	public static GTMeshData Parse(Mesh mesh)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		return new GTMeshData(mesh);
	}

	// Token: 0x04004182 RID: 16770
	public Mesh mesh;

	// Token: 0x04004183 RID: 16771
	public Vector3[] vertices;

	// Token: 0x04004184 RID: 16772
	public Vector3[] normals;

	// Token: 0x04004185 RID: 16773
	public Vector4[] tangents;

	// Token: 0x04004186 RID: 16774
	public Color32[] colors32;

	// Token: 0x04004187 RID: 16775
	public int[] triangles;

	// Token: 0x04004188 RID: 16776
	public BoneWeight[] boneWeights;

	// Token: 0x04004189 RID: 16777
	public Vector2[] uv;

	// Token: 0x0400418A RID: 16778
	public Vector2[] uv2;

	// Token: 0x0400418B RID: 16779
	public Vector2[] uv3;

	// Token: 0x0400418C RID: 16780
	public Vector2[] uv4;

	// Token: 0x0400418D RID: 16781
	public Vector2[] uv5;

	// Token: 0x0400418E RID: 16782
	public Vector2[] uv6;

	// Token: 0x0400418F RID: 16783
	public Vector2[] uv7;

	// Token: 0x04004190 RID: 16784
	public Vector2[] uv8;

	// Token: 0x04004191 RID: 16785
	public int subMeshCount;
}
