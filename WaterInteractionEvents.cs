using System;
using System.Collections.Generic;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008FE RID: 2302
public class WaterInteractionEvents : MonoBehaviour
{
	// Token: 0x060037E5 RID: 14309 RVA: 0x0010F968 File Offset: 0x0010DB68
	private void Update()
	{
		if (this.overlappingWaterVolumes.Count < 1)
		{
			if (this.inWater)
			{
				this.onExitWater.Invoke();
			}
			this.inWater = false;
			base.enabled = false;
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
		{
			WaterVolume.SurfaceQuery surfaceQuery;
			if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(this.waterContactSphere.transform.position, out surfaceQuery, false))
			{
				float num = Vector3.Dot(surfaceQuery.surfacePoint - this.waterContactSphere.transform.position, surfaceQuery.surfaceNormal);
				float num2 = Vector3.Dot(surfaceQuery.surfacePoint - Vector3.up * surfaceQuery.maxDepth - base.transform.position, surfaceQuery.surfaceNormal);
				if (num > -this.waterContactSphere.radius && num2 < this.waterContactSphere.radius)
				{
					flag = true;
				}
			}
		}
		bool flag2 = this.inWater;
		this.inWater = flag;
		if (!flag2 && this.inWater)
		{
			this.onEnterWater.Invoke();
			return;
		}
		if (flag2 && !this.inWater)
		{
			this.onExitWater.Invoke();
		}
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x0010FAA4 File Offset: 0x0010DCA4
	protected void OnTriggerEnter(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && !this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Add(component);
			base.enabled = true;
		}
	}

	// Token: 0x060037E7 RID: 14311 RVA: 0x0010FAE4 File Offset: 0x0010DCE4
	protected void OnTriggerExit(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Remove(component);
		}
	}

	// Token: 0x04003DC4 RID: 15812
	public UnityEvent onEnterWater = new UnityEvent();

	// Token: 0x04003DC5 RID: 15813
	public UnityEvent onExitWater = new UnityEvent();

	// Token: 0x04003DC6 RID: 15814
	[SerializeField]
	private SphereCollider waterContactSphere;

	// Token: 0x04003DC7 RID: 15815
	private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

	// Token: 0x04003DC8 RID: 15816
	private bool inWater;
}
