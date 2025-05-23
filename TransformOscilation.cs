using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A16 RID: 2582
public class TransformOscilation : MonoBehaviour
{
	// Token: 0x06003DB3 RID: 15795 RVA: 0x0012464C File Offset: 0x0012284C
	private void Start()
	{
		this.rootPos = base.transform.localPosition;
		this.rootRot = base.transform.localRotation.eulerAngles;
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x00124684 File Offset: 0x00122884
	private void Update()
	{
		if (this.useServerTime && GorillaComputer.instance == null)
		{
			return;
		}
		float num = Time.timeSinceLevelLoad;
		if (this.useServerTime)
		{
			this.dt = GorillaComputer.instance.GetServerTime();
			num = (float)this.dt.Minute * 60f + (float)this.dt.Second + (float)this.dt.Millisecond / 1000f;
		}
		this.offsPos.x = this.PosAmp.x * Mathf.Sin(num * this.PosFreq.x);
		this.offsPos.y = this.PosAmp.y * Mathf.Sin(num * this.PosFreq.y);
		this.offsPos.z = this.PosAmp.z * Mathf.Sin(num * this.PosFreq.z);
		this.offsRot.x = this.RotAmp.x * Mathf.Sin(num * this.RotFreq.x);
		this.offsRot.y = this.RotAmp.y * Mathf.Sin(num * this.RotFreq.y);
		this.offsRot.z = this.RotAmp.z * Mathf.Sin(num * this.RotFreq.z);
		base.transform.localPosition = this.rootPos + this.offsPos;
		base.transform.localRotation = Quaternion.Euler(this.rootRot + this.offsRot);
	}

	// Token: 0x04004179 RID: 16761
	[SerializeField]
	private Vector3 PosAmp;

	// Token: 0x0400417A RID: 16762
	[SerializeField]
	private Vector3 PosFreq;

	// Token: 0x0400417B RID: 16763
	[SerializeField]
	private Vector3 RotAmp;

	// Token: 0x0400417C RID: 16764
	[SerializeField]
	private Vector3 RotFreq;

	// Token: 0x0400417D RID: 16765
	private Vector3 rootPos;

	// Token: 0x0400417E RID: 16766
	private Vector3 rootRot;

	// Token: 0x0400417F RID: 16767
	private Vector3 offsPos = Vector3.zero;

	// Token: 0x04004180 RID: 16768
	private Vector3 offsRot = Vector3.zero;

	// Token: 0x04004181 RID: 16769
	private DateTime dt;

	// Token: 0x04004182 RID: 16770
	[SerializeField]
	private bool useServerTime;
}
