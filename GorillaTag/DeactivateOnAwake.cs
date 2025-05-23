using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D1A RID: 3354
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060053EF RID: 21487 RVA: 0x00196CB5 File Offset: 0x00194EB5
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
