using System;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x06000CB7 RID: 3255 RVA: 0x0004258B File Offset: 0x0004078B
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00042593 File Offset: 0x00040793
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x04000F32 RID: 3890
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x04000F33 RID: 3891
	public float innerRadius = 0.1f;

	// Token: 0x04000F34 RID: 3892
	public float outerRadius = 1f;
}
