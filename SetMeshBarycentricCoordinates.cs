using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000371 RID: 881
[RequireComponent(typeof(MeshFilter))]
public class SetMeshBarycentricCoordinates : MonoBehaviour
{
	// Token: 0x06001468 RID: 5224 RVA: 0x000638BA File Offset: 0x00061ABA
	private void Start()
	{
		this._meshFilter = base.GetComponent<MeshFilter>();
		base.StartCoroutine(this.CheckMeshData());
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x000638D5 File Offset: 0x00061AD5
	private IEnumerator CheckMeshData()
	{
		yield return null;
		if (this._meshFilter.mesh == null || this._mesh == this._meshFilter.mesh || this._meshFilter.mesh.vertexCount == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		this.CreateBarycentricCoordinates();
		yield break;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000638E4 File Offset: 0x00061AE4
	private void CreateBarycentricCoordinates()
	{
		Mesh mesh = this._meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.GetTriangles(0);
		Color[] array = new Color[triangles.Length];
		Vector3[] array2 = new Vector3[triangles.Length];
		int[] array3 = new int[triangles.Length];
		for (int i = 0; i < triangles.Length; i++)
		{
			array[i] = new Color((i % 3 == 0) ? 1f : 0f, (i % 3 == 1) ? 1f : 0f, (i % 3 == 2) ? 1f : 0f);
			array2[i] = vertices[triangles[i]];
			array3[i] = i;
		}
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.SetVertices(array2);
		mesh.SetColors(array);
		mesh.SetIndices(array3, MeshTopology.Triangles, 0, true, 0);
		this._mesh = mesh;
		this._meshFilter.mesh = this._mesh;
		this._meshFilter.mesh.RecalculateNormals();
	}

	// Token: 0x040016C3 RID: 5827
	private MeshFilter _meshFilter;

	// Token: 0x040016C4 RID: 5828
	private Mesh _mesh;
}
