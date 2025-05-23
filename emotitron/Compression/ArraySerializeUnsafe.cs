using System;

namespace emotitron.Compression
{
	// Token: 0x02000E06 RID: 3590
	public static class ArraySerializeUnsafe
	{
		// Token: 0x06005933 RID: 22835 RVA: 0x001B5D20 File Offset: 0x001B3F20
		public unsafe static void WriteSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005934 RID: 22836 RVA: 0x001B5D40 File Offset: 0x001B3F40
		public unsafe static void AppendSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005935 RID: 22837 RVA: 0x001B5D60 File Offset: 0x001B3F60
		public unsafe static void AddSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005936 RID: 22838 RVA: 0x001B5D80 File Offset: 0x001B3F80
		public unsafe static void AddSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005937 RID: 22839 RVA: 0x001B5DA0 File Offset: 0x001B3FA0
		public unsafe static void AddSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x001B5DC0 File Offset: 0x001B3FC0
		public unsafe static void InjectSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06005939 RID: 22841 RVA: 0x001B5DE0 File Offset: 0x001B3FE0
		public unsafe static void InjectSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x001B5E00 File Offset: 0x001B4000
		public unsafe static void InjectSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600593B RID: 22843 RVA: 0x001B5E20 File Offset: 0x001B4020
		public unsafe static void PokeSigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600593C RID: 22844 RVA: 0x001B5E44 File Offset: 0x001B4044
		public unsafe static void PokeSigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600593D RID: 22845 RVA: 0x001B5E68 File Offset: 0x001B4068
		public unsafe static void PokeSigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(((int)value << 1) ^ (value >> 31));
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600593E RID: 22846 RVA: 0x001B5E8C File Offset: 0x001B408C
		public unsafe static int ReadSigned(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600593F RID: 22847 RVA: 0x001B5EB0 File Offset: 0x001B40B0
		public unsafe static int PeekSigned(ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06005940 RID: 22848 RVA: 0x001B5ED4 File Offset: 0x001B40D4
		public unsafe static void Append(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (uPtr[num2] & num3) | (value << num);
			uPtr[num2] = num4;
			uPtr[num2 + 1] = num4 >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06005941 RID: 22849 RVA: 0x001B5F2C File Offset: 0x001B412C
		public unsafe static void Write(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			uPtr[num2] = (uPtr[num2] & ~num4) | (num5 & num4);
			num = 64 - num;
			if (num < bits)
			{
				num4 = num3 >> num;
				num5 = value >> num;
				num2++;
				uPtr[num2] = (uPtr[num2] & ~num4) | (num5 & num4);
			}
			bitposition += bits;
		}

		// Token: 0x06005942 RID: 22850 RVA: 0x001B5FB0 File Offset: 0x001B41B0
		public unsafe static ulong Read(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06005943 RID: 22851 RVA: 0x001B6014 File Offset: 0x001B4214
		public unsafe static ulong Read(ulong* uPtr, int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06005944 RID: 22852 RVA: 0x001B6073 File Offset: 0x001B4273
		public unsafe static void Add(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06005945 RID: 22853 RVA: 0x001B607F File Offset: 0x001B427F
		public unsafe static void Add(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005946 RID: 22854 RVA: 0x001B607F File Offset: 0x001B427F
		public unsafe static void Add(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005947 RID: 22855 RVA: 0x001B607F File Offset: 0x001B427F
		public unsafe static void Add(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x001B6073 File Offset: 0x001B4273
		public unsafe static void AddUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x001B608C File Offset: 0x001B428C
		public unsafe static void AddUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x001B608C File Offset: 0x001B428C
		public unsafe static void AddUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x001B608C File Offset: 0x001B428C
		public unsafe static void AddUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x001B6099 File Offset: 0x001B4299
		public unsafe static void Inject(this ulong value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x001B60A4 File Offset: 0x001B42A4
		public unsafe static void Inject(this uint value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600594E RID: 22862 RVA: 0x001B60A4 File Offset: 0x001B42A4
		public unsafe static void Inject(this ushort value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600594F RID: 22863 RVA: 0x001B60A4 File Offset: 0x001B42A4
		public unsafe static void Inject(this byte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x001B6099 File Offset: 0x001B4299
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005951 RID: 22865 RVA: 0x001B60B0 File Offset: 0x001B42B0
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06005952 RID: 22866 RVA: 0x001B60BC File Offset: 0x001B42BC
		public unsafe static void InjectUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06005953 RID: 22867 RVA: 0x001B60B0 File Offset: 0x001B42B0
		public unsafe static void InjectUnsigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06005954 RID: 22868 RVA: 0x001B60C9 File Offset: 0x001B42C9
		public unsafe static void Poke(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06005955 RID: 22869 RVA: 0x001B60D5 File Offset: 0x001B42D5
		public unsafe static void Poke(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005956 RID: 22870 RVA: 0x001B60D5 File Offset: 0x001B42D5
		public unsafe static void Poke(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005957 RID: 22871 RVA: 0x001B60D5 File Offset: 0x001B42D5
		public unsafe static void Poke(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005958 RID: 22872 RVA: 0x001B60C9 File Offset: 0x001B42C9
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06005959 RID: 22873 RVA: 0x001B60BC File Offset: 0x001B42BC
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600595A RID: 22874 RVA: 0x001B60BC File Offset: 0x001B42BC
		public unsafe static void PokeUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600595B RID: 22875 RVA: 0x001B60BC File Offset: 0x001B42BC
		public unsafe static void PokeUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600595C RID: 22876 RVA: 0x001B60E4 File Offset: 0x001B42E4
		public unsafe static void ReadOutUnsafe(ulong* sourcePtr, int sourcePos, ulong* targetPtr, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong num3 = ArraySerializeUnsafe.Read(sourcePtr, ref num, num2);
				ArraySerializeUnsafe.Write(targetPtr, num3, ref targetPos, num2);
			}
			targetPos += bits;
		}

		// Token: 0x0600595D RID: 22877 RVA: 0x001B612C File Offset: 0x001B432C
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr3, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x0600595E RID: 22878 RVA: 0x001B61B8 File Offset: 0x001B43B8
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr3, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x0600595F RID: 22879 RVA: 0x001B6244 File Offset: 0x001B4444
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005960 RID: 22880 RVA: 0x001B62CC File Offset: 0x001B44CC
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x001B635C File Offset: 0x001B455C
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x001B63EC File Offset: 0x001B45EC
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x001B6478 File Offset: 0x001B4678
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr2, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x001B6504 File Offset: 0x001B4704
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x001B6594 File Offset: 0x001B4794
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* ptr3 = (ulong*)ptr;
					ulong* ptr4 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = ((i > 64) ? 64 : i);
						ulong num3 = ArraySerializeUnsafe.Read(ptr3, ref num, num2);
						ArraySerializeUnsafe.Write(ptr4, num3, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x04005E2C RID: 24108
		private const string bufferOverrunMsg = "Byte buffer overrun. Dataloss will occur.";
	}
}
