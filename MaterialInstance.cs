using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000771 RID: 1905
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x06002F6C RID: 12140 RVA: 0x000EC7A5 File Offset: 0x000EA9A5
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x06002F6D RID: 12141 RVA: 0x000EC7E3 File Offset: 0x000EA9E3
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x000EC816 File Offset: 0x000EAA16
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x06002F6F RID: 12143 RVA: 0x000EC84E File Offset: 0x000EAA4E
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x06002F70 RID: 12144 RVA: 0x000EC858 File Offset: 0x000EAA58
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x06002F71 RID: 12145 RVA: 0x000EC862 File Offset: 0x000EAA62
	// (set) Token: 0x06002F72 RID: 12146 RVA: 0x000EC86A File Offset: 0x000EAA6A
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x06002F73 RID: 12147 RVA: 0x000EC899 File Offset: 0x000EAA99
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x06002F74 RID: 12148 RVA: 0x000EC8D4 File Offset: 0x000EAAD4
	// (set) Token: 0x06002F75 RID: 12149 RVA: 0x000EC909 File Offset: 0x000EAB09
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x000EC926 File Offset: 0x000EAB26
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x000EC92E File Offset: 0x000EAB2E
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x000EC936 File Offset: 0x000EAB36
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x000EC96C File Offset: 0x000EAB6C
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x000EC9C5 File Offset: 0x000EABC5
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x000EC9F0 File Offset: 0x000EABF0
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x000ECA48 File Offset: 0x000EAC48
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = ((a != null) ? new int?(a.Length) : null);
		int? num2 = ((b != null) ? new int?(b.Length) : null);
		if (!((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null))))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : null);
			if (!((num4 < num2.GetValueOrDefault()) & (num2 != null)))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x000ECAEC File Offset: 0x000EACEC
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x000ECB6C File Offset: 0x000EAD6C
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x000ECB92 File Offset: 0x000EAD92
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x000ECBB0 File Offset: 0x000EADB0
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x000ECBDE File Offset: 0x000EADDE
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04003608 RID: 13832
	private Renderer cachedRenderer;

	// Token: 0x04003609 RID: 13833
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x0400360A RID: 13834
	private Material[] instanceMaterials;

	// Token: 0x0400360B RID: 13835
	private Material[] cachedSharedMaterials;

	// Token: 0x0400360C RID: 13836
	private bool initialized;

	// Token: 0x0400360D RID: 13837
	private bool materialsInstanced;

	// Token: 0x0400360E RID: 13838
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x0400360F RID: 13839
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x04003610 RID: 13840
	private const string instancePostfix = " (Instance)";
}
