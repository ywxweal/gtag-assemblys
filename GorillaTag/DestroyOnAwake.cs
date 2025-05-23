using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D09 RID: 3337
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x060053BC RID: 21436 RVA: 0x00196554 File Offset: 0x00194754
		protected void Awake()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x060053BD RID: 21437 RVA: 0x00196584 File Offset: 0x00194784
		protected void OnEnable()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x060053BE RID: 21438 RVA: 0x001965B4 File Offset: 0x001947B4
		protected void Update()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
