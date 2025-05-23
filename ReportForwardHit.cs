using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000A02 RID: 2562
public class ReportForwardHit : MonoBehaviour
{
	// Token: 0x06003D39 RID: 15673 RVA: 0x00122B79 File Offset: 0x00120D79
	private void Start()
	{
		this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x00122B97 File Offset: 0x00120D97
	private void OnEnable()
	{
		if (this.seekOnEnable)
		{
			this.seek();
		}
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x06003D3B RID: 15675 RVA: 0x00122BCC File Offset: 0x00120DCC
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x06003D3C RID: 15676 RVA: 0x00122BF3 File Offset: 0x00120DF3
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06003D3D RID: 15677 RVA: 0x00122BFC File Offset: 0x00120DFC
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
			this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x00122C68 File Offset: 0x00120E68
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.maxRadias * num) && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, raycastHit.point);
		}
	}

	// Token: 0x040040F4 RID: 16628
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x040040F5 RID: 16629
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x040040F6 RID: 16630
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x040040F7 RID: 16631
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x040040F8 RID: 16632
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x040040F9 RID: 16633
	[SerializeField]
	private RandomDispatcher nsRand;

	// Token: 0x040040FA RID: 16634
	private float timeSinceSeek;

	// Token: 0x040040FB RID: 16635
	private float seekFreq;

	// Token: 0x040040FC RID: 16636
	[SerializeField]
	private bool seekOnEnable;
}
