using System;

namespace emotitron.Compression
{
	// Token: 0x02000E05 RID: 3589
	public static class ArraySerializeExt
	{
		// Token: 0x060058F0 RID: 22768 RVA: 0x001B5270 File Offset: 0x001B3470
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F1 RID: 22769 RVA: 0x001B5290 File Offset: 0x001B3490
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x001B52B4 File Offset: 0x001B34B4
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x001B52D8 File Offset: 0x001B34D8
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F4 RID: 22772 RVA: 0x001B52F8 File Offset: 0x001B34F8
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F5 RID: 22773 RVA: 0x001B531C File Offset: 0x001B351C
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F6 RID: 22774 RVA: 0x001B5340 File Offset: 0x001B3540
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058F7 RID: 22775 RVA: 0x001B5360 File Offset: 0x001B3560
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058F8 RID: 22776 RVA: 0x001B5384 File Offset: 0x001B3584
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058F9 RID: 22777 RVA: 0x001B53A8 File Offset: 0x001B35A8
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x001B53C8 File Offset: 0x001B35C8
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x001B53EC File Offset: 0x001B35EC
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FC RID: 22780 RVA: 0x001B5410 File Offset: 0x001B3610
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058FD RID: 22781 RVA: 0x001B5430 File Offset: 0x001B3630
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x001B5450 File Offset: 0x001B3650
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058FF RID: 22783 RVA: 0x001B5470 File Offset: 0x001B3670
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005900 RID: 22784 RVA: 0x001B5490 File Offset: 0x001B3690
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x001B54B0 File Offset: 0x001B36B0
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005902 RID: 22786 RVA: 0x001B54D0 File Offset: 0x001B36D0
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005903 RID: 22787 RVA: 0x001B54F4 File Offset: 0x001B36F4
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x001B5518 File Offset: 0x001B3718
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x001B553C File Offset: 0x001B373C
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x001B555C File Offset: 0x001B375C
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x001B557C File Offset: 0x001B377C
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x001B559B File Offset: 0x001B379B
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x001B55B2 File Offset: 0x001B37B2
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x001B55C8 File Offset: 0x001B37C8
		public static void Append(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | (value << i);
			buffer[num] = (byte)num3;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				buffer[num] = (byte)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x001B5624 File Offset: 0x001B3824
		public static void Append(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | (value << i);
			buffer[num] = (uint)num3;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				buffer[num] = (uint)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x0600590C RID: 22796 RVA: 0x001B5684 File Offset: 0x001B3884
		public static void Append(this uint[] buffer, uint value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = ((ulong)buffer[num2] & num3) | ((ulong)value << num);
			buffer[num2] = (uint)num4;
			buffer[num2 + 1] = (uint)(num4 >> 32);
			bitposition += bits;
		}

		// Token: 0x0600590D RID: 22797 RVA: 0x001B56D0 File Offset: 0x001B38D0
		public static void Append(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (buffer[num2] & num3) | (value << num);
			buffer[num2] = num4;
			buffer[num2 + 1] = value >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x0600590E RID: 22798 RVA: 0x001B571C File Offset: 0x001B391C
		public static void Write(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 7;
			int num2 = bitposition >> 3;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 8 - num;
			for (i -= 8; i > 8; i -= 8)
			{
				num2++;
				num5 = value >> num;
				buffer[num2] = (byte)num5;
				num += 8;
			}
			if (i > 0)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x0600590F RID: 22799 RVA: 0x001B57C0 File Offset: 0x001B39C0
		public static void Write(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 32 - num;
			for (i -= 32; i > 32; i -= 32)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
				num += 32;
			}
			bitposition += bits;
		}

		// Token: 0x06005910 RID: 22800 RVA: 0x001B5854 File Offset: 0x001B3A54
		public static void Write(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (buffer[num2] & ~num4) | (num5 & num4);
			num = 64 - num;
			for (i -= 64; i > 64; i -= 64)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (buffer[num2] & ~num4) | (num5 & num4);
				num += 64;
			}
			bitposition += bits;
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x001B58E4 File Offset: 0x001B3AE4
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005912 RID: 22802 RVA: 0x001B58F6 File Offset: 0x001B3AF6
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x001B5908 File Offset: 0x001B3B08
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x001B591C File Offset: 0x001B3B1C
		public static ulong Read(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x001B5978 File Offset: 0x001B3B78
		public static ulong Read(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x001B59D4 File Offset: 0x001B3BD4
		public static ulong Read(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = buffer[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x001B5A2E File Offset: 0x001B3C2E
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x001B5A38 File Offset: 0x001B3C38
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x001B5A42 File Offset: 0x001B3C42
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x001B5A4C File Offset: 0x001B3C4C
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x001B5A57 File Offset: 0x001B3C57
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x001B5A62 File Offset: 0x001B3C62
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x001B5A6D File Offset: 0x001B3C6D
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x001B5A78 File Offset: 0x001B3C78
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x001B5A83 File Offset: 0x001B3C83
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x001B5A8E File Offset: 0x001B3C8E
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x001B5A99 File Offset: 0x001B3C99
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x001B5AA4 File Offset: 0x001B3CA4
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005923 RID: 22819 RVA: 0x001B5AAF File Offset: 0x001B3CAF
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005924 RID: 22820 RVA: 0x001B5AC0 File Offset: 0x001B3CC0
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x001B5AD1 File Offset: 0x001B3CD1
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x001B5AE2 File Offset: 0x001B3CE2
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x001B5AEE File Offset: 0x001B3CEE
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06005928 RID: 22824 RVA: 0x001B5AFA File Offset: 0x001B3CFA
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x001B5B08 File Offset: 0x001B3D08
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
			bitposition += bits;
		}

		// Token: 0x0600592A RID: 22826 RVA: 0x001B5B50 File Offset: 0x001B3D50
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x0600592B RID: 22827 RVA: 0x001B5B90 File Offset: 0x001B3D90
		public static void ReadOutSafe(this byte[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x0600592C RID: 22828 RVA: 0x001B5BD0 File Offset: 0x001B3DD0
		public static void ReadOutSafe(this byte[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong num3 = source.Read(ref num, num2);
				target.Write(num3, ref bitposition, num2);
			}
		}

		// Token: 0x0600592D RID: 22829 RVA: 0x001B5C10 File Offset: 0x001B3E10
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 8) | ((ulong)buffer[num + 2] << 16) | ((ulong)buffer[num + 3] << 24) | ((ulong)buffer[num + 4] << 32) | ((ulong)buffer[num + 5] << 40) | ((ulong)buffer[num + 6] << 48) | ((ulong)buffer[num + 7] << 56);
		}

		// Token: 0x0600592E RID: 22830 RVA: 0x001B5C6C File Offset: 0x001B3E6C
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 32);
		}

		// Token: 0x0600592F RID: 22831 RVA: 0x001B5C8C File Offset: 0x001B3E8C
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | ((int)buffer[num + 1] << 8) | ((int)buffer[num + 2] << 16) | ((int)buffer[num + 3] << 24));
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x001B5CBC File Offset: 0x001B3EBC
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x001B5CDC File Offset: 0x001B3EDC
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06005932 RID: 22834 RVA: 0x001B5CFC File Offset: 0x001B3EFC
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x04005E2B RID: 24107
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
