using System;

namespace emotitron.Compression
{
	// Token: 0x02000E05 RID: 3589
	public static class ArraySerializeExt
	{
		// Token: 0x060058F1 RID: 22769 RVA: 0x001B5348 File Offset: 0x001B3548
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x001B5368 File Offset: 0x001B3568
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x001B538C File Offset: 0x001B358C
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F4 RID: 22772 RVA: 0x001B53B0 File Offset: 0x001B35B0
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F5 RID: 22773 RVA: 0x001B53D0 File Offset: 0x001B35D0
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F6 RID: 22774 RVA: 0x001B53F4 File Offset: 0x001B35F4
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060058F7 RID: 22775 RVA: 0x001B5418 File Offset: 0x001B3618
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058F8 RID: 22776 RVA: 0x001B5438 File Offset: 0x001B3638
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058F9 RID: 22777 RVA: 0x001B545C File Offset: 0x001B365C
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x001B5480 File Offset: 0x001B3680
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x001B54A0 File Offset: 0x001B36A0
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FC RID: 22780 RVA: 0x001B54C4 File Offset: 0x001B36C4
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060058FD RID: 22781 RVA: 0x001B54E8 File Offset: 0x001B36E8
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x001B5508 File Offset: 0x001B3708
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060058FF RID: 22783 RVA: 0x001B5528 File Offset: 0x001B3728
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005900 RID: 22784 RVA: 0x001B5548 File Offset: 0x001B3748
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x001B5568 File Offset: 0x001B3768
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005902 RID: 22786 RVA: 0x001B5588 File Offset: 0x001B3788
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong num = (ulong)((value << 1) ^ (value >> 63));
			buffer.Write(num, ref bitposition, bits);
		}

		// Token: 0x06005903 RID: 22787 RVA: 0x001B55A8 File Offset: 0x001B37A8
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x001B55CC File Offset: 0x001B37CC
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x001B55F0 File Offset: 0x001B37F0
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x001B5614 File Offset: 0x001B3814
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x001B5634 File Offset: 0x001B3834
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x001B5654 File Offset: 0x001B3854
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)((num >> 1) ^ -(long)(num & 1UL));
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x001B5673 File Offset: 0x001B3873
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x001B568A File Offset: 0x001B388A
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x001B56A0 File Offset: 0x001B38A0
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

		// Token: 0x0600590C RID: 22796 RVA: 0x001B56FC File Offset: 0x001B38FC
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

		// Token: 0x0600590D RID: 22797 RVA: 0x001B575C File Offset: 0x001B395C
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

		// Token: 0x0600590E RID: 22798 RVA: 0x001B57A8 File Offset: 0x001B39A8
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

		// Token: 0x0600590F RID: 22799 RVA: 0x001B57F4 File Offset: 0x001B39F4
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

		// Token: 0x06005910 RID: 22800 RVA: 0x001B5898 File Offset: 0x001B3A98
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

		// Token: 0x06005911 RID: 22801 RVA: 0x001B592C File Offset: 0x001B3B2C
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

		// Token: 0x06005912 RID: 22802 RVA: 0x001B59BC File Offset: 0x001B3BBC
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x001B59CE File Offset: 0x001B3BCE
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x001B59E0 File Offset: 0x001B3BE0
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x001B59F4 File Offset: 0x001B3BF4
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

		// Token: 0x06005916 RID: 22806 RVA: 0x001B5A50 File Offset: 0x001B3C50
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

		// Token: 0x06005917 RID: 22807 RVA: 0x001B5AAC File Offset: 0x001B3CAC
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

		// Token: 0x06005918 RID: 22808 RVA: 0x001B5B06 File Offset: 0x001B3D06
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x001B5B10 File Offset: 0x001B3D10
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x001B5B1A File Offset: 0x001B3D1A
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x001B5B24 File Offset: 0x001B3D24
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x001B5B2F File Offset: 0x001B3D2F
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x001B5B3A File Offset: 0x001B3D3A
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x001B5B45 File Offset: 0x001B3D45
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x001B5B50 File Offset: 0x001B3D50
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x001B5B5B File Offset: 0x001B3D5B
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x001B5B66 File Offset: 0x001B3D66
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x001B5B71 File Offset: 0x001B3D71
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005923 RID: 22819 RVA: 0x001B5B7C File Offset: 0x001B3D7C
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06005924 RID: 22820 RVA: 0x001B5B87 File Offset: 0x001B3D87
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x001B5B98 File Offset: 0x001B3D98
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x001B5BA9 File Offset: 0x001B3DA9
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x001B5BBA File Offset: 0x001B3DBA
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06005928 RID: 22824 RVA: 0x001B5BC6 File Offset: 0x001B3DC6
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x001B5BD2 File Offset: 0x001B3DD2
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x0600592A RID: 22826 RVA: 0x001B5BE0 File Offset: 0x001B3DE0
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

		// Token: 0x0600592B RID: 22827 RVA: 0x001B5C28 File Offset: 0x001B3E28
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

		// Token: 0x0600592C RID: 22828 RVA: 0x001B5C68 File Offset: 0x001B3E68
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

		// Token: 0x0600592D RID: 22829 RVA: 0x001B5CA8 File Offset: 0x001B3EA8
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

		// Token: 0x0600592E RID: 22830 RVA: 0x001B5CE8 File Offset: 0x001B3EE8
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 8) | ((ulong)buffer[num + 2] << 16) | ((ulong)buffer[num + 3] << 24) | ((ulong)buffer[num + 4] << 32) | ((ulong)buffer[num + 5] << 40) | ((ulong)buffer[num + 6] << 48) | ((ulong)buffer[num + 7] << 56);
		}

		// Token: 0x0600592F RID: 22831 RVA: 0x001B5D44 File Offset: 0x001B3F44
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | ((ulong)buffer[num + 1] << 32);
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x001B5D64 File Offset: 0x001B3F64
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | ((int)buffer[num + 1] << 8) | ((int)buffer[num + 2] << 16) | ((int)buffer[num + 3] << 24));
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x001B5D94 File Offset: 0x001B3F94
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06005932 RID: 22834 RVA: 0x001B5DB4 File Offset: 0x001B3FB4
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06005933 RID: 22835 RVA: 0x001B5DD4 File Offset: 0x001B3FD4
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x04005E2C RID: 24108
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
