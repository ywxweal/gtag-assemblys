using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D17 RID: 3351
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x060053DF RID: 21471 RVA: 0x001968B7 File Offset: 0x00194AB7
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x060053E0 RID: 21472 RVA: 0x001968BF File Offset: 0x00194ABF
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x001968CD File Offset: 0x00194ACD
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x001968B7 File Offset: 0x00194AB7
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x040056D6 RID: 22230
		[SerializeField]
		private int hashCode;
	}
}
