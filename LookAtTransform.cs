using System;
using UnityEngine;

// Token: 0x0200099D RID: 2461
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x06003AF7 RID: 15095 RVA: 0x00119BAB File Offset: 0x00117DAB
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x04003FC1 RID: 16321
	[SerializeField]
	private Transform lookAt;
}
