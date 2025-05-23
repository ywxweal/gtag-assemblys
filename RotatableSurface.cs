using System;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public class RotatableSurface : MonoBehaviour
{
	// Token: 0x06001B4E RID: 6990 RVA: 0x000869D0 File Offset: 0x00084BD0
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.localRotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04001E4B RID: 7755
	public ManipulatableSpinner spinner;

	// Token: 0x04001E4C RID: 7756
	public float rotationScale = 1f;
}
