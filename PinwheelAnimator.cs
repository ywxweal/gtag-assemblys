using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x0600060A RID: 1546 RVA: 0x00022ED6 File Offset: 0x000210D6
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x00022EF4 File Offset: 0x000210F4
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float num = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, num, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x04000713 RID: 1811
	public Transform spinnerTransform;

	// Token: 0x04000714 RID: 1812
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x04000715 RID: 1813
	public float spinSpeedMultiplier = 5f;

	// Token: 0x04000716 RID: 1814
	public float damping = 0.5f;

	// Token: 0x04000717 RID: 1815
	private Vector3 oldPos;

	// Token: 0x04000718 RID: 1816
	private float spinSpeed;
}
