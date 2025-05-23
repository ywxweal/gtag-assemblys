using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200077B RID: 1915
[Serializable]
public struct SRand
{
	// Token: 0x06002FB8 RID: 12216 RVA: 0x000ED497 File Offset: 0x000EB697
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x000ED497 File Offset: 0x000EB697
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x000ED4AC File Offset: 0x000EB6AC
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000ED4C6 File Offset: 0x000EB6C6
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x000ED4E0 File Offset: 0x000EB6E0
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x000ED512 File Offset: 0x000EB712
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x000ED543 File Offset: 0x000EB743
	public double NextDouble()
	{
		return this.NextState() % 268435457U / 268435456.0;
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x000ED55D File Offset: 0x000EB75D
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x000ED580 File Offset: 0x000EB780
	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x000ED5AB File Offset: 0x000EB7AB
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x06002FC2 RID: 12226 RVA: 0x000ED5B4 File Offset: 0x000EB7B4
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x06002FC3 RID: 12227 RVA: 0x000ED5BF File Offset: 0x000EB7BF
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x000ED5CC File Offset: 0x000EB7CC
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x000ED5D9 File Offset: 0x000EB7D9
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x000ED5D9 File Offset: 0x000EB7D9
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x000ED5E1 File Offset: 0x000EB7E1
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x000ED5F4 File Offset: 0x000EB7F4
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x000ED614 File Offset: 0x000EB814
	public int NextIntWithExclusion(int min, int max, int exclude)
	{
		int num = max - min - 1;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 1 + this.NextInt(num);
		if (num2 > exclude)
		{
			return num2;
		}
		return num2 - 1;
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x000ED644 File Offset: 0x000EB844
	public int NextIntWithExclusion2(int min, int max, int exclude, int exclude2)
	{
		if (exclude == exclude2)
		{
			return this.NextIntWithExclusion(min, max, exclude);
		}
		int num = max - min - 2;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 2 + this.NextInt(num);
		int num3;
		int num4;
		if (exclude >= exclude2)
		{
			num3 = exclude2 + 1;
			num4 = exclude;
		}
		else
		{
			num3 = exclude + 1;
			num4 = exclude2;
		}
		if (num2 <= num3)
		{
			return num2 - 2;
		}
		if (num2 <= num4)
		{
			return num2 - 1;
		}
		return num2;
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x000ED6AC File Offset: 0x000EB8AC
	public Color32 NextColor32()
	{
		byte b = (byte)this.NextInt(256);
		byte b2 = (byte)this.NextInt(256);
		byte b3 = (byte)this.NextInt(256);
		return new Color32(b, b2, b3, byte.MaxValue);
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x000ED6EC File Offset: 0x000EB8EC
	public Color NextColor()
	{
		float num = this.NextFloat();
		float num2 = this.NextFloat();
		float num3 = this.NextFloat();
		return new Color(num, num2, num3, 1f);
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x000ED71C File Offset: 0x000EB91C
	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = array[num];
			T t2 = array[i];
			array[num2] = t;
			array[num3] = t2;
		}
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000ED76C File Offset: 0x000EB96C
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = list[num];
			T t2 = list[i];
			list[num2] = t;
			list[num3] = t2;
		}
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x000ED7C4 File Offset: 0x000EB9C4
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x000ED497 File Offset: 0x000EB697
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x000ED497 File Offset: 0x000EB697
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x000ED4AC File Offset: 0x000EB6AC
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x000ED4C6 File Offset: 0x000EB6C6
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x000ED4E0 File Offset: 0x000EB6E0
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x000ED512 File Offset: 0x000EB712
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x000ED7D4 File Offset: 0x000EB9D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x000ED7FC File Offset: 0x000EB9FC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = ((x >> 16) ^ x) * 73244475U;
		x = ((x >> 16) ^ x) * 73244475U;
		x = (x >> 16) ^ x;
		return x;
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000ED823 File Offset: 0x000EBA23
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000ED838 File Offset: 0x000EBA38
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[] { "SRand", "_seed", this._seed, "_state", this._state });
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x000ED889 File Offset: 0x000EBA89
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x000ED895 File Offset: 0x000EBA95
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x000ED89D File Offset: 0x000EBA9D
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x000ED8A5 File Offset: 0x000EBAA5
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x000ED8AD File Offset: 0x000EBAAD
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x000ED8B5 File Offset: 0x000EBAB5
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x000ED8BD File Offset: 0x000EBABD
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04003630 RID: 13872
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x04003631 RID: 13873
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x04003632 RID: 13874
	[SerializeField]
	private uint _seed;

	// Token: 0x04003633 RID: 13875
	[SerializeField]
	private uint _state;
}
