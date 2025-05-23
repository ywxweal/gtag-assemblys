using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000A03 RID: 2563
public class ReportTargetHit : MonoBehaviour
{
	// Token: 0x06003D40 RID: 15680 RVA: 0x00122C68 File Offset: 0x00120E68
	private void Start()
	{
		this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x00122C86 File Offset: 0x00120E86
	private void OnEnable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x00122CAD File Offset: 0x00120EAD
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x00122CD4 File Offset: 0x00120ED4
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06003D44 RID: 15684 RVA: 0x00122CDC File Offset: 0x00120EDC
	private void Update()
	{
		if (this.nsRand != null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06003D45 RID: 15685 RVA: 0x00122D48 File Offset: 0x00120F48
	private void seek()
	{
		if (this.targets.Length != 0)
		{
			Vector3 vector = this.targets[ReportTargetHit.rand.NextInt(this.targets.Length)].position - base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector, out raycastHit) && this.colliderFound != null)
			{
				this.colliderFound.Invoke(base.transform.position, raycastHit.point);
			}
		}
	}

	// Token: 0x040040FC RID: 16636
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x040040FD RID: 16637
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x040040FE RID: 16638
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x040040FF RID: 16639
	[SerializeField]
	private Transform[] targets;

	// Token: 0x04004100 RID: 16640
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04004101 RID: 16641
	private float timeSinceSeek;

	// Token: 0x04004102 RID: 16642
	private float seekFreq;

	// Token: 0x04004103 RID: 16643
	[SerializeField]
	private RandomDispatcher nsRand;
}
