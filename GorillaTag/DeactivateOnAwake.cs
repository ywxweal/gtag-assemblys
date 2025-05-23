using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D1A RID: 3354
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060053EE RID: 21486 RVA: 0x00196BDD File Offset: 0x00194DDD
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
