using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000CB0 RID: 3248
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x0600504E RID: 20558 RVA: 0x0017FD5E File Offset: 0x0017DF5E
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04005381 RID: 21377
		[SerializeField]
		private bool right;
	}
}
