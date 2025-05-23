using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000153 RID: 339
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x060008CC RID: 2252 RVA: 0x0002FB6D File Offset: 0x0002DD6D
	private void Start()
	{
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002FB80 File Offset: 0x0002DD80
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0002FBC2 File Offset: 0x0002DDC2
	public void SetColor(string propertyName, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyName);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0002FBD8 File Offset: 0x0002DDD8
	public void SetFloat(string propertyName, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyName);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002FBF0 File Offset: 0x0002DDF0
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			if (customMaterialData.name == propertyName)
			{
				return customMaterialData;
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData2 = new ApplyMaterialProperty.CustomMaterialData
		{
			name = propertyName
		};
		this.customData.Add(customMaterialData2);
		return customMaterialData2;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0002FC4C File Offset: 0x0002DE4C
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = (this.targetMaterial = this._instance.Material);
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			switch (customMaterialData.dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(customMaterialData.name, customMaterialData.color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(customMaterialData.name, customMaterialData.@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(customMaterialData.name, customMaterialData.vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(customMaterialData.name, customMaterialData.vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(customMaterialData.name, customMaterialData.vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(customMaterialData.name, customMaterialData.texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0002FD8C File Offset: 0x0002DF8C
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			switch (customMaterialData.dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(customMaterialData.name, customMaterialData.color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(customMaterialData.name, customMaterialData.@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(customMaterialData.name, customMaterialData.texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x04000A50 RID: 2640
	public ApplyMaterialProperty.ApplyMode mode;

	// Token: 0x04000A51 RID: 2641
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x04000A52 RID: 2642
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x04000A53 RID: 2643
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04000A54 RID: 2644
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x04000A55 RID: 2645
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x04000A56 RID: 2646
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x02000154 RID: 340
	public enum ApplyMode
	{
		// Token: 0x04000A58 RID: 2648
		MaterialInstance,
		// Token: 0x04000A59 RID: 2649
		MaterialPropertyBlock
	}

	// Token: 0x02000155 RID: 341
	public enum SuportedTypes
	{
		// Token: 0x04000A5B RID: 2651
		Color,
		// Token: 0x04000A5C RID: 2652
		Float,
		// Token: 0x04000A5D RID: 2653
		Vector2,
		// Token: 0x04000A5E RID: 2654
		Vector3,
		// Token: 0x04000A5F RID: 2655
		Vector4,
		// Token: 0x04000A60 RID: 2656
		Texture2D
	}

	// Token: 0x02000156 RID: 342
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x060008D4 RID: 2260 RVA: 0x0002FEC4 File Offset: 0x0002E0C4
		public override int GetHashCode()
		{
			return new ValueTuple<string, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.name, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x04000A61 RID: 2657
		public string name;

		// Token: 0x04000A62 RID: 2658
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x04000A63 RID: 2659
		public Color color;

		// Token: 0x04000A64 RID: 2660
		public float @float;

		// Token: 0x04000A65 RID: 2661
		public Vector2 vector2;

		// Token: 0x04000A66 RID: 2662
		public Vector3 vector3;

		// Token: 0x04000A67 RID: 2663
		public Vector4 vector4;

		// Token: 0x04000A68 RID: 2664
		public Texture2D texture2D;
	}
}
