using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D3D RID: 3389
	[Serializable]
	public class CoolDownHelper
	{
		// Token: 0x060054E7 RID: 21735 RVA: 0x0019DE23 File Offset: 0x0019C023
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		// Token: 0x060054E8 RID: 21736 RVA: 0x0019DE41 File Offset: 0x0019C041
		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

		// Token: 0x060054E9 RID: 21737 RVA: 0x0019DE5B File Offset: 0x0019C05B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CheckCooldown()
		{
			if (Time.time < this.checkTime)
			{
				return false;
			}
			this.OnCheckPass();
			this.checkTime = Time.unscaledTime + this.coolDown;
			return true;
		}

		// Token: 0x060054EA RID: 21738 RVA: 0x0019DE85 File Offset: 0x0019C085
		public virtual void Start()
		{
			this.checkTime = Time.time + this.coolDown;
		}

		// Token: 0x060054EB RID: 21739 RVA: 0x0019DE99 File Offset: 0x0019C099
		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		// Token: 0x060054EC RID: 21740 RVA: 0x000023F4 File Offset: 0x000005F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void OnCheckPass()
		{
		}

		// Token: 0x04005847 RID: 22599
		public float coolDown;

		// Token: 0x04005848 RID: 22600
		[NonSerialized]
		public float checkTime;
	}
}
