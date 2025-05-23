using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag.Rendering;
using MTAssets.EasyMeshCombiner;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x02000A18 RID: 2584
[RequireComponent(typeof(RuntimeMeshCombiner))]
public class UberCombiner : MonoBehaviour
{
	// Token: 0x06003DB9 RID: 15801 RVA: 0x00124B0C File Offset: 0x00122D0C
	private void CollectRenderers()
	{
		MeshRenderer[] array = UberCombiner.FilterRenderers(this.meshSources.SelectMany((GameObject g) => g.GetComponentsInChildren<MeshRenderer>(this.includeInactive)).ToArray<MeshRenderer>()).DistinctBy((MeshRenderer mr) => mr.GetInstanceID()).ToArray<MeshRenderer>();
		this.renderersToCombine = array;
		string.Format("Found {0} renderers to combine.", array.Length).Echo<string>();
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x00124B84 File Offset: 0x00122D84
	private void ValidateRenderers()
	{
		List<GameObject> list = new List<GameObject>(16);
		for (int i = 0; i < this.renderersToCombine.Length; i++)
		{
			MeshRenderer meshRenderer = this.renderersToCombine[i];
			GameObject gameObject = meshRenderer.gameObject;
			string name = gameObject.name;
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			if (meshRenderer == null || component == null)
			{
				Debug.LogError("Ojbect '" + name + "' is missing a MeshRenderer, MeshFilter, or both.", gameObject);
				list.Add(gameObject);
			}
			else
			{
				Mesh sharedMesh = component.sharedMesh;
				if (sharedMesh == null)
				{
					Debug.LogError("MeshFilter for '" + name + "' has no shared mesh.", gameObject);
					list.Add(gameObject);
				}
				else
				{
					int subMeshCount = sharedMesh.subMeshCount;
					if (subMeshCount == 0)
					{
						Debug.LogError("Shared mesh for '" + name + "' has 0 submeshes.", gameObject);
						list.Add(gameObject);
					}
					else if (sharedMesh.vertexCount < 3)
					{
						Debug.LogError("Shared mesh for '" + name + "' has less than 3 vertices.", gameObject);
						list.Add(gameObject);
					}
					else
					{
						Material[] sharedMaterials = meshRenderer.sharedMaterials;
						if (sharedMaterials.IsNullOrEmpty<Material>())
						{
							Debug.LogError("Object '" + name + "' has null or empty shared materials array.", gameObject);
							list.Add(gameObject);
						}
						else
						{
							foreach (Material material in sharedMaterials)
							{
								string name2 = material.name;
								Texture mainTexture = material.mainTexture;
								if (!(mainTexture == null) && mainTexture is RenderTexture)
								{
									Debug.LogError(string.Concat(new string[] { "Object '", name, "' has material (", name2, ") that uses a RenderTexture" }), gameObject);
									list.Add(gameObject);
									break;
								}
								if (material.HasProperty(UberCombiner._BaseMap))
								{
									Texture texture = material.GetTexture(UberCombiner._BaseMap);
									if (!(texture == null) && texture is RenderTexture)
									{
										Debug.LogError(string.Concat(new string[] { "Object '", name, "' has material (", name2, ") that uses a RenderTexture" }), gameObject);
										list.Add(gameObject);
										break;
									}
								}
								if (UberShader.IsAnimated(material))
								{
									Debug.LogError(string.Concat(new string[] { "Object '", name, "' has a material (", name2, ") that's animated" }), gameObject);
									list.Add(gameObject);
									break;
								}
							}
							if (subMeshCount != sharedMaterials.Length)
							{
								Debug.LogError("Object '" + name + "' has mismatched number of materials/submeshes" + string.Format(" Submeshes: {0} Materials: {1}", subMeshCount, sharedMaterials.Length), gameObject);
								list.Add(gameObject);
							}
						}
					}
				}
			}
		}
		this.invalidObjects = list.DistinctBy((GameObject g) => g.GetHashCode()).ToList<GameObject>();
	}

	// Token: 0x06003DBB RID: 15803 RVA: 0x00124E80 File Offset: 0x00123080
	private void SendToCombiner()
	{
		List<GameObject> list = (from r in this.renderersToCombine
			select r.gameObject into g
			where !(g == null)
			where !this.objectsToIgnore.Contains(g)
			where !this.invalidObjects.Contains(g)
			select g).DistinctBy((GameObject g) => g.GetInstanceID()).ToList<GameObject>();
		this._combiner.targetMeshes = list;
	}

	// Token: 0x06003DBC RID: 15804 RVA: 0x00124F33 File Offset: 0x00123133
	private void MergeMeshes()
	{
		this._combiner.CombineMeshes();
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x00124F41 File Offset: 0x00123141
	private void UndoMerge()
	{
		this._combiner.UndoMerge();
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x00124F4F File Offset: 0x0012314F
	private void MergeAndExtractPerMaterialMeshes()
	{
		this._combiner.onDoneMerge.AddListener(new UnityAction(this.OnPostMerge));
		this._combiner.CombineMeshes();
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x00124F79 File Offset: 0x00123179
	private void QuickMerge()
	{
		this.CollectRenderers();
		this.ValidateRenderers();
		this.SendToCombiner();
		this.MergeAndExtractPerMaterialMeshes();
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x00124F94 File Offset: 0x00123194
	private void OnPostMerge()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		Mesh sharedMesh = component.sharedMesh;
		int subMeshCount = sharedMesh.subMeshCount;
		string name = component2.name;
		Material[] sharedMaterials = component2.sharedMaterials;
		GameObject gameObject = new GameObject(name + "_PerMaterialMeshes");
		UberCombinerPerMaterialMeshes uberCombinerPerMaterialMeshes;
		this.GetOrAddComponent(out uberCombinerPerMaterialMeshes);
		uberCombinerPerMaterialMeshes.rootObject = gameObject;
		uberCombinerPerMaterialMeshes.objects = new GameObject[subMeshCount];
		uberCombinerPerMaterialMeshes.filters = new MeshFilter[subMeshCount];
		uberCombinerPerMaterialMeshes.renderers = new MeshRenderer[subMeshCount];
		uberCombinerPerMaterialMeshes.materials = new Material[subMeshCount];
		GTMeshData gtmeshData = GTMeshData.Parse(sharedMesh);
		for (int i = 0; i < subMeshCount; i++)
		{
			GameObject gameObject2 = new GameObject(string.Format("{0}_{1}", i, sharedMaterials[i].name));
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.isStatic = true;
			MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			Mesh mesh = gtmeshData.ExtractSubmesh(i, false);
			meshFilter.sharedMesh = mesh;
			meshRenderer.sharedMaterial = sharedMaterials[i];
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			uberCombinerPerMaterialMeshes.objects[i] = gameObject2;
			uberCombinerPerMaterialMeshes.filters[i] = meshFilter;
			uberCombinerPerMaterialMeshes.renderers[i] = meshRenderer;
			uberCombinerPerMaterialMeshes.materials[i] = sharedMaterials[i];
		}
	}

	// Token: 0x06003DC1 RID: 15809 RVA: 0x001250EC File Offset: 0x001232EC
	private void OnValidate()
	{
		if (!base.transform.position.Approx0(1E-05f))
		{
			base.transform.position = Vector3.zero;
		}
		if (this._combiner == null)
		{
			this._combiner = base.GetComponent<RuntimeMeshCombiner>();
			this._combiner.recalculateNormals = false;
			this._combiner.recalculateTangents = false;
			this._combiner.combineInactives = false;
			this._combiner.garbageCollectorAfterUndo = true;
			this._combiner.afterMerge = RuntimeMeshCombiner.AfterMerge.DoNothing;
		}
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x00125176 File Offset: 0x00123376
	private static IEnumerable<MeshRenderer> FilterRenderers(IList<MeshRenderer> renderers)
	{
		Shader uberShader = UberShader.ReferenceShader;
		Shader uberShaderNonSRP = UberShader.ReferenceShaderNonSRP;
		RenderQueueRange transQueue = RenderQueueRange.transparent;
		int num;
		for (int i = 0; i < renderers.Count; i = num)
		{
			MeshRenderer mr = renderers[i];
			if (!(mr == null) && mr.enabled && mr.gameObject.isStatic && !mr.GetComponent<EdDoNotMeshCombine>())
			{
				MeshFilter component = mr.GetComponent<MeshFilter>();
				if (!(component == null))
				{
					Mesh sharedMesh = component.sharedMesh;
					if (!(sharedMesh == null) && sharedMesh.vertexCount >= 3)
					{
						Material[] sharedMats = mr.sharedMaterials;
						if (!sharedMats.IsNullOrEmpty<Material>())
						{
							for (int j = 0; j < sharedMats.Length; j = num)
							{
								Material material = sharedMats[j];
								if (!(material == null))
								{
									int renderQueue = material.renderQueue;
									if ((renderQueue < transQueue.lowerBound || renderQueue > transQueue.upperBound) && (renderQueue < 2450 || renderQueue > 2500))
									{
										Shader shader = material.shader;
										if (shader == uberShader)
										{
											yield return mr;
										}
										else if (shader == uberShaderNonSRP)
										{
											yield return mr;
										}
									}
								}
								num = j + 1;
							}
							mr = null;
							sharedMats = null;
						}
					}
				}
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x04004193 RID: 16787
	[SerializeField]
	private RuntimeMeshCombiner _combiner;

	// Token: 0x04004194 RID: 16788
	[Space]
	public GameObject[] meshSources = new GameObject[0];

	// Token: 0x04004195 RID: 16789
	[Space]
	public GameObject[] objectsToIgnore = new GameObject[0];

	// Token: 0x04004196 RID: 16790
	[Space]
	[NonSerialized]
	private MeshRenderer[] renderersToCombine = new MeshRenderer[0];

	// Token: 0x04004197 RID: 16791
	[Space]
	[NonSerialized]
	private List<GameObject> invalidObjects = new List<GameObject>();

	// Token: 0x04004198 RID: 16792
	public bool includeInactive;

	// Token: 0x04004199 RID: 16793
	private static ShaderHashId _BaseMap = "_BaseMap";
}
