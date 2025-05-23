using System;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class ScalerUpper : MonoBehaviour
{
	// Token: 0x060016A2 RID: 5794 RVA: 0x0006CF4C File Offset: 0x0006B14C
	private void Update()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one * this.scaleCurve.Evaluate(this.t);
		}
		this.t += Time.deltaTime;
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x0006CFAB File Offset: 0x0006B1AB
	private void OnEnable()
	{
		this.t = 0f;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x0006CFB8 File Offset: 0x0006B1B8
	private void OnDisable()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one;
		}
	}

	// Token: 0x040018FA RID: 6394
	[SerializeField]
	private Transform[] target;

	// Token: 0x040018FB RID: 6395
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x040018FC RID: 6396
	private float t;
}
