using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D1F RID: 3359
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x06005401 RID: 21505 RVA: 0x00197313 File Offset: 0x00195513
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x06005402 RID: 21506 RVA: 0x00197348 File Offset: 0x00195548
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x06005403 RID: 21507 RVA: 0x00197436 File Offset: 0x00195636
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x06005404 RID: 21508 RVA: 0x0019743E File Offset: 0x0019563E
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x06005405 RID: 21509 RVA: 0x00197473 File Offset: 0x00195673
		public void ResetToDefaultState()
		{
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x040056F6 RID: 22262
		public bool editorPreview;

		// Token: 0x040056F7 RID: 22263
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x040056F8 RID: 22264
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x040056F9 RID: 22265
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x040056FA RID: 22266
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x040056FB RID: 22267
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x040056FC RID: 22268
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x040056FD RID: 22269
		public Texture[] textures;

		// Token: 0x040056FE RID: 22270
		public Renderer[] renderers;

		// Token: 0x040056FF RID: 22271
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x04005700 RID: 22272
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x04005701 RID: 22273
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x04005702 RID: 22274
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x04005703 RID: 22275
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x04005704 RID: 22276
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x04005705 RID: 22277
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x04005706 RID: 22278
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02000D20 RID: 3360
		public enum DirectionRetentionMode
		{
			// Token: 0x04005708 RID: 22280
			None,
			// Token: 0x04005709 RID: 22281
			IncreaseOnly,
			// Token: 0x0400570A RID: 22282
			DecreaseOnly
		}
	}
}
