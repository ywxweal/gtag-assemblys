using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x00042525 File Offset: 0x00040725
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x00042539 File Offset: 0x00040739
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00042541 File Offset: 0x00040741
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0004254F File Offset: 0x0004074F
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00042557 File Offset: 0x00040757
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0004255F File Offset: 0x0004075F
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x04000F2E RID: 3886
	public float radius = 0.2f;

	// Token: 0x04000F2F RID: 3887
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x04000F30 RID: 3888
	[DebugOption]
	public float celsius;

	// Token: 0x04000F31 RID: 3889
	private float defaultCelsius;
}
