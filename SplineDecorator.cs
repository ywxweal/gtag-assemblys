using System;
using UnityEngine;

// Token: 0x020009C5 RID: 2501
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x06003BD4 RID: 15316 RVA: 0x0011E7F4 File Offset: 0x0011C9F4
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x04004020 RID: 16416
	public BezierSpline spline;

	// Token: 0x04004021 RID: 16417
	public int frequency;

	// Token: 0x04004022 RID: 16418
	public bool lookForward;

	// Token: 0x04004023 RID: 16419
	public Transform[] items;
}
