using System;
using UnityEngine;

// Token: 0x020004D5 RID: 1237
public class BuilderPortraitEyes : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001DF7 RID: 7671 RVA: 0x00091BA6 File Offset: 0x0008FDA6
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.scale = base.transform.lossyScale.x;
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x00091BC5 File Offset: 0x0008FDC5
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.eyes.transform.position = this.eyeCenter.transform.position;
	}

	// Token: 0x06001DF9 RID: 7673 RVA: 0x00091BF0 File Offset: 0x0008FDF0
	public void SliceUpdate()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		Vector3 vector = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(GorillaTagger.Instance.headCollider.transform.position - this.eyeCenter.position, this.eyeCenter.forward), this.moveRadius * this.scale);
		this.eyes.transform.position = this.eyeCenter.position + vector;
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002126 RID: 8486
	[SerializeField]
	private Transform eyeCenter;

	// Token: 0x04002127 RID: 8487
	[SerializeField]
	private GameObject eyes;

	// Token: 0x04002128 RID: 8488
	[SerializeField]
	private float moveRadius = 0.5f;

	// Token: 0x04002129 RID: 8489
	private float scale = 1f;
}
