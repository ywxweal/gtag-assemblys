using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000766 RID: 1894
[Serializable]
public struct Id32
{
	// Token: 0x06002F3A RID: 12090 RVA: 0x000EBD44 File Offset: 0x000E9F44
	public Id32(string idString)
	{
		if (idString == null)
		{
			throw new ArgumentNullException("idString");
		}
		if (string.IsNullOrWhiteSpace(idString.Trim()))
		{
			throw new ArgumentNullException("idString");
		}
		this._id = XXHash32.Compute(idString, 0U);
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x000EBD79 File Offset: 0x000E9F79
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x000EBD83 File Offset: 0x000E9F83
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x000EBD8C File Offset: 0x000E9F8C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x000EBDAC File Offset: 0x000E9FAC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeHash(string s)
	{
		if (s == null)
		{
			return 0;
		}
		s = s.Trim();
		if (string.IsNullOrWhiteSpace(s))
		{
			return 0;
		}
		return XXHash32.Compute(s, 0U);
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x000EBDCC File Offset: 0x000E9FCC
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x000EBDD4 File Offset: 0x000E9FD4
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x040035A6 RID: 13734
	[SerializeField]
	private int _id;
}
