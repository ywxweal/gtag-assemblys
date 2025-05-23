using System;
using System.Collections.Generic;

// Token: 0x02000756 RID: 1878
public static class EnumUtil
{
	// Token: 0x06002EDC RID: 11996 RVA: 0x000EADD4 File Offset: 0x000E8FD4
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000EADE5 File Offset: 0x000E8FE5
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000EADF6 File Offset: 0x000E8FF6
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000EAE07 File Offset: 0x000E9007
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x000EAE19 File Offset: 0x000E9019
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000EAE2B File Offset: 0x000E902B
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x000EAE3D File Offset: 0x000E903D
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000EAE4F File Offset: 0x000E904F
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x000EAE61 File Offset: 0x000E9061
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x000EAE73 File Offset: 0x000E9073
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000EAE2B File Offset: 0x000E902B
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000EAE07 File Offset: 0x000E9007
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000EAE19 File Offset: 0x000E9019
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000EAE4F File Offset: 0x000E904F
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000EAE61 File Offset: 0x000E9061
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000EAE85 File Offset: 0x000E9085
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000EAE98 File Offset: 0x000E9098
	public static TEnum[] SplitBitmask<TEnum>(long bitmaskLong) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			throw new ArgumentException("The enum type " + typeof(TEnum).Name + " is not bitmask-compatible.");
		}
		if (bitmaskLong == 0L)
		{
			return new TEnum[] { (TEnum)((object)Enum.ToObject(typeof(TEnum), 0L)) };
		}
		List<TEnum> list = new List<TEnum>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			TEnum tenum = shared.Values[i];
			long num = shared.LongValues[i];
			if (num != 0L && (bitmaskLong & num) == num)
			{
				list.Add(tenum);
			}
		}
		return list.ToArray();
	}
}
