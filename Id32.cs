﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000766 RID: 1894
[Serializable]
public struct Id32
{
	// Token: 0x06002F3B RID: 12091 RVA: 0x000EBDE8 File Offset: 0x000E9FE8
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

	// Token: 0x06002F3C RID: 12092 RVA: 0x000EBE1D File Offset: 0x000EA01D
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x000EBE27 File Offset: 0x000EA027
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x000EBE30 File Offset: 0x000EA030
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x000EBE50 File Offset: 0x000EA050
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

	// Token: 0x06002F40 RID: 12096 RVA: 0x000EBE70 File Offset: 0x000EA070
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x000EBE78 File Offset: 0x000EA078
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x040035A8 RID: 13736
	[SerializeField]
	private int _id;
}
