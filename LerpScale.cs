using System;
using UnityEngine;

// Token: 0x0200076A RID: 1898
public class LerpScale : LerpComponent
{
	// Token: 0x06002F53 RID: 12115 RVA: 0x000EBF70 File Offset: 0x000EA170
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x040035B3 RID: 13747
	[Space]
	public Transform target;

	// Token: 0x040035B4 RID: 13748
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x040035B5 RID: 13749
	public Vector3 end = Vector3.one;

	// Token: 0x040035B6 RID: 13750
	public Vector3 current;

	// Token: 0x040035B7 RID: 13751
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
