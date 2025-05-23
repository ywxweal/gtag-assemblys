using System;
using System.Collections.Generic;

// Token: 0x02000756 RID: 1878
public static class EnumUtil
{
	// Token: 0x06002EDD RID: 11997 RVA: 0x000EAE78 File Offset: 0x000E9078
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000EAE89 File Offset: 0x000E9089
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000EAE9A File Offset: 0x000E909A
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x000EAEAB File Offset: 0x000E90AB
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000EAEBD File Offset: 0x000E90BD
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x000EAECF File Offset: 0x000E90CF
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000EAEE1 File Offset: 0x000E90E1
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x000EAEF3 File Offset: 0x000E90F3
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x000EAF05 File Offset: 0x000E9105
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000EAF17 File Offset: 0x000E9117
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000EAECF File Offset: 0x000E90CF
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000EAEAB File Offset: 0x000E90AB
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000EAEBD File Offset: 0x000E90BD
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000EAEF3 File Offset: 0x000E90F3
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000EAF05 File Offset: 0x000E9105
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000EAF29 File Offset: 0x000E9129
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x000EAF3C File Offset: 0x000E913C
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
