using System;
using Unity.Burst;

// Token: 0x020008A2 RID: 2210
public static class LuaHashing
{
	// Token: 0x0600355A RID: 13658 RVA: 0x00103658 File Offset: 0x00101858
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes, int len)
	{
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < len; i += 2)
		{
			num = ((num << 5) + num) ^ (int)bytes[i];
			if (i == len - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i + 1];
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x001036A0 File Offset: 0x001018A0
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes)
	{
		int num = 352654597;
		int num2 = num;
		int num3 = 0;
		while (bytes[num3] != 0)
		{
			num = ((num << 5) + num) ^ (int)bytes[num3];
			num3++;
			if (bytes[num3] == 0)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[num3];
			num3++;
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x001036EC File Offset: 0x001018EC
	public static int ByteHash(string bytes)
	{
		int length = bytes.Length;
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < length; i += 2)
		{
			num = ((num << 5) + num) ^ (int)bytes[i];
			if (i == length - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i + 1];
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x00103744 File Offset: 0x00101944
	[BurstCompile]
	public static int ByteHash(byte[] bytes)
	{
		int num = bytes.Length;
		int num2 = 352654597;
		int num3 = num2;
		for (int i = 0; i < num; i += 2)
		{
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i];
			if (i == num - 1)
			{
				break;
			}
			num3 = ((num3 << 5) + num3) ^ (int)bytes[i + 1];
		}
		return num2 + num3 * 1648465312;
	}

	// Token: 0x04003B5D RID: 15197
	private const int k_enhancer = 1648465312;

	// Token: 0x04003B5E RID: 15198
	private const int k_Seed = 352654597;
}
