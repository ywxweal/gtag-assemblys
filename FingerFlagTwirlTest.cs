using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x06000583 RID: 1411 RVA: 0x0001FFA4 File Offset: 0x0001E1A4
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x04000678 RID: 1656
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x04000679 RID: 1657
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x0400067A RID: 1658
	public AnimationCurve rotXAnimCurve;

	// Token: 0x0400067B RID: 1659
	public AnimationCurve rotYAnimCurve;

	// Token: 0x0400067C RID: 1660
	public AnimationCurve rotZAnimCurve;

	// Token: 0x0400067D RID: 1661
	private Vector3 animTimes = Vector3.zero;
}
