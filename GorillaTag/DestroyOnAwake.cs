using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D09 RID: 3337
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x060053BD RID: 21437 RVA: 0x0019662C File Offset: 0x0019482C
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

		// Token: 0x060053BE RID: 21438 RVA: 0x0019665C File Offset: 0x0019485C
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

		// Token: 0x060053BF RID: 21439 RVA: 0x0019668C File Offset: 0x0019488C
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
