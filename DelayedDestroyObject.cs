using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class DelayedDestroyObject : MonoBehaviour
{
	// Token: 0x060002EE RID: 750 RVA: 0x000126C5 File Offset: 0x000108C5
	private void Start()
	{
		this._timeToDie = Time.time + this.lifetime;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x000126D9 File Offset: 0x000108D9
	private void LateUpdate()
	{
		if (Time.time >= this._timeToDie)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400039B RID: 923
	public float lifetime = 10f;

	// Token: 0x0400039C RID: 924
	private float _timeToDie;
}
