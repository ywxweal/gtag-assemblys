using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D17 RID: 3351
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x060053E0 RID: 21472 RVA: 0x0019698F File Offset: 0x00194B8F
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x00196997 File Offset: 0x00194B97
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x001969A5 File Offset: 0x00194BA5
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x0019698F File Offset: 0x00194B8F
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x040056D7 RID: 22231
		[SerializeField]
		private int hashCode;
	}
}
