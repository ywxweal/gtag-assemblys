using System;
using UnityEngine;

// Token: 0x020009B7 RID: 2487
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x06003B6F RID: 15215 RVA: 0x0011B7F0 File Offset: 0x001199F0
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x06003B70 RID: 15216 RVA: 0x0011B80C File Offset: 0x00119A0C
	private void Update()
	{
		float num = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * num);
	}

	// Token: 0x04003FDD RID: 16349
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x04003FDE RID: 16350
	[SerializeField]
	private Transform source;
}
