using System;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class TestManipulatableSpinner : MonoBehaviour
{
	// Token: 0x06001947 RID: 6471 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x0007A688 File Offset: 0x00078888
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.rotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04001C12 RID: 7186
	public ManipulatableSpinner spinner;

	// Token: 0x04001C13 RID: 7187
	public float rotationScale = 1f;
}
