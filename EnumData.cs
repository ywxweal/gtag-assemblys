using System;
using System.Collections.Generic;

// Token: 0x02000755 RID: 1877
public class EnumData<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x06002ED9 RID: 11993 RVA: 0x000EAC39 File Offset: 0x000E8E39
	public static EnumData<TEnum> Shared { get; } = new EnumData<TEnum>();

	// Token: 0x06002EDA RID: 11994 RVA: 0x000EAC40 File Offset: 0x000E8E40
	private EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		int num = this.Names.Length;
		this.Values = new TEnum[num];
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = Enum.Parse<TEnum>(text);
			long num2 = Convert.ToInt64(tenum);
			this.Values[i] = tenum;
			this.LongValues[i] = num2;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.EnumToIndex[tenum] = i;
			this.IndexToEnum[i] = tenum;
			this.EnumToLong[tenum] = num2;
			this.LongToEnum[num2] = tenum;
		}
		long num3 = 0L;
		bool flag = true;
		foreach (long num4 in this.LongValues)
		{
			if (num4 != 0L && (num4 & (num4 - 1L)) != 0L && (num3 & num4) != num4)
			{
				flag = false;
				break;
			}
			num3 |= num4;
		}
		this.IsBitMaskCompatible = flag;
	}

	// Token: 0x04003557 RID: 13655
	public readonly string[] Names;

	// Token: 0x04003558 RID: 13656
	public readonly TEnum[] Values;

	// Token: 0x04003559 RID: 13657
	public readonly long[] LongValues;

	// Token: 0x0400355A RID: 13658
	public readonly bool IsBitMaskCompatible;

	// Token: 0x0400355B RID: 13659
	public readonly Dictionary<TEnum, string> EnumToName;

	// Token: 0x0400355C RID: 13660
	public readonly Dictionary<string, TEnum> NameToEnum;

	// Token: 0x0400355D RID: 13661
	public readonly Dictionary<TEnum, int> EnumToIndex;

	// Token: 0x0400355E RID: 13662
	public readonly Dictionary<int, TEnum> IndexToEnum;

	// Token: 0x0400355F RID: 13663
	public readonly Dictionary<TEnum, long> EnumToLong;

	// Token: 0x04003560 RID: 13664
	public readonly Dictionary<long, TEnum> LongToEnum;
}
