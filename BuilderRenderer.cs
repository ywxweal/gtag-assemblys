using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

// Token: 0x02000505 RID: 1285
public class BuilderRenderer : MonoBehaviour
{
	// Token: 0x06001F28 RID: 7976 RVA: 0x0009ADA7 File Offset: 0x00098FA7
	private void Awake()
	{
		this.InitIfNeeded();
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x0009ADB0 File Offset: 0x00098FB0
	public void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.renderData = new BuilderTableDataRenderData();
		this.renderData.materialToIndex = new Dictionary<Material, int>(256);
		this.renderData.materials = new List<Material>(256);
		this.renderData.meshToIndex = new Dictionary<Mesh, int>(1024);
		this.renderData.meshInstanceCount = new List<int>(1024);
		this.renderData.meshes = new List<Mesh>(4096);
		this.renderData.textureToIndex = new Dictionary<Texture2D, int>(256);
		this.renderData.textures = new List<Texture2D>(256);
		this.renderData.perTextureMaterial = new List<Material>(256);
		this.renderData.perTexturePropertyBlock = new List<MaterialPropertyBlock>(256);
		this.renderData.sharedMaterial = new Material(this.sharedMaterialBase);
		this.renderData.sharedMaterialIndirect = new Material(this.sharedMaterialIndirectBase);
		this.built = false;
		this.showing = false;
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x0009AECF File Offset: 0x000990CF
	public void Show(bool show)
	{
		this.showing = show;
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x0009AED8 File Offset: 0x000990D8
	public void BuildRenderer(List<BuilderPiece> piecePrefabs)
	{
		this.InitIfNeeded();
		for (int i = 0; i < piecePrefabs.Count; i++)
		{
			if (piecePrefabs[i] != null)
			{
				this.AddPrefab(piecePrefabs[i]);
			}
			else
			{
				Debug.LogErrorFormat("Prefab at {0} is null", new object[] { i });
			}
		}
		this.BuildSharedMaterial();
		this.BuildSharedMesh();
		this.BuildBuffer();
		this.built = true;
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x0009AF4C File Offset: 0x0009914C
	public void LogDraws()
	{
		Debug.LogFormat("Builder Renderer Counts {0} {1} {2} {3}", new object[]
		{
			this.renderData.meshes.Count,
			this.renderData.textures.Count,
			this.renderData.dynamicBatch.totalInstances,
			this.renderData.staticBatch.totalInstances
		});
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x0009AFC9 File Offset: 0x000991C9
	public void LateUpdate()
	{
		if (!this.built || !this.showing)
		{
			return;
		}
		this.RenderIndirect();
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x0009AFE4 File Offset: 0x000991E4
	public void AddPrefab(BuilderPiece prefab)
	{
		BuilderRenderer.meshRenderers.Clear();
		prefab.GetComponentsInChildren<MeshRenderer>(true, BuilderRenderer.meshRenderers);
		for (int i = 0; i < BuilderRenderer.meshRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = BuilderRenderer.meshRenderers[i];
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if (sharedMaterial == null)
			{
				if (!prefab.suppressMaterialWarnings)
				{
					Debug.LogErrorFormat("{0} {1} is missing a buidler material", new object[] { prefab.name, meshRenderer.name });
				}
			}
			else if (!this.AddMaterial(sharedMaterial, prefab.suppressMaterialWarnings))
			{
				if (!prefab.suppressMaterialWarnings)
				{
					Debug.LogWarningFormat("{0} {1} failed to add builder material", new object[] { prefab.name, meshRenderer.name });
				}
			}
			else
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component != null)
				{
					Mesh sharedMesh = component.sharedMesh;
					int num;
					if (sharedMesh != null && !this.renderData.meshToIndex.TryGetValue(sharedMesh, out num))
					{
						this.renderData.meshToIndex.Add(sharedMesh, this.renderData.meshToIndex.Count);
						this.renderData.meshInstanceCount.Add(0);
						for (int j = 0; j < 1; j++)
						{
							this.renderData.meshes.Add(sharedMesh);
						}
					}
				}
			}
		}
		if (prefab.materialOptions != null)
		{
			for (int k = 0; k < prefab.materialOptions.options.Count; k++)
			{
				Material material = prefab.materialOptions.options[k].material;
				if (!this.AddMaterial(material, prefab.suppressMaterialWarnings) && !prefab.suppressMaterialWarnings)
				{
					Debug.LogWarningFormat("builder material options {0} bad material index {1}", new object[]
					{
						prefab.materialOptions.name,
						k
					});
				}
			}
		}
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x0009B1C4 File Offset: 0x000993C4
	private bool AddMaterial(Material material, bool suppressWarnings = false)
	{
		if (material == null)
		{
			return false;
		}
		if (!material.HasTexture("_BaseMap"))
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} does not have texture property {1}", new object[] { material.name, "_BaseMap" });
			}
			return false;
		}
		Texture texture = material.GetTexture("_BaseMap");
		if (texture == null)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} null texture", new object[] { material.name });
			}
			return false;
		}
		Texture2D texture2D = texture as Texture2D;
		if (texture2D == null)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} no texture2d type is {1}", new object[]
				{
					material.name,
					texture.GetType()
				});
			}
			return false;
		}
		int num;
		if (!this.renderData.materialToIndex.TryGetValue(material, out num))
		{
			this.renderData.materialToIndex.Add(material, this.renderData.materials.Count);
			this.renderData.materials.Add(material);
		}
		int num2;
		if (!this.renderData.textureToIndex.TryGetValue(texture2D, out num2))
		{
			this.renderData.textureToIndex.Add(texture2D, this.renderData.textures.Count);
			this.renderData.textures.Add(texture2D);
			if (this.renderData.textures.Count == 1)
			{
				this.renderData.textureFormat = texture2D.format;
				this.renderData.texWidth = texture2D.width;
				this.renderData.texHeight = texture2D.height;
			}
		}
		return true;
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x0009B354 File Offset: 0x00099554
	public void BuildSharedMaterial()
	{
		TextureFormat textureFormat = TextureFormat.RGBA32;
		this.renderData.sharedTexArray = new Texture2DArray(this.renderData.texWidth, this.renderData.texHeight, this.renderData.textures.Count, textureFormat, true);
		this.renderData.sharedTexArray.filterMode = FilterMode.Point;
		for (int i = 0; i < this.renderData.textures.Count; i++)
		{
			this.renderData.sharedTexArray.SetPixels(this.renderData.textures[i].GetPixels(), i);
		}
		this.renderData.sharedTexArray.Apply(true, true);
		this.renderData.sharedMaterial.SetTexture("_BaseMapArray", this.renderData.sharedTexArray);
		this.renderData.sharedMaterialIndirect.SetTexture("_BaseMapArray", this.renderData.sharedTexArray);
		this.renderData.sharedMaterialIndirect.enableInstancing = true;
		for (int j = 0; j < this.renderData.textures.Count; j++)
		{
			Material material = new Material(this.renderData.sharedMaterial);
			material.SetInt("_BaseMapArrayIndex", j);
			this.renderData.perTextureMaterial.Add(material);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetInt("_BaseMapArrayIndex", j);
			this.renderData.perTexturePropertyBlock.Add(materialPropertyBlock);
		}
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x0009B4C0 File Offset: 0x000996C0
	public void BuildSharedMesh()
	{
		this.renderData.sharedMesh = new Mesh();
		this.renderData.sharedMesh.indexFormat = IndexFormat.UInt32;
		BuilderRenderer.verticesAll.Clear();
		BuilderRenderer.normalsAll.Clear();
		BuilderRenderer.uv1All.Clear();
		BuilderRenderer.trianglesAll.Clear();
		this.renderData.subMeshes = new NativeList<BuilderTableSubMesh>(512, Allocator.Persistent);
		for (int i = 0; i < this.renderData.meshes.Count; i++)
		{
			Mesh mesh = this.renderData.meshes[i];
			int count = BuilderRenderer.trianglesAll.Count;
			int count2 = BuilderRenderer.verticesAll.Count;
			BuilderRenderer.vertices.Clear();
			BuilderRenderer.normals.Clear();
			BuilderRenderer.uv1.Clear();
			BuilderRenderer.triangles.Clear();
			mesh.GetVertices(BuilderRenderer.vertices);
			mesh.GetNormals(BuilderRenderer.normals);
			mesh.GetUVs(0, BuilderRenderer.uv1);
			mesh.GetTriangles(BuilderRenderer.triangles, 0);
			BuilderRenderer.verticesAll.AddRange(BuilderRenderer.vertices);
			BuilderRenderer.normalsAll.AddRange(BuilderRenderer.normals);
			BuilderRenderer.uv1All.AddRange(BuilderRenderer.uv1);
			BuilderRenderer.trianglesAll.AddRange(BuilderRenderer.triangles);
			int num = BuilderRenderer.trianglesAll.Count - count;
			BuilderTableSubMesh builderTableSubMesh = new BuilderTableSubMesh
			{
				startIndex = count,
				indexCount = num,
				startVertex = count2
			};
			this.renderData.subMeshes.Add(in builderTableSubMesh);
		}
		this.renderData.sharedMesh.SetVertices(BuilderRenderer.verticesAll);
		this.renderData.sharedMesh.SetNormals(BuilderRenderer.normalsAll);
		this.renderData.sharedMesh.SetUVs(0, BuilderRenderer.uv1All);
		this.renderData.sharedMesh.SetTriangles(BuilderRenderer.trianglesAll, 0);
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x0009B6A4 File Offset: 0x000998A4
	public void BuildBuffer()
	{
		this.renderData.dynamicBatch = new BuilderTableDataRenderIndirectBatch();
		BuilderRenderer.BuildBatch(this.renderData.dynamicBatch, this.renderData.meshes.Count, 8192, this.renderData.sharedMaterialIndirect);
		this.renderData.staticBatch = new BuilderTableDataRenderIndirectBatch();
		BuilderRenderer.BuildBatch(this.renderData.staticBatch, this.renderData.meshes.Count, 8192, this.renderData.sharedMaterialIndirect);
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x0009B734 File Offset: 0x00099934
	public static void BuildBatch(BuilderTableDataRenderIndirectBatch indirectBatch, int meshCount, int maxInstances, Material sharedMaterialIndirect)
	{
		indirectBatch.totalInstances = 0;
		indirectBatch.commandCount = meshCount;
		indirectBatch.commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, indirectBatch.commandCount, 20);
		indirectBatch.commandData = new NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs>(indirectBatch.commandCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		indirectBatch.matrixBuf = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxInstances, 64);
		indirectBatch.texIndexBuf = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxInstances, 4);
		indirectBatch.tintBuf = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxInstances, 4);
		indirectBatch.instanceTransform = new TransformAccessArray(maxInstances, 3);
		for (int i = 0; i < maxInstances; i++)
		{
			indirectBatch.instanceTransform.Add(null);
		}
		indirectBatch.instanceTransformIndexToDataIndex = new NativeArray<int>(maxInstances, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		for (int j = 0; j < maxInstances; j++)
		{
			indirectBatch.instanceTransformIndexToDataIndex[j] = -1;
		}
		indirectBatch.instanceObjectToWorld = new NativeArray<Matrix4x4>(maxInstances, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		indirectBatch.instanceTexIndex = new NativeArray<int>(maxInstances, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		indirectBatch.instanceTint = new NativeArray<float>(maxInstances, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		indirectBatch.renderMeshes = new NativeList<BuilderTableMeshInstances>(512, Allocator.Persistent);
		for (int k = 0; k < meshCount; k++)
		{
			BuilderTableMeshInstances builderTableMeshInstances = new BuilderTableMeshInstances
			{
				transforms = new TransformAccessArray(maxInstances, 3),
				texIndex = new NativeList<int>(Allocator.Persistent),
				tint = new NativeList<float>(Allocator.Persistent)
			};
			indirectBatch.renderMeshes.Add(in builderTableMeshInstances);
		}
		indirectBatch.rp = new RenderParams(sharedMaterialIndirect);
		indirectBatch.rp.worldBounds = new Bounds(Vector3.zero, 10000f * Vector3.one);
		indirectBatch.rp.matProps = new MaterialPropertyBlock();
		indirectBatch.rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.identity);
		indirectBatch.matrixBuf.SetData<Matrix4x4>(indirectBatch.instanceObjectToWorld);
		indirectBatch.texIndexBuf.SetData<int>(indirectBatch.instanceTexIndex);
		indirectBatch.tintBuf.SetData<float>(indirectBatch.instanceTint);
		indirectBatch.rp.matProps.SetBuffer("_TransformMatrix", indirectBatch.matrixBuf);
		indirectBatch.rp.matProps.SetBuffer("_TexIndex", indirectBatch.texIndexBuf);
		indirectBatch.rp.matProps.SetBuffer("_Tint", indirectBatch.tintBuf);
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x0009B968 File Offset: 0x00099B68
	private void OnDestroy()
	{
		this.DestroyBuffer();
		this.renderData.subMeshes.Dispose();
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x0009B980 File Offset: 0x00099B80
	public void DestroyBuffer()
	{
		BuilderRenderer.DestroyBatch(this.renderData.staticBatch);
		BuilderRenderer.DestroyBatch(this.renderData.dynamicBatch);
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x0009B9A4 File Offset: 0x00099BA4
	public static void DestroyBatch(BuilderTableDataRenderIndirectBatch indirectBatch)
	{
		indirectBatch.commandBuf.Dispose();
		indirectBatch.commandData.Dispose();
		indirectBatch.matrixBuf.Dispose();
		indirectBatch.texIndexBuf.Dispose();
		indirectBatch.tintBuf.Dispose();
		indirectBatch.instanceTransform.Dispose();
		indirectBatch.instanceTransformIndexToDataIndex.Dispose();
		indirectBatch.instanceObjectToWorld.Dispose();
		indirectBatch.instanceTexIndex.Dispose();
		indirectBatch.instanceTint.Dispose();
		foreach (BuilderTableMeshInstances builderTableMeshInstances in indirectBatch.renderMeshes)
		{
			TransformAccessArray transforms = builderTableMeshInstances.transforms;
			transforms.Dispose();
			NativeList<int> texIndex = builderTableMeshInstances.texIndex;
			texIndex.Dispose();
			NativeList<float> tint = builderTableMeshInstances.tint;
			tint.Dispose();
		}
		indirectBatch.renderMeshes.Dispose();
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x0009BA94 File Offset: 0x00099C94
	public void PreRenderIndirect()
	{
		if (!this.built || !this.showing)
		{
			return;
		}
		this.renderData.setupInstancesJobs = default(JobHandle);
		BuilderRenderer.SetupIndirectBatchArgs(this.renderData.staticBatch, this.renderData.subMeshes);
		BuilderRenderer.SetupInstanceDataForMeshStatic setupInstanceDataForMeshStatic = new BuilderRenderer.SetupInstanceDataForMeshStatic
		{
			transformIndexToDataIndex = this.renderData.staticBatch.instanceTransformIndexToDataIndex,
			objectToWorld = this.renderData.staticBatch.instanceObjectToWorld
		};
		this.renderData.setupInstancesJobs = setupInstanceDataForMeshStatic.ScheduleReadOnly(this.renderData.staticBatch.instanceTransform, 32, default(JobHandle));
		JobHandle.ScheduleBatchedJobs();
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x0009BB47 File Offset: 0x00099D47
	public void RenderIndirect()
	{
		this.renderData.setupInstancesJobs.Complete();
		this.RenderIndirectBatch(this.renderData.staticBatch);
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x0009BB6C File Offset: 0x00099D6C
	private static void SetupIndirectBatchArgs(BuilderTableDataRenderIndirectBatch indirectBatch, NativeList<BuilderTableSubMesh> subMeshes)
	{
		uint num = 0U;
		for (int i = 0; i < indirectBatch.commandCount; i++)
		{
			BuilderTableMeshInstances builderTableMeshInstances = indirectBatch.renderMeshes[i];
			BuilderTableSubMesh builderTableSubMesh = subMeshes[i];
			GraphicsBuffer.IndirectDrawIndexedArgs indirectDrawIndexedArgs = default(GraphicsBuffer.IndirectDrawIndexedArgs);
			indirectDrawIndexedArgs.indexCountPerInstance = (uint)builderTableSubMesh.indexCount;
			indirectDrawIndexedArgs.startIndex = (uint)builderTableSubMesh.startIndex;
			indirectDrawIndexedArgs.baseVertexIndex = (uint)builderTableSubMesh.startVertex;
			indirectDrawIndexedArgs.startInstance = num;
			indirectDrawIndexedArgs.instanceCount = (uint)builderTableMeshInstances.transforms.length;
			num += indirectDrawIndexedArgs.instanceCount;
			indirectBatch.commandData[i] = indirectDrawIndexedArgs;
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x0009BC08 File Offset: 0x00099E08
	private void RenderIndirectBatch(BuilderTableDataRenderIndirectBatch indirectBatch)
	{
		indirectBatch.matrixBuf.SetData<Matrix4x4>(indirectBatch.instanceObjectToWorld);
		indirectBatch.texIndexBuf.SetData<int>(indirectBatch.instanceTexIndex);
		indirectBatch.tintBuf.SetData<float>(indirectBatch.instanceTint);
		indirectBatch.commandBuf.SetData<GraphicsBuffer.IndirectDrawIndexedArgs>(indirectBatch.commandData);
		Graphics.RenderMeshIndirect(in indirectBatch.rp, this.renderData.sharedMesh, indirectBatch.commandBuf, indirectBatch.commandCount, 0);
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x0009BC7C File Offset: 0x00099E7C
	public void AddPiece(BuilderPiece piece)
	{
		bool isStatic = piece.isStatic;
		BuilderRenderer.meshRenderers.Clear();
		piece.GetComponentsInChildren<MeshRenderer>(false, BuilderRenderer.meshRenderers);
		for (int i = 0; i < BuilderRenderer.meshRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = BuilderRenderer.meshRenderers[i];
			if (meshRenderer.enabled)
			{
				Material material = meshRenderer.material;
				if (material.HasTexture("_BaseMap"))
				{
					Texture2D texture2D = material.GetTexture("_BaseMap") as Texture2D;
					if (!(texture2D == null))
					{
						int num;
						if (!this.renderData.textureToIndex.TryGetValue(texture2D, out num))
						{
							if (!piece.suppressMaterialWarnings)
							{
								Debug.LogWarningFormat("builder piece {0} material {1} texture not found in render data", new object[] { piece.displayName, material.name });
							}
						}
						else
						{
							MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
							if (!(component == null))
							{
								Mesh sharedMesh = component.sharedMesh;
								if (!(sharedMesh == null))
								{
									int num2;
									if (!this.renderData.meshToIndex.TryGetValue(sharedMesh, out num2))
									{
										Debug.LogWarningFormat("builder piece {0} mesh {1} not found in render data", new object[] { piece.displayName, meshRenderer.name });
									}
									else
									{
										int num3 = this.renderData.meshInstanceCount[num2] % 1;
										this.renderData.meshInstanceCount[num2] = this.renderData.meshInstanceCount[num2] + 1;
										num2 += num3;
										int num4 = -1;
										if (isStatic)
										{
											NativeArray<int> instanceTransformIndexToDataIndex = this.renderData.staticBatch.instanceTransformIndexToDataIndex;
											int length = instanceTransformIndexToDataIndex.Length;
											for (int j = 0; j < length; j++)
											{
												if (instanceTransformIndexToDataIndex[j] == -1)
												{
													num4 = j;
													break;
												}
											}
											BuilderTableMeshInstances builderTableMeshInstances = this.renderData.staticBatch.renderMeshes[num2];
											int num5 = 0;
											for (int k = 0; k <= num2; k++)
											{
												num5 += this.renderData.staticBatch.renderMeshes[k].transforms.length;
											}
											for (int l = 0; l < length; l++)
											{
												if (this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] >= num5)
												{
													this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] = this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] + 1;
												}
											}
											this.renderData.staticBatch.instanceTransform[num4] = meshRenderer.transform;
											this.renderData.staticBatch.instanceTransformIndexToDataIndex[num4] = num5;
											builderTableMeshInstances.transforms.Add(meshRenderer.transform);
											builderTableMeshInstances.texIndex.Add(in num);
											builderTableMeshInstances.tint.Add(in piece.tint);
											int num6 = this.renderData.staticBatch.totalInstances - 1;
											for (int m = num6; m >= num5; m--)
											{
												this.renderData.staticBatch.instanceTexIndex[m + 1] = this.renderData.staticBatch.instanceTexIndex[m];
											}
											for (int n = num6; n >= num5; n--)
											{
												this.renderData.staticBatch.instanceObjectToWorld[n + 1] = this.renderData.staticBatch.instanceObjectToWorld[n];
											}
											for (int num7 = num6; num7 >= num5; num7--)
											{
												this.renderData.staticBatch.instanceTint[num7 + 1] = this.renderData.staticBatch.instanceTint[num7];
											}
											this.renderData.staticBatch.instanceObjectToWorld[num5] = meshRenderer.transform.localToWorldMatrix;
											this.renderData.staticBatch.instanceTexIndex[num5] = num;
											this.renderData.staticBatch.instanceTint[num5] = 1f;
											this.renderData.staticBatch.totalInstances++;
										}
										else
										{
											BuilderTableMeshInstances builderTableMeshInstances2 = this.renderData.dynamicBatch.renderMeshes[num2];
											builderTableMeshInstances2.transforms.Add(meshRenderer.transform);
											builderTableMeshInstances2.texIndex.Add(in num);
											builderTableMeshInstances2.tint.Add(in piece.tint);
											this.renderData.dynamicBatch.totalInstances++;
										}
										piece.renderingIndirect.Add(meshRenderer);
										piece.renderingIndirectTransformIndex.Add(num4);
										meshRenderer.enabled = false;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x0009C134 File Offset: 0x0009A334
	public void RemovePiece(BuilderPiece piece)
	{
		bool isStatic = piece.isStatic;
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			if (!(meshRenderer == null))
			{
				Material sharedMaterial = meshRenderer.sharedMaterial;
				if (sharedMaterial.HasTexture("_BaseMap"))
				{
					Texture2D texture2D = sharedMaterial.GetTexture("_BaseMap") as Texture2D;
					int num;
					if (!(texture2D == null) && this.renderData.textureToIndex.TryGetValue(texture2D, out num))
					{
						MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
						if (!(component == null))
						{
							Mesh sharedMesh = component.sharedMesh;
							int num2;
							if (this.renderData.meshToIndex.TryGetValue(sharedMesh, out num2))
							{
								Transform transform = meshRenderer.transform;
								bool flag = false;
								int num3 = 0;
								int num4 = -1;
								if (isStatic)
								{
									for (int j = 0; j < num2; j++)
									{
										num3 += this.renderData.staticBatch.renderMeshes[j].transforms.length;
									}
									TransformAccessArray instanceTransform = this.renderData.staticBatch.instanceTransform;
									int length = instanceTransform.length;
									int num5 = piece.renderingIndirectTransformIndex[i];
									num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num5];
									this.renderData.staticBatch.instanceTransform[num5] = null;
									this.renderData.staticBatch.instanceTransformIndexToDataIndex[num5] = -1;
									for (int k = 0; k < length; k++)
									{
										if (this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] > num4)
										{
											this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] = this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] - 1;
										}
									}
								}
								for (int l = 0; l < 1; l++)
								{
									int num6 = num2 + l;
									if (isStatic)
									{
										BuilderTableMeshInstances builderTableMeshInstances = this.renderData.staticBatch.renderMeshes[num6];
										for (int m = 0; m < builderTableMeshInstances.transforms.length; m++)
										{
											if (builderTableMeshInstances.transforms[m] == transform)
											{
												num3 += m;
												BuilderRenderer.RemoveAt(builderTableMeshInstances.transforms, m);
												builderTableMeshInstances.texIndex.RemoveAt(m);
												builderTableMeshInstances.tint.RemoveAt(m);
												flag = true;
												this.renderData.staticBatch.totalInstances--;
												break;
											}
										}
									}
									else
									{
										BuilderTableMeshInstances builderTableMeshInstances2 = this.renderData.dynamicBatch.renderMeshes[num6];
										for (int n = 0; n < builderTableMeshInstances2.transforms.length; n++)
										{
											if (builderTableMeshInstances2.transforms[n] == transform)
											{
												BuilderRenderer.RemoveAt(builderTableMeshInstances2.transforms, n);
												builderTableMeshInstances2.texIndex.RemoveAt(n);
												builderTableMeshInstances2.tint.RemoveAt(n);
												flag = true;
												this.renderData.dynamicBatch.totalInstances--;
												break;
											}
										}
									}
									if (flag)
									{
										break;
									}
								}
								if (flag && isStatic)
								{
									int num7 = this.renderData.staticBatch.totalInstances + 1;
									for (int num8 = num4; num8 < num7; num8++)
									{
										this.renderData.staticBatch.instanceTexIndex[num8] = this.renderData.staticBatch.instanceTexIndex[num8 + 1];
									}
									for (int num9 = num4; num9 < num7; num9++)
									{
										this.renderData.staticBatch.instanceObjectToWorld[num9] = this.renderData.staticBatch.instanceObjectToWorld[num9 + 1];
									}
									for (int num10 = num4; num10 < num7; num10++)
									{
										this.renderData.staticBatch.instanceTint[num10] = this.renderData.staticBatch.instanceTint[num10 + 1];
									}
								}
								meshRenderer.enabled = true;
							}
						}
					}
				}
			}
		}
		piece.renderingIndirect.Clear();
		piece.renderingIndirectTransformIndex.Clear();
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x0009C580 File Offset: 0x0009A780
	public void ChangePieceIndirectMaterial(BuilderPiece piece, List<MeshRenderer> targetRenderers, Material targetMaterial)
	{
		if (targetMaterial == null)
		{
			return;
		}
		if (!targetMaterial.HasTexture("_BaseMap"))
		{
			Debug.LogError("New Material is missing a texture");
			return;
		}
		Texture2D texture2D = targetMaterial.GetTexture("_BaseMap") as Texture2D;
		if (texture2D == null)
		{
			Debug.LogError("New Material does not have a \"_BaseMap\" property");
			return;
		}
		int num;
		if (!this.renderData.textureToIndex.TryGetValue(texture2D, out num))
		{
			Debug.LogError("New Material is not in the texture array");
			return;
		}
		bool isStatic = piece.isStatic;
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			if (!targetRenderers.Contains(meshRenderer))
			{
				Debug.Log("renderer not in target list");
			}
			else
			{
				meshRenderer.material = targetMaterial;
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (!(component == null))
				{
					Mesh sharedMesh = component.sharedMesh;
					int num2;
					if (this.renderData.meshToIndex.TryGetValue(sharedMesh, out num2))
					{
						Transform transform = meshRenderer.transform;
						bool flag = false;
						if (isStatic)
						{
							int num3 = piece.renderingIndirectTransformIndex[i];
							int num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num3];
							if (num4 >= 0)
							{
								this.renderData.staticBatch.instanceTexIndex[num4] = num;
							}
						}
						else
						{
							for (int j = 0; j < 1; j++)
							{
								int num5 = num2 + j;
								BuilderTableMeshInstances builderTableMeshInstances = this.renderData.dynamicBatch.renderMeshes[num5];
								for (int k = 0; k < builderTableMeshInstances.transforms.length; k++)
								{
									if (builderTableMeshInstances.transforms[k] == transform)
									{
										this.renderData.dynamicBatch.renderMeshes.ElementAt(num5).texIndex[k] = num;
										flag = true;
										break;
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x0009C770 File Offset: 0x0009A970
	public static void RemoveAt(TransformAccessArray a, int i)
	{
		int length = a.length;
		for (int j = i; j < length - 1; j++)
		{
			a[j] = a[j + 1];
		}
		a.RemoveAtSwapBack(length - 1);
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x0009C7B0 File Offset: 0x0009A9B0
	public void SetPieceTint(BuilderPiece piece, float tint)
	{
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if (sharedMaterial.HasTexture("_BaseMap"))
			{
				Texture2D texture2D = sharedMaterial.GetTexture("_BaseMap") as Texture2D;
				int num;
				if (!(texture2D == null) && this.renderData.textureToIndex.TryGetValue(texture2D, out num))
				{
					MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
					if (!(component == null))
					{
						Mesh sharedMesh = component.sharedMesh;
						int num2;
						if (this.renderData.meshToIndex.TryGetValue(sharedMesh, out num2))
						{
							Transform transform = meshRenderer.transform;
							if (piece.isStatic)
							{
								int num3 = piece.renderingIndirectTransformIndex[i];
								int num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num3];
								if (num4 >= 0)
								{
									this.renderData.staticBatch.instanceTint[num4] = tint;
								}
							}
							else
							{
								for (int j = 0; j < 1; j++)
								{
									int num5 = num2 + j;
									BuilderTableMeshInstances builderTableMeshInstances = this.renderData.dynamicBatch.renderMeshes[num5];
									for (int k = 0; k < builderTableMeshInstances.transforms.length; k++)
									{
										if (builderTableMeshInstances.transforms[k] == transform)
										{
											builderTableMeshInstances.tint[k] = tint;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x040022E0 RID: 8928
	public Material sharedMaterialBase;

	// Token: 0x040022E1 RID: 8929
	public Material sharedMaterialIndirectBase;

	// Token: 0x040022E2 RID: 8930
	public BuilderTableDataRenderData renderData;

	// Token: 0x040022E3 RID: 8931
	private const string texturePropName = "_BaseMap";

	// Token: 0x040022E4 RID: 8932
	private const string textureArrayPropName = "_BaseMapArray";

	// Token: 0x040022E5 RID: 8933
	private const string textureArrayIndexPropName = "_BaseMapArrayIndex";

	// Token: 0x040022E6 RID: 8934
	private const string transformMatrixPropName = "_TransformMatrix";

	// Token: 0x040022E7 RID: 8935
	private const string texIndexPropName = "_TexIndex";

	// Token: 0x040022E8 RID: 8936
	private const string tintPropName = "_Tint";

	// Token: 0x040022E9 RID: 8937
	public const int MAX_STATIC_INSTANCES = 8192;

	// Token: 0x040022EA RID: 8938
	public const int MAX_DYNAMIC_INSTANCES = 8192;

	// Token: 0x040022EB RID: 8939
	private bool initialized;

	// Token: 0x040022EC RID: 8940
	private bool built;

	// Token: 0x040022ED RID: 8941
	private bool showing;

	// Token: 0x040022EE RID: 8942
	private static List<MeshRenderer> meshRenderers = new List<MeshRenderer>(128);

	// Token: 0x040022EF RID: 8943
	private const int MAX_TOTAL_VERTS = 65536;

	// Token: 0x040022F0 RID: 8944
	private const int MAX_TOTAL_TRIS = 65536;

	// Token: 0x040022F1 RID: 8945
	private static List<Vector3> verticesAll = new List<Vector3>(65536);

	// Token: 0x040022F2 RID: 8946
	private static List<Vector3> normalsAll = new List<Vector3>(65536);

	// Token: 0x040022F3 RID: 8947
	private static List<Vector2> uv1All = new List<Vector2>(65536);

	// Token: 0x040022F4 RID: 8948
	private static List<int> trianglesAll = new List<int>(65536);

	// Token: 0x040022F5 RID: 8949
	private static List<Vector3> vertices = new List<Vector3>(65536);

	// Token: 0x040022F6 RID: 8950
	private static List<Vector3> normals = new List<Vector3>(65536);

	// Token: 0x040022F7 RID: 8951
	private static List<Vector2> uv1 = new List<Vector2>(65536);

	// Token: 0x040022F8 RID: 8952
	private static List<int> triangles = new List<int>(65536);

	// Token: 0x02000506 RID: 1286
	[BurstCompile]
	public struct SetupInstanceDataForMesh : IJobParallelForTransform
	{
		// Token: 0x06001F42 RID: 8002 RVA: 0x0009C9CC File Offset: 0x0009ABCC
		public void Execute(int index, TransformAccess transform)
		{
			int num = index + (int)this.commandData.startInstance;
			this.objectToWorld[num] = transform.localToWorldMatrix;
			this.instanceTexIndex[num] = this.texIndex[index];
			this.instanceTint[num] = this.tint[index];
		}

		// Token: 0x040022F9 RID: 8953
		[ReadOnly]
		public NativeList<int> texIndex;

		// Token: 0x040022FA RID: 8954
		[ReadOnly]
		public NativeList<float> tint;

		// Token: 0x040022FB RID: 8955
		[ReadOnly]
		public GraphicsBuffer.IndirectDrawIndexedArgs commandData;

		// Token: 0x040022FC RID: 8956
		[ReadOnly]
		public Vector3 cameraPos;

		// Token: 0x040022FD RID: 8957
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> instanceTexIndex;

		// Token: 0x040022FE RID: 8958
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Matrix4x4> objectToWorld;

		// Token: 0x040022FF RID: 8959
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<float> instanceTint;

		// Token: 0x04002300 RID: 8960
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> lodLevel;

		// Token: 0x04002301 RID: 8961
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> lodDirty;
	}

	// Token: 0x02000507 RID: 1287
	[BurstCompile]
	public struct SetupInstanceDataForMeshStatic : IJobParallelForTransform
	{
		// Token: 0x06001F43 RID: 8003 RVA: 0x0009CA2C File Offset: 0x0009AC2C
		public void Execute(int index, TransformAccess transform)
		{
			if (transform.isValid)
			{
				int num = this.transformIndexToDataIndex[index];
				this.objectToWorld[num] = transform.localToWorldMatrix;
			}
		}

		// Token: 0x04002302 RID: 8962
		[ReadOnly]
		public NativeArray<int> transformIndexToDataIndex;

		// Token: 0x04002303 RID: 8963
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Matrix4x4> objectToWorld;
	}
}
