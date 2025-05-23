using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D21 RID: 3361
	[ExecuteAlways]
	public class TextureTransitionerManager : MonoBehaviour
	{
		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06005406 RID: 21510 RVA: 0x001973E6 File Offset: 0x001955E6
		// (set) Token: 0x06005407 RID: 21511 RVA: 0x001973ED File Offset: 0x001955ED
		public static TextureTransitionerManager instance { get; private set; }

		// Token: 0x06005408 RID: 21512 RVA: 0x001973F5 File Offset: 0x001955F5
		protected void Awake()
		{
			if (TextureTransitionerManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TextureTransitionerManager.instance = this;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			this.matPropBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x00197434 File Offset: 0x00195634
		protected void LateUpdate()
		{
			foreach (TextureTransitioner textureTransitioner in TextureTransitionerManager.components)
			{
				int num = textureTransitioner.textures.Length;
				float num2 = Mathf.Clamp01(textureTransitioner.remapInfo.Remap(textureTransitioner.iDynamicFloat.floatValue));
				TextureTransitioner.DirectionRetentionMode directionRetentionMode = textureTransitioner.directionRetentionMode;
				if (directionRetentionMode != TextureTransitioner.DirectionRetentionMode.IncreaseOnly)
				{
					if (directionRetentionMode == TextureTransitioner.DirectionRetentionMode.DecreaseOnly)
					{
						num2 = Mathf.Min(num2, textureTransitioner.normalizedValue);
					}
				}
				else
				{
					num2 = Mathf.Max(num2, textureTransitioner.normalizedValue);
				}
				float num3 = num2 * (float)(num - 1);
				float num4 = num3 % 1f;
				int num5 = (int)(num4 * 1000f);
				int num6 = (int)num3;
				int num7 = Mathf.Min(num - 1, num6 + 1);
				if (num5 != textureTransitioner.transitionPercent || num6 != textureTransitioner.tex1Index || num7 != textureTransitioner.tex2Index)
				{
					this.matPropBlock.SetFloat(textureTransitioner.texTransitionShaderParam, num4);
					this.matPropBlock.SetTexture(textureTransitioner.tex1ShaderParam, textureTransitioner.textures[num6]);
					this.matPropBlock.SetTexture(textureTransitioner.tex2ShaderParam, textureTransitioner.textures[num7]);
					Renderer[] renderers = textureTransitioner.renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].SetPropertyBlock(this.matPropBlock);
					}
					textureTransitioner.normalizedValue = num2;
					textureTransitioner.transitionPercent = num5;
					textureTransitioner.tex1Index = num6;
					textureTransitioner.tex2Index = num7;
				}
			}
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x001975C4 File Offset: 0x001957C4
		public static void EnsureInstanceIsAvailable()
		{
			if (TextureTransitionerManager.instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject();
			TextureTransitionerManager.instance = gameObject.AddComponent<TextureTransitionerManager>();
			gameObject.name = "TextureTransitionerManager (Singleton)";
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x001975EE File Offset: 0x001957EE
		public static void Register(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Add(component);
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x001975FB File Offset: 0x001957FB
		public static void Unregister(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Remove(component);
		}

		// Token: 0x0400570B RID: 22283
		public static readonly List<TextureTransitioner> components = new List<TextureTransitioner>(256);

		// Token: 0x0400570C RID: 22284
		private MaterialPropertyBlock matPropBlock;
	}
}
