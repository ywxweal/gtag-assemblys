using System;
using UnityEngine;

// Token: 0x0200074C RID: 1868
[Serializable]
public struct AnimStateHash
{
	// Token: 0x06002EC4 RID: 11972 RVA: 0x000EA744 File Offset: 0x000E8944
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000EA767 File Offset: 0x000E8967
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x0400354A RID: 13642
	[SerializeField]
	private int _hash;
}
