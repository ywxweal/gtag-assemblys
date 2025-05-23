using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x02000764 RID: 1892
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x06002F12 RID: 12050 RVA: 0x000EB8A0 File Offset: 0x000E9AA0
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x000EB8F4 File Offset: 0x000E9AF4
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x000EB948 File Offset: 0x000E9B48
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x000EB99C File Offset: 0x000E9B9C
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x000EB9F0 File Offset: 0x000E9BF0
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x000EBA5C File Offset: 0x000E9C5C
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x000EBAD9 File Offset: 0x000E9CD9
	[return: TupleElementNames(new string[] { "l1", "l2" })]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x000EBAEC File Offset: 0x000E9CEC
	[return: TupleElementNames(new string[] { "i1", "i2", "i3", "i4" })]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x000EBB0B File Offset: 0x000E9D0B
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x000EBB18 File Offset: 0x000E9D18
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x06002F1C RID: 12060 RVA: 0x000EBB38 File Offset: 0x000E9D38
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06002F1D RID: 12061 RVA: 0x000EBB46 File Offset: 0x000E9D46
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x000EBB54 File Offset: 0x000E9D54
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid guid = (Guid)obj;
			return this.Equals(guid);
		}
		if (obj is Hash128)
		{
			Hash128 hash = (Hash128)obj;
			return this.Equals(hash);
		}
		return false;
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x000EBBA7 File Offset: 0x000E9DA7
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x000EBBBA File Offset: 0x000E9DBA
	public override int GetHashCode()
	{
		return StaticHash.Compute(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x000EBBDC File Offset: 0x000E9DDC
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x000EBC14 File Offset: 0x000E9E14
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid guid = (Guid)obj;
			return this.guid.CompareTo(guid);
		}
		if (obj is Hash128)
		{
			Hash128 hash = (Hash128)obj;
			return this.h128.CompareTo(hash);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x000EBC7A File Offset: 0x000E9E7A
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x000EBC88 File Offset: 0x000E9E88
	public static Id128 ComputeMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 id;
		using (MD5 md = MD5.Create())
		{
			id = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return id;
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x000EBCE4 File Offset: 0x000E9EE4
	public static Id128 ComputeSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x000EBCFF File Offset: 0x000E9EFF
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x000EBD09 File Offset: 0x000E9F09
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x000EBD16 File Offset: 0x000E9F16
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x000EBD20 File Offset: 0x000E9F20
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x000EBD2D File Offset: 0x000E9F2D
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x000EBD3C File Offset: 0x000E9F3C
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x000EBD4E File Offset: 0x000E9F4E
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x000EBD58 File Offset: 0x000E9F58
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x000EBD65 File Offset: 0x000E9F65
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x000EBD74 File Offset: 0x000E9F74
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x000EBD86 File Offset: 0x000E9F86
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x000EBD93 File Offset: 0x000E9F93
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x000EBDA0 File Offset: 0x000E9FA0
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x000EBDB0 File Offset: 0x000E9FB0
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x000EBDC0 File Offset: 0x000E9FC0
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x000EBDC8 File Offset: 0x000E9FC8
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x000EBDD0 File Offset: 0x000E9FD0
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x000EBDD8 File Offset: 0x000E9FD8
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x000EBDE0 File Offset: 0x000E9FE0
	public static explicit operator Id128(string s)
	{
		return Id128.ComputeMD5(s);
	}

	// Token: 0x0400359F RID: 13727
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x040035A0 RID: 13728
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x040035A1 RID: 13729
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x040035A2 RID: 13730
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x040035A3 RID: 13731
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x040035A4 RID: 13732
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x040035A5 RID: 13733
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x040035A6 RID: 13734
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x040035A7 RID: 13735
	public static readonly Id128 Empty;
}
