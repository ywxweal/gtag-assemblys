using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x060029E5 RID: 10725 RVA: 0x000CF42A File Offset: 0x000CD62A
	public bool MomentaryGrabOnly()
	{
		return this.momentaryGrabOnly;
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x00047642 File Offset: 0x00045842
	bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x000CF434 File Offset: 0x000CD634
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosiiton)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		grabbedObject = base.transform;
		grabbedLocalPosiiton = base.transform.InverseTransformPoint(g.transform.position);
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000CF49C File Offset: 0x000CD69C
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x0001396B File Offset: 0x00011B6B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04002F01 RID: 12033
	[SerializeField]
	private bool grabChangesSizeLayer = true;

	// Token: 0x04002F02 RID: 12034
	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	// Token: 0x04002F03 RID: 12035
	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	// Token: 0x04002F04 RID: 12036
	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;

	// Token: 0x04002F05 RID: 12037
	[SerializeField]
	private bool momentaryGrabOnly = true;
}
