using System;
using UnityEngine;

// Token: 0x0200076A RID: 1898
public class LerpScale : LerpComponent
{
	// Token: 0x06002F52 RID: 12114 RVA: 0x000EBECC File Offset: 0x000EA0CC
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x040035B1 RID: 13745
	[Space]
	public Transform target;

	// Token: 0x040035B2 RID: 13746
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x040035B3 RID: 13747
	public Vector3 end = Vector3.one;

	// Token: 0x040035B4 RID: 13748
	public Vector3 current;

	// Token: 0x040035B5 RID: 13749
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
