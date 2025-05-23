using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000E15 RID: 3605
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06005A29 RID: 23081 RVA: 0x001B7D46 File Offset: 0x001B5F46
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06005A2A RID: 23082 RVA: 0x001B7D54 File Offset: 0x001B5F54
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x001B7D5C File Offset: 0x001B5F5C
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x001B7D98 File Offset: 0x001B5F98
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06005A2D RID: 23085 RVA: 0x001B7DCC File Offset: 0x001B5FCC
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x001B7DE8 File Offset: 0x001B5FE8
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x001B7DF5 File Offset: 0x001B5FF5
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06005A30 RID: 23088 RVA: 0x001B7E02 File Offset: 0x001B6002
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x001B7E0F File Offset: 0x001B600F
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x001B7E21 File Offset: 0x001B6021
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x001B7E33 File Offset: 0x001B6033
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x001B7E48 File Offset: 0x001B6048
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06005A35 RID: 23093 RVA: 0x001B7E5D File Offset: 0x001B605D
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x001B7E67 File Offset: 0x001B6067
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06005A37 RID: 23095 RVA: 0x001B7E74 File Offset: 0x001B6074
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06005A38 RID: 23096 RVA: 0x001B7E7C File Offset: 0x001B607C
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06005A39 RID: 23097 RVA: 0x001B7E8C File Offset: 0x001B608C
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x001B7EBC File Offset: 0x001B60BC
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x001B7F04 File Offset: 0x001B6104
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x001B7F30 File Offset: 0x001B6130
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x001B7F69 File Offset: 0x001B6169
		public override int GetHashCode()
		{
			return (int)((this.value * 3 / 2) ^ this.value);
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x001B7F7C File Offset: 0x001B617C
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06005A3F RID: 23103 RVA: 0x001B7FD4 File Offset: 0x001B61D4
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06005A40 RID: 23104 RVA: 0x001B8048 File Offset: 0x001B6248
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06005A41 RID: 23105 RVA: 0x001B8058 File Offset: 0x001B6258
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06005A42 RID: 23106 RVA: 0x001B8068 File Offset: 0x001B6268
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x001B809A File Offset: 0x001B629A
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06005A44 RID: 23108 RVA: 0x001B80AB File Offset: 0x001B62AB
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x001B80BD File Offset: 0x001B62BD
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x001B80CF File Offset: 0x001B62CF
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06005A47 RID: 23111 RVA: 0x001B80DB File Offset: 0x001B62DB
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06005A48 RID: 23112 RVA: 0x001B80E7 File Offset: 0x001B62E7
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06005A49 RID: 23113 RVA: 0x001B80F9 File Offset: 0x001B62F9
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06005A4A RID: 23114 RVA: 0x001B810B File Offset: 0x001B630B
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x001B811D File Offset: 0x001B631D
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06005A4C RID: 23116 RVA: 0x001B812F File Offset: 0x001B632F
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06005A4D RID: 23117 RVA: 0x001B8141 File Offset: 0x001B6341
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x001B8153 File Offset: 0x001B6353
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06005A4F RID: 23119 RVA: 0x001B8160 File Offset: 0x001B6360
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06005A50 RID: 23120 RVA: 0x001B817A File Offset: 0x001B637A
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x001B818C File Offset: 0x001B638C
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06005A52 RID: 23122 RVA: 0x001B819E File Offset: 0x001B639E
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04005E6C RID: 24172
		private ushort value;

		// Token: 0x04005E6D RID: 24173
		public const int PrecisionDigits = 3;

		// Token: 0x04005E6E RID: 24174
		public const int MantissaBits = 11;

		// Token: 0x04005E6F RID: 24175
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04005E70 RID: 24176
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04005E71 RID: 24177
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04005E72 RID: 24178
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04005E73 RID: 24179
		public const int ExponentRadix = 2;

		// Token: 0x04005E74 RID: 24180
		public const int AdditionRounding = 1;

		// Token: 0x04005E75 RID: 24181
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04005E76 RID: 24182
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04005E77 RID: 24183
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x04005E78 RID: 24184
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x04005E79 RID: 24185
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x04005E7A RID: 24186
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
