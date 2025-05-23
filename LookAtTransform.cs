using System;
using UnityEngine;

// Token: 0x0200099D RID: 2461
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x06003AF6 RID: 15094 RVA: 0x00119AD3 File Offset: 0x00117CD3
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x04003FC0 RID: 16320
	[SerializeField]
	private Transform lookAt;
}
