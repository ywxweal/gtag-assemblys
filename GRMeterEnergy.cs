using System;
using UnityEngine;

// Token: 0x020005B4 RID: 1460
public class GRMeterEnergy : MonoBehaviour
{
	// Token: 0x060023A1 RID: 9121 RVA: 0x000023F4 File Offset: 0x000005F4
	public void Awake()
	{
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000B34B4 File Offset: 0x000B16B4
	public void Refresh()
	{
		float num = 0f;
		if (this.tool != null && this.tool.maxEnergy > 0)
		{
			num = (float)this.tool.energy / (float)this.tool.maxEnergy;
		}
		num = Mathf.Clamp(num, 0f, 1f);
		GRMeterEnergy.MeterType meterType = this.meterType;
		if (meterType == GRMeterEnergy.MeterType.Linear || meterType != GRMeterEnergy.MeterType.Radial)
		{
			this.meter.localScale = new Vector3(1f, num, 1f);
			return;
		}
		float num2 = Mathf.Lerp(this.angularRange.x, this.angularRange.y, num);
		Vector3 zero = Vector3.zero;
		zero[this.rotationAxis] = num2;
		this.meter.localRotation = Quaternion.Euler(zero);
	}

	// Token: 0x0400285A RID: 10330
	public GRTool tool;

	// Token: 0x0400285B RID: 10331
	public Transform meter;

	// Token: 0x0400285C RID: 10332
	public Transform chargePoint;

	// Token: 0x0400285D RID: 10333
	public GRMeterEnergy.MeterType meterType;

	// Token: 0x0400285E RID: 10334
	public Vector2 angularRange = new Vector2(-45f, 45f);

	// Token: 0x0400285F RID: 10335
	[Range(0f, 2f)]
	public int rotationAxis;

	// Token: 0x020005B5 RID: 1461
	public enum MeterType
	{
		// Token: 0x04002861 RID: 10337
		Linear,
		// Token: 0x04002862 RID: 10338
		Radial
	}
}
