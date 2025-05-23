using System;
using System.Collections.Generic;
using CjLib;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class MoonController : MonoBehaviour
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060006F4 RID: 1780 RVA: 0x00027792 File Offset: 0x00025992
	public float Distance
	{
		get
		{
			return this.distance;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0002779A File Offset: 0x0002599A
	private float TimeOfDay
	{
		get
		{
			if (this.debugOverrideTimeOfDay)
			{
				return Mathf.Repeat(this.timeOfDayOverride, 1f);
			}
			if (!(BetterDayNightManager.instance != null))
			{
				return 1f;
			}
			return BetterDayNightManager.instance.NormalizedTimeOfDay;
		}
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x000277D6 File Offset: 0x000259D6
	public void SetEyeOpenAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x000277EA File Offset: 0x000259EA
	public void StartEyeCloseAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00027800 File Offset: 0x00025A00
	private void Start()
	{
		this.eyeOpenHash = Animator.StringToHash("EyeOpen");
		this.zoneToSceneMapping.Add(GTZone.forest, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.city, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.basement, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.canyon, MoonController.Scenes.Canyon);
		this.zoneToSceneMapping.Add(GTZone.beach, MoonController.Scenes.Beach);
		this.zoneToSceneMapping.Add(GTZone.mountain, MoonController.Scenes.Mountain);
		this.zoneToSceneMapping.Add(GTZone.skyJungle, MoonController.Scenes.Clouds);
		this.zoneToSceneMapping.Add(GTZone.cave, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.cityWithSkyJungle, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.tutorial, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.rotating, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.none, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.Metropolis, MoonController.Scenes.Metropolis);
		this.zoneToSceneMapping.Add(GTZone.cityNoBuildings, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.attic, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.arcade, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.bayou, MoonController.Scenes.Bayou);
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.RegisterMoon(this);
		}
		this.crackStartDayOfYear = new DateTime(2024, 10, 4).DayOfYear;
		this.crackEndDayOfYear = new DateTime(2024, 10, 25).DayOfYear;
		if (this.crackRenderer != null)
		{
			this.currentlySetCrackProgress = 1f;
			this.crackMaterialPropertyBlock = new MaterialPropertyBlock();
			this.crackRenderer.GetPropertyBlock(this.crackMaterialPropertyBlock);
			this.crackMaterialPropertyBlock.SetFloat(this.crackProgressHash, this.currentlySetCrackProgress);
			this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
		}
		this.orbitAngle = 0f;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x000279FC File Offset: 0x00025BFC
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.UnregisterMoon(this);
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00027A1C File Offset: 0x00025C1C
	private void OnZoneChanged()
	{
		ZoneManagement instance = ZoneManagement.instance;
		MoonController.Scenes scenes = MoonController.Scenes.Forest;
		for (int i = 0; i < instance.activeZones.Count; i++)
		{
			MoonController.Scenes scenes2;
			if (this.zoneToSceneMapping.TryGetValue(instance.activeZones[i], out scenes2) && scenes2 > scenes)
			{
				scenes = scenes2;
			}
		}
		this.UpdateActiveScene(scenes);
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00027A6F File Offset: 0x00025C6F
	private void UpdateActiveScene(MoonController.Scenes nextScene)
	{
		this.activeScene = nextScene;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00027A84 File Offset: 0x00025C84
	private void Update()
	{
		this.UpdateCrack();
		if (!this.alwaysInTheSky)
		{
			float timeOfDay = this.TimeOfDay;
			bool flag = timeOfDay > 0.53999996f && timeOfDay < 0.6733333f;
			bool flag2 = timeOfDay > 0.086666666f && timeOfDay < 0.22f;
			bool flag3 = timeOfDay <= 0.086666666f || timeOfDay >= 0.6733333f;
			if (timeOfDay >= 0.22f)
			{
				bool flag4 = timeOfDay <= 0.53999996f;
			}
			float num = this.orbitAngle;
			if (flag)
			{
				num = Mathf.Lerp(3.1415927f, 0f, (timeOfDay - 0.53999996f) / 0.13333333f);
			}
			else if (flag2)
			{
				num = Mathf.Lerp(0f, -3.1415927f, (timeOfDay - 0.086666666f) / 0.13333333f);
			}
			else if (flag3)
			{
				num = 0f;
			}
			else
			{
				num = 3.1415927f;
			}
			if (this.orbitAngle != num)
			{
				this.orbitAngle = num;
				this.UpdateCrack();
				this.UpdatePlacement();
			}
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00027B75 File Offset: 0x00025D75
	public void UpdateDistance(float nextDistance)
	{
		this.distance = nextDistance;
		this.UpdateVisualState();
		this.UpdatePlacement();
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00027B8C File Offset: 0x00025D8C
	public void UpdateVisualState()
	{
		bool flag = false;
		if (GreyZoneManager.Instance != null)
		{
			flag = GreyZoneManager.Instance.GreyZoneActive;
		}
		if (flag && this.openEyeModelEnabled && this.distance < this.eyeOpenDistThreshold && !this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
			return;
		}
		if (!flag && this.distance > this.eyeCloseDistThreshold && this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00027C2C File Offset: 0x00025E2C
	public void UpdatePlacement()
	{
		if (this.alwaysInTheSky)
		{
			this.UpdatePlacementSimple();
			return;
		}
		this.UpdatePlacementOrbit();
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00027C44 File Offset: 0x00025E44
	private void UpdatePlacementSimple()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = (sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement);
		float num = Mathf.Lerp(placement.heightRange.x, placement.heightRange.y, this.distance);
		float num2 = Mathf.Lerp(placement.radiusRange.x, placement.radiusRange.y, this.distance);
		float num3 = Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		float restAngle = placement.restAngle;
		Vector3 position = referencePoint.position;
		position.y += num;
		position.x += num2 * Mathf.Cos(restAngle * 0.017453292f);
		position.z += num2 * Mathf.Sin(restAngle * 0.017453292f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * num3;
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00027D88 File Offset: 0x00025F88
	public void UpdatePlacementOrbit()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = (sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement);
		float y = placement.heightRange.y;
		float y2 = placement.radiusRange.y;
		Vector3 position = referencePoint.position;
		position.y += y;
		position.x += y2 * Mathf.Cos(placement.restAngle * 0.017453292f);
		position.z += y2 * Mathf.Sin(placement.restAngle * 0.017453292f);
		float num = Mathf.Sqrt(y * y + y2 * y2);
		float num2 = Mathf.Atan2(y, y2);
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * num2, Vector3.Cross(position - referencePoint.position, Vector3.up));
		float num3 = placement.restAngle * 0.017453292f + this.orbitAngle;
		Vector3 vector = referencePoint.position + quaternion * new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)) * num;
		if (this.distance < 1f)
		{
			Vector3 position2 = referencePoint.position;
			position2.y += placement.heightRange.x;
			position2.x += placement.radiusRange.x * Mathf.Cos(placement.restAngle * 0.017453292f);
			position2.z += placement.radiusRange.x * Mathf.Sin(placement.restAngle * 0.017453292f);
			if (Mathf.Abs(this.orbitAngle) < 0.9424779f)
			{
				vector = Vector3.Lerp(position2, vector, this.distance);
			}
			else
			{
				vector = Vector3.Lerp(position2, position, this.distance);
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		if (this.debugDrawOrbit)
		{
			int num4 = 32;
			float timeOfDay = this.TimeOfDay;
			float num5 = 0.086666666f;
			float num6 = 0.24666667f;
			float num7 = 0.6333333f;
			float num8 = 0.76f;
			bool flag = timeOfDay > num5 && timeOfDay < num6;
			bool flag2 = timeOfDay > num7 && timeOfDay < num8;
			bool flag3 = timeOfDay <= num5 || timeOfDay >= num8;
			if (timeOfDay >= num6)
			{
				bool flag4 = timeOfDay <= num7;
			}
			Color color = (flag2 ? Color.red : (flag3 ? Color.green : (flag ? Color.yellow : Color.blue)));
			Vector3 vector2 = referencePoint.position + quaternion * new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * num;
			for (int i = 1; i <= num4; i++)
			{
				float num9 = (float)i / (float)num4;
				Vector3 vector3 = referencePoint.position + quaternion * new Vector3(Mathf.Cos(6.2831855f * num9), 0f, Mathf.Sin(6.2831855f * num9)) * num;
				DebugUtil.DrawLine(vector2, vector3, color, false);
				vector2 = vector3;
			}
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00028118 File Offset: 0x00026318
	private void UpdateCrack()
	{
		bool flag = GreyZoneManager.Instance != null && GreyZoneManager.Instance.GreyZoneAvailable;
		if (flag && !this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = true;
			this.defaultMoon.gameObject.SetActive(false);
			this.openMoon.gameObject.SetActive(true);
		}
		else if (!flag && this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = false;
			this.defaultMoon.gameObject.SetActive(true);
			this.openMoon.gameObject.SetActive(false);
		}
		if (!flag && GorillaComputer.instance != null)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (this.debugOverrideCrackDayInOctober)
			{
				serverTime = new DateTime(2024, 10, Mathf.Clamp(this.crackDayInOctoberOverride, 1, 31));
			}
			float num = Mathf.InverseLerp((float)this.crackStartDayOfYear, (float)this.crackEndDayOfYear, (float)serverTime.DayOfYear);
			if (this.debugOverrideCrackProgress)
			{
				num = this.crackProgress;
			}
			float num2 = 1f - Mathf.Clamp01(num);
			if (this.crackRenderer != null && Mathf.Abs(num2 - this.currentlySetCrackProgress) > Mathf.Epsilon)
			{
				this.currentlySetCrackProgress = num2;
				this.crackMaterialPropertyBlock.SetFloat(this.crackProgressHash, this.currentlySetCrackProgress);
				this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
			}
		}
	}

	// Token: 0x0400084E RID: 2126
	[SerializeField]
	private List<MoonController.SceneData> scenes = new List<MoonController.SceneData>();

	// Token: 0x0400084F RID: 2127
	[SerializeField]
	private MoonController.Scenes activeScene;

	// Token: 0x04000850 RID: 2128
	[SerializeField]
	private MoonController.Placement defaultPlacement;

	// Token: 0x04000851 RID: 2129
	[SerializeField]
	[Range(0f, 1f)]
	private float distance;

	// Token: 0x04000852 RID: 2130
	[SerializeField]
	private bool alwaysInTheSky;

	// Token: 0x04000853 RID: 2131
	[Header("Model Swap")]
	[SerializeField]
	private Transform defaultMoon;

	// Token: 0x04000854 RID: 2132
	[SerializeField]
	private Transform openMoon;

	// Token: 0x04000855 RID: 2133
	[Header("Animation")]
	[SerializeField]
	private Animator openMoonAnimator;

	// Token: 0x04000856 RID: 2134
	[SerializeField]
	private float eyeOpenDistThreshold = 0.9f;

	// Token: 0x04000857 RID: 2135
	[SerializeField]
	private float eyeCloseDistThreshold = 0.05f;

	// Token: 0x04000858 RID: 2136
	[Header("Debug")]
	[SerializeField]
	private bool debugOverrideTimeOfDay;

	// Token: 0x04000859 RID: 2137
	[SerializeField]
	[Range(0f, 4f)]
	private float timeOfDayOverride;

	// Token: 0x0400085A RID: 2138
	[SerializeField]
	private bool debugOverrideCrackProgress;

	// Token: 0x0400085B RID: 2139
	[SerializeField]
	[Range(0f, 1f)]
	private float crackProgress;

	// Token: 0x0400085C RID: 2140
	[SerializeField]
	private bool debugOverrideCrackDayInOctober;

	// Token: 0x0400085D RID: 2141
	[SerializeField]
	[Range(1f, 31f)]
	private int crackDayInOctoberOverride = 4;

	// Token: 0x0400085E RID: 2142
	[SerializeField]
	private MeshRenderer crackRenderer;

	// Token: 0x0400085F RID: 2143
	private int crackStartDayOfYear;

	// Token: 0x04000860 RID: 2144
	private int crackEndDayOfYear;

	// Token: 0x04000861 RID: 2145
	private float orbitAngle;

	// Token: 0x04000862 RID: 2146
	private int crackProgressHash = Shader.PropertyToID("_Progress");

	// Token: 0x04000863 RID: 2147
	private int eyeOpenHash;

	// Token: 0x04000864 RID: 2148
	private bool openEyeModelEnabled;

	// Token: 0x04000865 RID: 2149
	private float currentlySetCrackProgress;

	// Token: 0x04000866 RID: 2150
	private MaterialPropertyBlock crackMaterialPropertyBlock;

	// Token: 0x04000867 RID: 2151
	private bool debugDrawOrbit;

	// Token: 0x04000868 RID: 2152
	private Dictionary<GTZone, MoonController.Scenes> zoneToSceneMapping = new Dictionary<GTZone, MoonController.Scenes>();

	// Token: 0x04000869 RID: 2153
	private const float moonFallStart = 0.086666666f;

	// Token: 0x0400086A RID: 2154
	private const float moonFallEnd = 0.22f;

	// Token: 0x0400086B RID: 2155
	private const float moonRiseStart = 0.53999996f;

	// Token: 0x0400086C RID: 2156
	private const float moonRiseEnd = 0.6733333f;

	// Token: 0x02000111 RID: 273
	public enum Scenes
	{
		// Token: 0x0400086E RID: 2158
		Forest,
		// Token: 0x0400086F RID: 2159
		Bayou,
		// Token: 0x04000870 RID: 2160
		Beach,
		// Token: 0x04000871 RID: 2161
		Canyon,
		// Token: 0x04000872 RID: 2162
		Clouds,
		// Token: 0x04000873 RID: 2163
		City,
		// Token: 0x04000874 RID: 2164
		Metropolis,
		// Token: 0x04000875 RID: 2165
		Mountain
	}

	// Token: 0x02000112 RID: 274
	[Serializable]
	public struct SceneData
	{
		// Token: 0x04000876 RID: 2166
		public MoonController.Scenes scene;

		// Token: 0x04000877 RID: 2167
		public Transform referencePoint;

		// Token: 0x04000878 RID: 2168
		public bool overridePlacement;

		// Token: 0x04000879 RID: 2169
		public MoonController.Placement PlacementOverride;
	}

	// Token: 0x02000113 RID: 275
	[Serializable]
	public struct Placement
	{
		// Token: 0x0400087A RID: 2170
		public Vector2 radiusRange;

		// Token: 0x0400087B RID: 2171
		public Vector2 heightRange;

		// Token: 0x0400087C RID: 2172
		public Vector2 scaleRange;

		// Token: 0x0400087D RID: 2173
		public float restAngle;
	}
}
