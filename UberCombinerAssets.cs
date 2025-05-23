using System;
using UnityEngine;

// Token: 0x02000A1B RID: 2587
public class UberCombinerAssets : ScriptableObject
{
	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x06003DD6 RID: 15830 RVA: 0x00125403 File Offset: 0x00123603
	public static UberCombinerAssets Instance
	{
		get
		{
			UberCombinerAssets.gInstance == null;
			return UberCombinerAssets.gInstance;
		}
	}

	// Token: 0x06003DD7 RID: 15831 RVA: 0x00125416 File Offset: 0x00123616
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06003DD8 RID: 15832 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Setup()
	{
	}

	// Token: 0x06003DD9 RID: 15833 RVA: 0x000023F4 File Offset: 0x000005F4
	public void ClearMaterialAssets()
	{
	}

	// Token: 0x06003DDA RID: 15834 RVA: 0x000023F4 File Offset: 0x000005F4
	public void ClearPrefabAssets()
	{
	}

	// Token: 0x040041AB RID: 16811
	[SerializeField]
	private Object _rootFolder;

	// Token: 0x040041AC RID: 16812
	[SerializeField]
	private Object _resourcesFolder;

	// Token: 0x040041AD RID: 16813
	[SerializeField]
	private Object _materialsFolder;

	// Token: 0x040041AE RID: 16814
	[SerializeField]
	private Object _prefabsFolder;

	// Token: 0x040041AF RID: 16815
	[Space]
	public Object MeshBakerDefaultCustomizer;

	// Token: 0x040041B0 RID: 16816
	public Material ReferenceUberMaterial;

	// Token: 0x040041B1 RID: 16817
	public Shader TextureArrayCapableShader;

	// Token: 0x040041B2 RID: 16818
	[Space]
	public string RootFolderPath;

	// Token: 0x040041B3 RID: 16819
	public string ResourcesFolderPath;

	// Token: 0x040041B4 RID: 16820
	public string MaterialsFolderPath;

	// Token: 0x040041B5 RID: 16821
	public string PrefabsFolderPath;

	// Token: 0x040041B6 RID: 16822
	private static UberCombinerAssets gInstance;
}
