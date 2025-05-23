using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060029E1 RID: 10721 RVA: 0x000CF34C File Offset: 0x000CD54C
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

	// Token: 0x060029E2 RID: 10722 RVA: 0x000CF38C File Offset: 0x000CD58C
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000CF3A4 File Offset: 0x000CD5A4
	public void OnTriggerEnter(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000CF424 File Offset: 0x000CD624
	public void OnTriggerExit(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04002EF9 RID: 12025
	public float maxScale;

	// Token: 0x04002EFA RID: 12026
	public float minScale;

	// Token: 0x04002EFB RID: 12027
	public bool isAssurance;

	// Token: 0x04002EFC RID: 12028
	public bool affectLayerA = true;

	// Token: 0x04002EFD RID: 12029
	public bool affectLayerB = true;

	// Token: 0x04002EFE RID: 12030
	public bool affectLayerC = true;

	// Token: 0x04002EFF RID: 12031
	public bool affectLayerD = true;

	// Token: 0x04002F00 RID: 12032
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04002F01 RID: 12033
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04002F02 RID: 12034
	[SerializeField]
	private bool triggerWithBodyCollider;
}
