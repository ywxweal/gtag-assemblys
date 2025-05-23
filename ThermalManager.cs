using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000220 RID: 544
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000CA6 RID: 3238 RVA: 0x00042390 File Offset: 0x00040590
	public void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTime = Time.time;
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000423C4 File Offset: 0x000405C4
	public void SliceUpdate()
	{
		float num = Time.time - this.lastTime;
		this.lastTime = Time.time;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num2 = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				Transform transform2 = thermalSourceVolume.transform;
				float x2 = transform2.lossyScale.x;
				float num3 = Vector3.Distance(transform2.position, position);
				float num4 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num3 - thermalReceiver.radius * x);
				num2 += thermalSourceVolume.celsius * num4;
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num2, num * thermalReceiver.conductivity);
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x000424CF File Offset: 0x000406CF
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x000424DC File Offset: 0x000406DC
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x000424EA File Offset: 0x000406EA
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x000424F7 File Offset: 0x000406F7
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000F2A RID: 3882
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x04000F2B RID: 3883
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x04000F2C RID: 3884
	[NonSerialized]
	public static ThermalManager instance;

	// Token: 0x04000F2D RID: 3885
	private float lastTime;
}
