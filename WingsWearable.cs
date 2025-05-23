using System;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class WingsWearable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600060D RID: 1549 RVA: 0x00022FCB File Offset: 0x000211CB
	private void Awake()
	{
		this.xform = this.animator.transform;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00022FDE File Offset: 0x000211DE
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.oldPos = this.xform.localPosition;
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00022FF8 File Offset: 0x000211F8
	public void SliceUpdate()
	{
		Vector3 position = this.xform.position;
		float num = (position - this.oldPos).magnitude / Time.deltaTime;
		float num2 = this.flapSpeedCurve.Evaluate(Mathf.Abs(num));
		this.animator.SetFloat(this.flapSpeedParamID, num2);
		this.oldPos = position;
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000719 RID: 1817
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x0400071A RID: 1818
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x0400071B RID: 1819
	private Transform xform;

	// Token: 0x0400071C RID: 1820
	private Vector3 oldPos;

	// Token: 0x0400071D RID: 1821
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
