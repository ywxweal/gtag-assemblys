using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000DA0 RID: 3488
	public class ZoneShaderSettings : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06005685 RID: 22149 RVA: 0x001A530F File Offset: 0x001A350F
		// (set) Token: 0x06005686 RID: 22150 RVA: 0x001A5316 File Offset: 0x001A3516
		[DebugReadout]
		public static ZoneShaderSettings defaultsInstance { get; private set; }

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x06005687 RID: 22151 RVA: 0x001A531E File Offset: 0x001A351E
		// (set) Token: 0x06005688 RID: 22152 RVA: 0x001A5325 File Offset: 0x001A3525
		public static bool hasDefaultsInstance { get; private set; }

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x06005689 RID: 22153 RVA: 0x001A532D File Offset: 0x001A352D
		// (set) Token: 0x0600568A RID: 22154 RVA: 0x001A5334 File Offset: 0x001A3534
		[DebugReadout]
		public static ZoneShaderSettings activeInstance { get; private set; }

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x0600568B RID: 22155 RVA: 0x001A533C File Offset: 0x001A353C
		// (set) Token: 0x0600568C RID: 22156 RVA: 0x001A5343 File Offset: 0x001A3543
		public static bool hasActiveInstance { get; private set; }

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x0600568D RID: 22157 RVA: 0x001A534B File Offset: 0x001A354B
		public bool isActiveInstance
		{
			get
			{
				return ZoneShaderSettings.activeInstance == this;
			}
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x0600568E RID: 22158 RVA: 0x001A5358 File Offset: 0x001A3558
		[DebugReadout]
		private float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogDepthFadeSize * this._groundFogDepthFadeSize);
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x0600568F RID: 22159 RVA: 0x001A5377 File Offset: 0x001A3577
		[DebugReadout]
		private float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogHeightFadeSize);
			}
		}

		// Token: 0x06005690 RID: 22160 RVA: 0x001A5390 File Offset: 0x001A3590
		public void SetZoneLiquidTypeKeywordEnum(ZoneShaderSettings.EZoneLiquidType liquidType)
		{
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.None)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Water)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Lava)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
				return;
			}
			Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		}

		// Token: 0x06005691 RID: 22161 RVA: 0x001A53E9 File Offset: 0x001A35E9
		public void SetZoneLiquidShapeKeywordEnum(ZoneShaderSettings.ELiquidShape shape)
		{
			if (shape == ZoneShaderSettings.ELiquidShape.Plane)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			else
			{
				Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			if (shape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
				return;
			}
			Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06005692 RID: 22162 RVA: 0x001A541D File Offset: 0x001A361D
		// (set) Token: 0x06005693 RID: 22163 RVA: 0x001A5424 File Offset: 0x001A3624
		public static int shaderParam_ZoneLiquidPosRadiusSq { get; private set; } = Shader.PropertyToID("_ZoneLiquidPosRadiusSq");

		// Token: 0x06005694 RID: 22164 RVA: 0x001A542C File Offset: 0x001A362C
		public static float GetWaterY()
		{
			return ZoneShaderSettings.activeInstance.mainWaterSurfacePlane.position.y;
		}

		// Token: 0x06005695 RID: 22165 RVA: 0x001A5444 File Offset: 0x001A3644
		protected void Awake()
		{
			this.hasMainWaterSurfacePlane = this.mainWaterSurfacePlane != null && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
			this.hasDynamicWaterSurfacePlane = this.hasMainWaterSurfacePlane && !this.mainWaterSurfacePlane.gameObject.isStatic;
			this.hasLiquidBottomTransform = this.liquidBottomTransform != null && (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
			this.CheckDefaultsInstance();
			if (this._activateOnAwake)
			{
				this.BecomeActiveInstance(false);
			}
		}

		// Token: 0x06005696 RID: 22166 RVA: 0x001A54DC File Offset: 0x001A36DC
		protected void OnEnable()
		{
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
		}

		// Token: 0x06005697 RID: 22167 RVA: 0x000D1D87 File Offset: 0x000CFF87
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005698 RID: 22168 RVA: 0x001A54EC File Offset: 0x001A36EC
		protected void OnDestroy()
		{
			if (ZoneShaderSettings.defaultsInstance == this)
			{
				ZoneShaderSettings.hasDefaultsInstance = false;
			}
			if (ZoneShaderSettings.activeInstance == this)
			{
				ZoneShaderSettings.hasActiveInstance = false;
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06005699 RID: 22169 RVA: 0x001A551A File Offset: 0x001A371A
		// (set) Token: 0x0600569A RID: 22170 RVA: 0x001A5522 File Offset: 0x001A3722
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x0600569B RID: 22171 RVA: 0x001A552B File Offset: 0x001A372B
		void ITickSystemPost.PostTick()
		{
			if (ZoneShaderSettings.activeInstance == this && Application.isPlaying && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateMainPlaneShaderProperty();
			}
		}

		// Token: 0x0600569C RID: 22172 RVA: 0x001A5550 File Offset: 0x001A3750
		private void UpdateMainPlaneShaderProperty()
		{
			Transform transform = null;
			bool flag = false;
			if (this.hasMainWaterSurfacePlane && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues))
			{
				flag = true;
				transform = this.mainWaterSurfacePlane;
			}
			else if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasMainWaterSurfacePlane)
			{
				flag = true;
				transform = ZoneShaderSettings.defaultsInstance.mainWaterSurfacePlane;
			}
			if (!flag)
			{
				return;
			}
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			float num = -Vector3.Dot(up, position);
			Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, num));
			ZoneShaderSettings.ELiquidShape eliquidShape;
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				eliquidShape = this.liquidShape;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				eliquidShape = ZoneShaderSettings.defaultsInstance.liquidShape;
			}
			else
			{
				eliquidShape = ZoneShaderSettings.liquidShape_previousValue;
			}
			ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
			float num2;
			if ((this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues) && this.hasLiquidBottomTransform)
			{
				num2 = this.liquidBottomTransform.position.y;
			}
			else if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasLiquidBottomTransform)
			{
				num2 = ZoneShaderSettings.defaultsInstance.liquidBottomTransform.position.y;
			}
			else
			{
				num2 = this.liquidBottomPosY_previousValue;
			}
			float num3;
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				num3 = this.liquidShapeRadius;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				num3 = ZoneShaderSettings.defaultsInstance.liquidShapeRadius;
			}
			else
			{
				num3 = ZoneShaderSettings.liquidShapeRadius_previousValue;
			}
			if (eliquidShape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_ZoneLiquidPosRadiusSq, new Vector4(position.x, num2, position.z, num3 * num3));
				ZoneShaderSettings.liquidShapeRadius_previousValue = num3;
			}
		}

		// Token: 0x0600569D RID: 22173 RVA: 0x001A570C File Offset: 0x001A390C
		private void CheckDefaultsInstance()
		{
			if (!this.isDefaultValues)
			{
				return;
			}
			if (ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance != null && ZoneShaderSettings.defaultsInstance != this)
			{
				string path = ZoneShaderSettings.defaultsInstance.transform.GetPath();
				Debug.LogError(string.Concat(new string[]
				{
					"ZoneShaderSettings: Destroying conflicting defaults instance.\n- keeping: \"",
					path,
					"\"\n- destroying (this): \"",
					base.transform.GetPath(),
					"\""
				}), this);
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneShaderSettings.defaultsInstance = this;
			ZoneShaderSettings.hasDefaultsInstance = true;
			this.BecomeActiveInstance(false);
		}

		// Token: 0x0600569E RID: 22174 RVA: 0x001A57B0 File Offset: 0x001A39B0
		public void BecomeActiveInstance(bool force = false)
		{
			if (ZoneShaderSettings.activeInstance == this && !force)
			{
				return;
			}
			if (ZoneShaderSettings.activeInstance.IsNotNull())
			{
				TickSystem<object>.RemovePostTickCallback(ZoneShaderSettings.activeInstance);
			}
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
			this.ApplyValues();
			ZoneShaderSettings.activeInstance = this;
			ZoneShaderSettings.hasActiveInstance = true;
		}

		// Token: 0x0600569F RID: 22175 RVA: 0x001A5804 File Offset: 0x001A3A04
		public static void ActivateDefaultSettings()
		{
			if (ZoneShaderSettings.hasDefaultsInstance)
			{
				ZoneShaderSettings.defaultsInstance.BecomeActiveInstance(false);
			}
		}

		// Token: 0x060056A0 RID: 22176 RVA: 0x001A5818 File Offset: 0x001A3A18
		public void SetGroundFogValue(Color fogColor, float fogDepthFade, float fogHeight, float fogHeightFade)
		{
			this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogColor = fogColor;
			this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogDepthFadeSize = fogDepthFade;
			this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogHeight = fogHeight;
			this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogHeightFadeSize = fogHeightFade;
			this.BecomeActiveInstance(true);
		}

		// Token: 0x060056A1 RID: 22177 RVA: 0x001A5868 File Offset: 0x001A3A68
		private void ApplyValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.ApplyColor(ZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, this.groundFogColor, ZoneShaderSettings.defaultsInstance.groundFogColor);
			this.ApplyFloat(ZoneShaderSettings.groundFogDepthFadeSq_shaderProp, this.groundFogDepthFade_overrideMode, this.GroundFogDepthFadeSq, ZoneShaderSettings.defaultsInstance.GroundFogDepthFadeSq);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeight_shaderProp, this.groundFogHeight_overrideMode, this.groundFogHeight, ZoneShaderSettings.defaultsInstance.groundFogHeight);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeightFade_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogHeightFade, ZoneShaderSettings.defaultsInstance.GroundFogHeightFade);
			if (this.zoneLiquidType_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.EZoneLiquidType ezoneLiquidType = ((this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.zoneLiquidType : ZoneShaderSettings.defaultsInstance.zoneLiquidType);
				if (ezoneLiquidType != ZoneShaderSettings.liquidType_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidTypeKeywordEnum(ezoneLiquidType);
					ZoneShaderSettings.liquidType_previousValue = ezoneLiquidType;
				}
			}
			if (this.liquidShape_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.ELiquidShape eliquidShape = ((this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.liquidShape : ZoneShaderSettings.defaultsInstance.liquidShape);
				if (eliquidShape != ZoneShaderSettings.liquidShape_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidShapeKeywordEnum(eliquidShape);
					ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
				}
			}
			this.ApplyFloat(ZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale_overrideMode, this.zoneLiquidUVScale, ZoneShaderSettings.defaultsInstance.zoneLiquidUVScale);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor_overrideMode, this.underwaterTintColor, ZoneShaderSettings.defaultsInstance.underwaterTintColor);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor_overrideMode, this.underwaterFogColor, ZoneShaderSettings.defaultsInstance.underwaterFogColor);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, this.underwaterFogParams_overrideMode, this.underwaterFogParams, ZoneShaderSettings.defaultsInstance.underwaterFogParams);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, this.underwaterCausticsParams_overrideMode, this.underwaterCausticsParams, ZoneShaderSettings.defaultsInstance.underwaterCausticsParams);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, this.underwaterCausticsTexture_overrideMode, this.underwaterCausticsTexture, ZoneShaderSettings.defaultsInstance.underwaterCausticsTexture);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, this.underwaterEffectsDistanceToSurfaceFade_overrideMode, this.underwaterEffectsDistanceToSurfaceFade, ZoneShaderSettings.defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex_overrideMode, this.liquidResidueTex, ZoneShaderSettings.defaultsInstance.liquidResidueTex);
			this.ApplyFloat(ZoneShaderSettings.shaderParam_ZoneWeatherMapDissolveProgress, this.zoneWeatherMapDissolveProgress_overrideMode, this.zoneWeatherMapDissolveProgress, ZoneShaderSettings.defaultsInstance.zoneWeatherMapDissolveProgress);
			this.UpdateMainPlaneShaderProperty();
			ZoneShaderSettings.isInitialized = true;
		}

		// Token: 0x060056A2 RID: 22178 RVA: 0x001A5ABD File Offset: 0x001A3CBD
		private void ApplyColor(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Color value, Color defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(shaderProp, value.linear);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderProp, defaultValue.linear);
			}
		}

		// Token: 0x060056A3 RID: 22179 RVA: 0x001A5AEA File Offset: 0x001A3CEA
		private void ApplyFloat(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, float value, float defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(shaderProp, defaultValue);
			}
		}

		// Token: 0x060056A4 RID: 22180 RVA: 0x001A5B0C File Offset: 0x001A3D0C
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector2 value, Vector2 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x060056A5 RID: 22181 RVA: 0x001A5B38 File Offset: 0x001A3D38
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector3 value, Vector3 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x060056A6 RID: 22182 RVA: 0x001A5B64 File Offset: 0x001A3D64
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector4 value, Vector4 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x060056A7 RID: 22183 RVA: 0x001A5B86 File Offset: 0x001A3D86
		private void ApplyTexture(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Texture2D value, Texture2D defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(shaderProp, defaultValue);
			}
		}

		// Token: 0x060056A8 RID: 22184 RVA: 0x001A5BA8 File Offset: 0x001A3DA8
		public void CopySettings(CMSZoneShaderSettings cmsZoneShaderSettings, bool rerunAwake = false, bool becomeActive = false)
		{
			this._activateOnAwake = cmsZoneShaderSettings.activateOnLoad;
			if (cmsZoneShaderSettings.applyGroundFog)
			{
				this.groundFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogColorOverrideMode();
				this.groundFogColor = cmsZoneShaderSettings.groundFogColor;
				this.groundFogHeight_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightOverrideMode();
				if (cmsZoneShaderSettings.groundFogHeightPlane.IsNotNull())
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeightPlane.position.y;
				}
				else
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeight;
				}
				this.groundFogHeightFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightFadeOverrideMode();
				this._groundFogHeightFadeSize = cmsZoneShaderSettings.groundFogHeightFadeSize;
				this.groundFogDepthFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogDepthFadeOverrideMode();
				this._groundFogDepthFadeSize = cmsZoneShaderSettings.groundFogDepthFadeSize;
			}
			else
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = new Color(0f, 0f, 0f, 0f);
				this.groundFogHeight = -9999f;
			}
			if (cmsZoneShaderSettings.applyLiquidEffects)
			{
				this.zoneLiquidType_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidTypeOverrideMode();
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)cmsZoneShaderSettings.GetZoneLiquidType();
				this.liquidShape_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeOverrideMode();
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)cmsZoneShaderSettings.GetZoneLiquidShape();
				this.liquidShapeRadius_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeRadiusOverrideMode();
				this.liquidShapeRadius = cmsZoneShaderSettings.liquidShapeRadius;
				this.liquidBottomTransform_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidBottomTransformOverrideMode();
				this.liquidBottomTransform = cmsZoneShaderSettings.liquidBottomTransform;
				this.zoneLiquidUVScale_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidUVScaleOverrideMode();
				this.zoneLiquidUVScale = cmsZoneShaderSettings.zoneLiquidUVScale;
				this.underwaterTintColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterTintColorOverrideMode();
				this.underwaterTintColor = cmsZoneShaderSettings.underwaterTintColor;
				this.underwaterFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogColorOverrideMode();
				this.underwaterFogColor = cmsZoneShaderSettings.underwaterFogColor;
				this.underwaterFogParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogParamsOverrideMode();
				this.underwaterFogParams = cmsZoneShaderSettings.underwaterFogParams;
				this.underwaterCausticsParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsParamsOverrideMode();
				this.underwaterCausticsParams = cmsZoneShaderSettings.underwaterCausticsParams;
				this.underwaterCausticsTexture_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsTextureOverrideMode();
				this.underwaterCausticsTexture = cmsZoneShaderSettings.underwaterCausticsTexture;
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterEffectsDistanceToSurfaceFadeOverrideMode();
				this.underwaterEffectsDistanceToSurfaceFade = cmsZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
				this.liquidResidueTex_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidResidueTextureOverrideMode();
				this.liquidResidueTex = cmsZoneShaderSettings.liquidResidueTex;
				this.mainWaterSurfacePlane_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetMainWaterSurfacePlaneOverrideMode();
				this.mainWaterSurfacePlane = cmsZoneShaderSettings.mainWaterSurfacePlane;
			}
			else
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = new Color(0f, 0f, 0f, 0f);
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = new Color(0f, 0f, 0f, 0f);
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				Transform transform = base.gameObject.transform.Find("DummyWaterPlane");
				GameObject gameObject;
				if (transform != null)
				{
					gameObject = transform.gameObject;
				}
				else
				{
					gameObject = new GameObject("DummyWaterPlane");
					gameObject.transform.SetParent(base.gameObject.transform);
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.position = new Vector3(0f, -9999f, 0f);
				}
				this.mainWaterSurfacePlane = gameObject.transform;
			}
			this.zoneWeatherMapDissolveProgress_overrideMode = ZoneShaderSettings.EOverrideMode.LeaveUnchanged;
			if (rerunAwake)
			{
				this.Awake();
			}
			if (becomeActive)
			{
				this.BecomeActiveInstance(true);
			}
		}

		// Token: 0x060056A9 RID: 22185 RVA: 0x001A5EC0 File Offset: 0x001A40C0
		public void CopySettings(ZoneShaderSettings zoneShaderSettings, bool rerunAwake = false, bool becomeActive = false)
		{
			this._activateOnAwake = zoneShaderSettings._activateOnAwake;
			this.groundFogColor_overrideMode = zoneShaderSettings.groundFogColor_overrideMode;
			this.groundFogColor = zoneShaderSettings.groundFogColor;
			this.groundFogHeight_overrideMode = zoneShaderSettings.groundFogHeight_overrideMode;
			this.groundFogHeight = zoneShaderSettings.groundFogHeight;
			this.groundFogHeightFade_overrideMode = zoneShaderSettings.groundFogHeightFade_overrideMode;
			this._groundFogHeightFadeSize = zoneShaderSettings._groundFogHeightFadeSize;
			this.groundFogDepthFade_overrideMode = zoneShaderSettings.groundFogDepthFade_overrideMode;
			this._groundFogDepthFadeSize = zoneShaderSettings._groundFogDepthFadeSize;
			this.zoneLiquidType_overrideMode = zoneShaderSettings.zoneLiquidType_overrideMode;
			this.zoneLiquidType = zoneShaderSettings.zoneLiquidType;
			this.liquidShape_overrideMode = zoneShaderSettings.liquidShape_overrideMode;
			this.liquidShape = zoneShaderSettings.liquidShape;
			this.liquidShapeRadius_overrideMode = zoneShaderSettings.liquidShapeRadius_overrideMode;
			this.liquidShapeRadius = zoneShaderSettings.liquidShapeRadius;
			this.liquidBottomTransform_overrideMode = zoneShaderSettings.liquidBottomTransform_overrideMode;
			this.liquidBottomTransform = zoneShaderSettings.liquidBottomTransform;
			this.zoneLiquidUVScale_overrideMode = zoneShaderSettings.zoneLiquidUVScale_overrideMode;
			this.zoneLiquidUVScale = zoneShaderSettings.zoneLiquidUVScale;
			this.underwaterTintColor_overrideMode = zoneShaderSettings.underwaterTintColor_overrideMode;
			this.underwaterTintColor = zoneShaderSettings.underwaterTintColor;
			this.underwaterFogColor_overrideMode = zoneShaderSettings.underwaterFogColor_overrideMode;
			this.underwaterFogColor = zoneShaderSettings.underwaterFogColor;
			this.underwaterFogParams_overrideMode = zoneShaderSettings.underwaterFogParams_overrideMode;
			this.underwaterFogParams = zoneShaderSettings.underwaterFogParams;
			this.underwaterCausticsParams_overrideMode = zoneShaderSettings.underwaterCausticsParams_overrideMode;
			this.underwaterCausticsParams = zoneShaderSettings.underwaterCausticsParams;
			this.underwaterCausticsTexture_overrideMode = zoneShaderSettings.underwaterCausticsTexture_overrideMode;
			this.underwaterCausticsTexture = zoneShaderSettings.underwaterCausticsTexture;
			this.underwaterEffectsDistanceToSurfaceFade_overrideMode = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade_overrideMode;
			this.underwaterEffectsDistanceToSurfaceFade = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			this.liquidResidueTex_overrideMode = zoneShaderSettings.liquidResidueTex_overrideMode;
			this.liquidResidueTex = zoneShaderSettings.liquidResidueTex;
			this.mainWaterSurfacePlane_overrideMode = zoneShaderSettings.mainWaterSurfacePlane_overrideMode;
			this.mainWaterSurfacePlane = zoneShaderSettings.mainWaterSurfacePlane;
			this.zoneWeatherMapDissolveProgress_overrideMode = zoneShaderSettings.zoneWeatherMapDissolveProgress_overrideMode;
			this.zoneWeatherMapDissolveProgress = zoneShaderSettings.zoneWeatherMapDissolveProgress;
			if (rerunAwake)
			{
				this.Awake();
			}
			if (becomeActive)
			{
				this.BecomeActiveInstance(true);
			}
		}

		// Token: 0x060056AA RID: 22186 RVA: 0x001A609C File Offset: 0x001A429C
		public void ReplaceDefaultValues(ZoneShaderSettings defaultZoneShaderSettings, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderSettings.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderSettings.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderSettings._groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderSettings._groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = defaultZoneShaderSettings.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = defaultZoneShaderSettings.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderSettings.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderSettings.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderSettings.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderSettings.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderSettings.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderSettings.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderSettings.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderSettings.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderSettings.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderSettings.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x060056AB RID: 22187 RVA: 0x001A6290 File Offset: 0x001A4490
		public void ReplaceDefaultValues(CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderProperties.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderProperties.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderProperties.groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderProperties.groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)defaultZoneShaderProperties.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)defaultZoneShaderProperties.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderProperties.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderProperties.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderProperties.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderProperties.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderProperties.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderProperties.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderProperties.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderProperties.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderProperties.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderProperties.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderProperties.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x04005A42 RID: 23106
		[OnEnterPlay_Set(false)]
		private static bool isInitialized;

		// Token: 0x04005A47 RID: 23111
		[Tooltip("Set this to true for cases like it is the first ZoneShaderSettings that should be activated when entering a scene.")]
		[SerializeField]
		private bool _activateOnAwake;

		// Token: 0x04005A48 RID: 23112
		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		// Token: 0x04005A49 RID: 23113
		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		// Token: 0x04005A4A RID: 23114
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		// Token: 0x04005A4B RID: 23115
		[SerializeField]
		private Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		// Token: 0x04005A4C RID: 23116
		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		// Token: 0x04005A4D RID: 23117
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		// Token: 0x04005A4E RID: 23118
		[SerializeField]
		private float _groundFogDepthFadeSize = 20f;

		// Token: 0x04005A4F RID: 23119
		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		// Token: 0x04005A50 RID: 23120
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		// Token: 0x04005A51 RID: 23121
		[SerializeField]
		private float groundFogHeight = 7.45f;

		// Token: 0x04005A52 RID: 23122
		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		// Token: 0x04005A53 RID: 23123
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		// Token: 0x04005A54 RID: 23124
		[SerializeField]
		private float _groundFogHeightFadeSize = 20f;

		// Token: 0x04005A55 RID: 23125
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		// Token: 0x04005A56 RID: 23126
		[SerializeField]
		private ZoneShaderSettings.EZoneLiquidType zoneLiquidType = ZoneShaderSettings.EZoneLiquidType.Water;

		// Token: 0x04005A57 RID: 23127
		[OnEnterPlay_Set(ZoneShaderSettings.EZoneLiquidType.None)]
		private static ZoneShaderSettings.EZoneLiquidType liquidType_previousValue = ZoneShaderSettings.EZoneLiquidType.None;

		// Token: 0x04005A58 RID: 23128
		[OnEnterPlay_Set(false)]
		private static bool didEverSetLiquidShape;

		// Token: 0x04005A59 RID: 23129
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShape_overrideMode;

		// Token: 0x04005A5A RID: 23130
		[SerializeField]
		private ZoneShaderSettings.ELiquidShape liquidShape;

		// Token: 0x04005A5B RID: 23131
		[OnEnterPlay_Set(ZoneShaderSettings.ELiquidShape.Plane)]
		private static ZoneShaderSettings.ELiquidShape liquidShape_previousValue = ZoneShaderSettings.ELiquidShape.Plane;

		// Token: 0x04005A5D RID: 23133
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShapeRadius_overrideMode;

		// Token: 0x04005A5E RID: 23134
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float liquidShapeRadius = 1f;

		// Token: 0x04005A5F RID: 23135
		[OnEnterPlay_Set(1f)]
		private static float liquidShapeRadius_previousValue;

		// Token: 0x04005A60 RID: 23136
		private bool hasLiquidBottomTransform;

		// Token: 0x04005A61 RID: 23137
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidBottomTransform_overrideMode;

		// Token: 0x04005A62 RID: 23138
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform liquidBottomTransform;

		// Token: 0x04005A63 RID: 23139
		private float liquidBottomPosY_previousValue;

		// Token: 0x04005A64 RID: 23140
		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		// Token: 0x04005A65 RID: 23141
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		// Token: 0x04005A66 RID: 23142
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float zoneLiquidUVScale = 1f;

		// Token: 0x04005A67 RID: 23143
		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		// Token: 0x04005A68 RID: 23144
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		// Token: 0x04005A69 RID: 23145
		[SerializeField]
		private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		// Token: 0x04005A6A RID: 23146
		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		// Token: 0x04005A6B RID: 23147
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		// Token: 0x04005A6C RID: 23148
		[SerializeField]
		private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		// Token: 0x04005A6D RID: 23149
		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		// Token: 0x04005A6E RID: 23150
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		// Token: 0x04005A6F RID: 23151
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		// Token: 0x04005A70 RID: 23152
		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		// Token: 0x04005A71 RID: 23153
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		// Token: 0x04005A72 RID: 23154
		[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
		[SerializeField]
		private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		// Token: 0x04005A73 RID: 23155
		private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		// Token: 0x04005A74 RID: 23156
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		// Token: 0x04005A75 RID: 23157
		[SerializeField]
		private Texture2D underwaterCausticsTexture;

		// Token: 0x04005A76 RID: 23158
		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		// Token: 0x04005A77 RID: 23159
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		// Token: 0x04005A78 RID: 23160
		[SerializeField]
		private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		// Token: 0x04005A79 RID: 23161
		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		// Token: 0x04005A7A RID: 23162
		private static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		// Token: 0x04005A7B RID: 23163
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private ZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		// Token: 0x04005A7C RID: 23164
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private Texture2D liquidResidueTex;

		// Token: 0x04005A7D RID: 23165
		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		// Token: 0x04005A7E RID: 23166
		private bool hasMainWaterSurfacePlane;

		// Token: 0x04005A7F RID: 23167
		private bool hasDynamicWaterSurfacePlane;

		// Token: 0x04005A80 RID: 23168
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		// Token: 0x04005A81 RID: 23169
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform mainWaterSurfacePlane;

		// Token: 0x04005A82 RID: 23170
		private static readonly int shaderParam_ZoneWeatherMapDissolveProgress = Shader.PropertyToID("_ZoneWeatherMapDissolveProgress");

		// Token: 0x04005A83 RID: 23171
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneWeatherMapDissolveProgress_overrideMode;

		// Token: 0x04005A84 RID: 23172
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[Range(0f, 1f)]
		[SerializeField]
		private float zoneWeatherMapDissolveProgress = 1f;

		// Token: 0x02000DA1 RID: 3489
		public enum EOverrideMode
		{
			// Token: 0x04005A87 RID: 23175
			LeaveUnchanged,
			// Token: 0x04005A88 RID: 23176
			ApplyNewValue,
			// Token: 0x04005A89 RID: 23177
			ApplyDefaultValue
		}

		// Token: 0x02000DA2 RID: 3490
		public enum EZoneLiquidType
		{
			// Token: 0x04005A8B RID: 23179
			None,
			// Token: 0x04005A8C RID: 23180
			Water,
			// Token: 0x04005A8D RID: 23181
			Lava
		}

		// Token: 0x02000DA3 RID: 3491
		public enum ELiquidShape
		{
			// Token: 0x04005A8F RID: 23183
			Plane,
			// Token: 0x04005A90 RID: 23184
			Cylinder
		}
	}
}
