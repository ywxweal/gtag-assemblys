using System;
using UnityEngine;

// Token: 0x020009B7 RID: 2487
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x06003B6E RID: 15214 RVA: 0x0011B718 File Offset: 0x00119918
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x0011B734 File Offset: 0x00119934
	private void Update()
	{
		float num = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * num);
	}

	// Token: 0x04003FDC RID: 16348
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x04003FDD RID: 16349
	[SerializeField]
	private Transform source;
}
