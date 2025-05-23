using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000CB0 RID: 3248
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x0600504D RID: 20557 RVA: 0x0017FC86 File Offset: 0x0017DE86
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04005380 RID: 21376
		[SerializeField]
		private bool right;
	}
}
