using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class GameLightingManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002202 RID: 8706 RVA: 0x000AA788 File Offset: 0x000A8988
	private void Awake()
	{
		this.InitData();
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000AA790 File Offset: 0x000A8990
	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		this.lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightData>());
		this.lightData = new GameLightingManager.LightData[50];
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000AA7FA File Offset: 0x000A89FA
	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000AA820 File Offset: 0x000A8A20
	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000AA846 File Offset: 0x000A8A46
	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor("_GT_GameLight_Ambient_Color", color);
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000AA853 File Offset: 0x000A8A53
	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor("_GT_DesaturateAndTint_TintColor", tint);
		Shader.SetGlobalFloat("_GT_DesaturateAndTint_TintAmount", enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000AA880 File Offset: 0x000A8A80
	public void SliceUpdate()
	{
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000AA8BD File Offset: 0x000A8ABD
	public void SortLights()
	{
		if (this.gameLights.Count <= 50)
		{
			return;
		}
		this.cameraPosForSort = this.mainCameraTransform.position;
		this.gameLights.Sort(new Comparison<GameLight>(this.CompareDistFromCamera));
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000AA8F7 File Offset: 0x000A8AF7
	private void Update()
	{
		this.RefreshLightData();
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x000AA900 File Offset: 0x000A8B00
	private void RefreshLightData()
	{
		if (this.lightData == null)
		{
			return;
		}
		if (this.customVertexLightingEnabled)
		{
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.SortLights();
			}
			for (int i = 0; i < 50; i++)
			{
				if (i < this.gameLights.Count)
				{
					this.GetFromLight(i, i);
				}
				else
				{
					this.ResetLight(i);
				}
			}
			this.lightDataBuffer.SetData(this.lightData);
			Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
		}
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000AA988 File Offset: 0x000A8B88
	public int AddGameLight(GameLight light)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (this.gameLights.Contains(light))
		{
			return -1;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000AA9F8 File Offset: 0x000A8BF8
	public void RemoveGameLight(GameLight light)
	{
		this.gameLights.Remove(light);
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000AAA08 File Offset: 0x000A8C08
	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		if (this.lightData == null)
		{
			return;
		}
		for (int i = 0; i < this.lightData.Length; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData(this.lightData);
		Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000AAA6C File Offset: 0x000A8C6C
	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (this.lightData == null)
		{
			return;
		}
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		Vector4 vector = gameLight.light.transform.position;
		vector.w = 1f;
		Color color = gameLight.light.color * gameLight.light.intensity;
		Vector3 forward = gameLight.light.transform.forward;
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = vector,
			lightColor = color,
			lightDirection = forward
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000AAB48 File Offset: 0x000A8D48
	private void ResetLight(int lightIndex)
	{
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = Vector4.zero,
			lightColor = Color.black,
			lightDirection = Vector4.zero
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x000AAB98 File Offset: 0x000A8D98
	private int CompareDistFromCamera(GameLight a, GameLight b)
	{
		if (a == null || a.light == null)
		{
			if (b == null || b.light == null)
			{
				return 0;
			}
			return -1;
		}
		else
		{
			if (b == null || b.light == null)
			{
				return 1;
			}
			float sqrMagnitude = (this.cameraPosForSort - a.light.transform.position).sqrMagnitude;
			float sqrMagnitude2 = (this.cameraPosForSort - b.light.transform.position).sqrMagnitude;
			return sqrMagnitude.CompareTo(sqrMagnitude2);
		}
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002610 RID: 9744
	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	// Token: 0x04002611 RID: 9745
	public const int MAX_VERTEX_LIGHTS = 50;

	// Token: 0x04002612 RID: 9746
	public Transform testLightsCenter;

	// Token: 0x04002613 RID: 9747
	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	// Token: 0x04002614 RID: 9748
	public float testLightBrightness = 10f;

	// Token: 0x04002615 RID: 9749
	public float testLightRadius = 2f;

	// Token: 0x04002616 RID: 9750
	public int maxUseTestLights = 1;

	// Token: 0x04002617 RID: 9751
	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	// Token: 0x04002618 RID: 9752
	private bool customVertexLightingEnabled;

	// Token: 0x04002619 RID: 9753
	private bool desaturateAndTintEnabled;

	// Token: 0x0400261A RID: 9754
	private Transform mainCameraTransform;

	// Token: 0x0400261B RID: 9755
	private GameLightingManager.LightData[] lightData;

	// Token: 0x0400261C RID: 9756
	private GraphicsBuffer lightDataBuffer;

	// Token: 0x0400261D RID: 9757
	private Vector3 cameraPosForSort;

	// Token: 0x0400261E RID: 9758
	private bool skipNextSlice;

	// Token: 0x0400261F RID: 9759
	private bool immediateSort;

	// Token: 0x02000572 RID: 1394
	private struct LightData
	{
		// Token: 0x04002620 RID: 9760
		public Vector4 lightPos;

		// Token: 0x04002621 RID: 9761
		public Vector4 lightColor;

		// Token: 0x04002622 RID: 9762
		public Vector4 lightDirection;
	}
}
