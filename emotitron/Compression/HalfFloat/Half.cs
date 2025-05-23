using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000E15 RID: 3605
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06005A2A RID: 23082 RVA: 0x001B7E1E File Offset: 0x001B601E
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06005A2B RID: 23083 RVA: 0x001B7E2C File Offset: 0x001B602C
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x001B7E34 File Offset: 0x001B6034
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06005A2D RID: 23085 RVA: 0x001B7E70 File Offset: 0x001B6070
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x001B7EA4 File Offset: 0x001B60A4
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x001B7EC0 File Offset: 0x001B60C0
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06005A30 RID: 23088 RVA: 0x001B7ECD File Offset: 0x001B60CD
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x001B7EDA File Offset: 0x001B60DA
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x001B7EE7 File Offset: 0x001B60E7
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x001B7EF9 File Offset: 0x001B60F9
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x001B7F0B File Offset: 0x001B610B
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06005A35 RID: 23093 RVA: 0x001B7F20 File Offset: 0x001B6120
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x001B7F35 File Offset: 0x001B6135
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005A37 RID: 23095 RVA: 0x001B7F3F File Offset: 0x001B613F
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06005A38 RID: 23096 RVA: 0x001B7F4C File Offset: 0x001B614C
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06005A39 RID: 23097 RVA: 0x001B7F54 File Offset: 0x001B6154
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x001B7F64 File Offset: 0x001B6164
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x001B7F94 File Offset: 0x001B6194
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x001B7FDC File Offset: 0x001B61DC
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x001B8008 File Offset: 0x001B6208
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x001B8041 File Offset: 0x001B6241
		public override int GetHashCode()
		{
			return (int)((this.value * 3 / 2) ^ this.value);
		}

		// Token: 0x06005A3F RID: 23103 RVA: 0x001B8054 File Offset: 0x001B6254
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

		// Token: 0x06005A40 RID: 23104 RVA: 0x001B80AC File Offset: 0x001B62AC
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

		// Token: 0x06005A41 RID: 23105 RVA: 0x001B8120 File Offset: 0x001B6320
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06005A42 RID: 23106 RVA: 0x001B8130 File Offset: 0x001B6330
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x001B8140 File Offset: 0x001B6340
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06005A44 RID: 23108 RVA: 0x001B8172 File Offset: 0x001B6372
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x001B8183 File Offset: 0x001B6383
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x001B8195 File Offset: 0x001B6395
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06005A47 RID: 23111 RVA: 0x001B81A7 File Offset: 0x001B63A7
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06005A48 RID: 23112 RVA: 0x001B81B3 File Offset: 0x001B63B3
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06005A49 RID: 23113 RVA: 0x001B81BF File Offset: 0x001B63BF
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06005A4A RID: 23114 RVA: 0x001B81D1 File Offset: 0x001B63D1
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x001B81E3 File Offset: 0x001B63E3
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06005A4C RID: 23116 RVA: 0x001B81F5 File Offset: 0x001B63F5
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06005A4D RID: 23117 RVA: 0x001B8207 File Offset: 0x001B6407
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x001B8219 File Offset: 0x001B6419
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06005A4F RID: 23119 RVA: 0x001B822B File Offset: 0x001B642B
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06005A50 RID: 23120 RVA: 0x001B8238 File Offset: 0x001B6438
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x001B8252 File Offset: 0x001B6452
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06005A52 RID: 23122 RVA: 0x001B8264 File Offset: 0x001B6464
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06005A53 RID: 23123 RVA: 0x001B8276 File Offset: 0x001B6476
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04005E6D RID: 24173
		private ushort value;

		// Token: 0x04005E6E RID: 24174
		public const int PrecisionDigits = 3;

		// Token: 0x04005E6F RID: 24175
		public const int MantissaBits = 11;

		// Token: 0x04005E70 RID: 24176
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04005E71 RID: 24177
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04005E72 RID: 24178
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04005E73 RID: 24179
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04005E74 RID: 24180
		public const int ExponentRadix = 2;

		// Token: 0x04005E75 RID: 24181
		public const int AdditionRounding = 1;

		// Token: 0x04005E76 RID: 24182
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04005E77 RID: 24183
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04005E78 RID: 24184
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x04005E79 RID: 24185
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x04005E7A RID: 24186
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x04005E7B RID: 24187
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
