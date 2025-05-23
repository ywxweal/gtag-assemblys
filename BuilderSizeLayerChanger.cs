using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class BuilderSizeLayerChanger : MonoBehaviour
{
	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001E01 RID: 7681 RVA: 0x00091D14 File Offset: 0x0008FF14
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x00091D54 File Offset: 0x0008FF54
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x00091D6C File Offset: 0x0008FF6C
	public void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x00091DF8 File Offset: 0x0008FFF8
	public void OnTriggerExit(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x0400212E RID: 8494
	public float maxScale;

	// Token: 0x0400212F RID: 8495
	public float minScale;

	// Token: 0x04002130 RID: 8496
	public bool isAssurance;

	// Token: 0x04002131 RID: 8497
	public bool affectLayerA = true;

	// Token: 0x04002132 RID: 8498
	public bool affectLayerB = true;

	// Token: 0x04002133 RID: 8499
	public bool affectLayerC = true;

	// Token: 0x04002134 RID: 8500
	public bool affectLayerD = true;

	// Token: 0x04002135 RID: 8501
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04002136 RID: 8502
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04002137 RID: 8503
	[SerializeField]
	private GameObject fxForLayerChange;
}
