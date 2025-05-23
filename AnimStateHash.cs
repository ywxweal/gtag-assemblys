using System;
using UnityEngine;

// Token: 0x0200074C RID: 1868
[Serializable]
public struct AnimStateHash
{
	// Token: 0x06002EC5 RID: 11973 RVA: 0x000EA7E8 File Offset: 0x000E89E8
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000EA80B File Offset: 0x000E8A0B
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x0400354C RID: 13644
	[SerializeField]
	private int _hash;
}
