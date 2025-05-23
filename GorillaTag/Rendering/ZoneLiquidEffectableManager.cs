using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D9F RID: 3487
	public class ZoneLiquidEffectableManager : MonoBehaviour
	{
		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x06005678 RID: 22136 RVA: 0x001A4F89 File Offset: 0x001A3189
		// (set) Token: 0x06005679 RID: 22137 RVA: 0x001A4F90 File Offset: 0x001A3190
		public static ZoneLiquidEffectableManager instance { get; private set; }

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x0600567A RID: 22138 RVA: 0x001A4F98 File Offset: 0x001A3198
		// (set) Token: 0x0600567B RID: 22139 RVA: 0x001A4F9F File Offset: 0x001A319F
		public static bool hasInstance { get; private set; }

		// Token: 0x0600567C RID: 22140 RVA: 0x001A4FA7 File Offset: 0x001A31A7
		protected void Awake()
		{
			if (ZoneLiquidEffectableManager.hasInstance && ZoneLiquidEffectableManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneLiquidEffectableManager.SetInstance(this);
		}

		// Token: 0x0600567D RID: 22141 RVA: 0x001A4FCF File Offset: 0x001A31CF
		protected void OnDestroy()
		{
			if (ZoneLiquidEffectableManager.instance == this)
			{
				ZoneLiquidEffectableManager.hasInstance = false;
				ZoneLiquidEffectableManager.instance = null;
			}
		}

		// Token: 0x0600567E RID: 22142 RVA: 0x001A4FEC File Offset: 0x001A31EC
		protected void LateUpdate()
		{
			int num = UnityLayer.Water.ToLayerMask();
			foreach (ZoneLiquidEffectable zoneLiquidEffectable in this.zoneLiquidEffectables)
			{
				Transform transform = zoneLiquidEffectable.transform;
				zoneLiquidEffectable.inLiquidVolume = Physics.CheckSphere(transform.position, zoneLiquidEffectable.radius * transform.lossyScale.x, num);
				if (zoneLiquidEffectable.inLiquidVolume != zoneLiquidEffectable.wasInLiquidVolume)
				{
					for (int i = 0; i < zoneLiquidEffectable.childRenderers.Length; i++)
					{
						if (zoneLiquidEffectable.inLiquidVolume)
						{
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
						else
						{
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
					}
				}
				zoneLiquidEffectable.wasInLiquidVolume = zoneLiquidEffectable.inLiquidVolume;
			}
		}

		// Token: 0x0600567F RID: 22143 RVA: 0x001A5110 File Offset: 0x001A3310
		private static void CreateManager()
		{
			ZoneLiquidEffectableManager.SetInstance(new GameObject("ZoneLiquidEffectableManager").AddComponent<ZoneLiquidEffectableManager>());
		}

		// Token: 0x06005680 RID: 22144 RVA: 0x001A5126 File Offset: 0x001A3326
		private static void SetInstance(ZoneLiquidEffectableManager manager)
		{
			ZoneLiquidEffectableManager.instance = manager;
			ZoneLiquidEffectableManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06005681 RID: 22145 RVA: 0x001A5144 File Offset: 0x001A3344
		public static void Register(ZoneLiquidEffectable effect)
		{
			if (!ZoneLiquidEffectableManager.hasInstance)
			{
				ZoneLiquidEffectableManager.CreateManager();
			}
			if (effect == null)
			{
				return;
			}
			if (ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Contains(effect))
			{
				return;
			}
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Add(effect);
			effect.inLiquidVolume = false;
			for (int i = 0; i < effect.childRenderers.Length; i++)
			{
				if (!(effect.childRenderers[i] == null))
				{
					Material sharedMaterial = effect.childRenderers[i].sharedMaterial;
					if (!(sharedMaterial == null) || sharedMaterial.shader.keywordSpace.FindKeyword("_WATER_EFFECT").isValid)
					{
						effect.inLiquidVolume = sharedMaterial.IsKeywordEnabled("_WATER_EFFECT") && sharedMaterial.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
						return;
					}
				}
			}
		}

		// Token: 0x06005682 RID: 22146 RVA: 0x001A520F File Offset: 0x001A340F
		public static void Unregister(ZoneLiquidEffectable effect)
		{
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Remove(effect);
		}

		// Token: 0x04005A40 RID: 23104
		private readonly List<ZoneLiquidEffectable> zoneLiquidEffectables = new List<ZoneLiquidEffectable>(32);
	}
}
