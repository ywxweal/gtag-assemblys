using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060029E0 RID: 10720 RVA: 0x000CF2A8 File Offset: 0x000CD4A8
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

	// Token: 0x060029E1 RID: 10721 RVA: 0x000CF2E8 File Offset: 0x000CD4E8
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000CF300 File Offset: 0x000CD500
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

	// Token: 0x060029E3 RID: 10723 RVA: 0x000CF380 File Offset: 0x000CD580
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

	// Token: 0x04002EF7 RID: 12023
	public float maxScale;

	// Token: 0x04002EF8 RID: 12024
	public float minScale;

	// Token: 0x04002EF9 RID: 12025
	public bool isAssurance;

	// Token: 0x04002EFA RID: 12026
	public bool affectLayerA = true;

	// Token: 0x04002EFB RID: 12027
	public bool affectLayerB = true;

	// Token: 0x04002EFC RID: 12028
	public bool affectLayerC = true;

	// Token: 0x04002EFD RID: 12029
	public bool affectLayerD = true;

	// Token: 0x04002EFE RID: 12030
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04002EFF RID: 12031
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04002F00 RID: 12032
	[SerializeField]
	private bool triggerWithBodyCollider;
}
