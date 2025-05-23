using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200077B RID: 1915
[Serializable]
public struct SRand
{
	// Token: 0x06002FB9 RID: 12217 RVA: 0x000ED53B File Offset: 0x000EB73B
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x000ED53B File Offset: 0x000EB73B
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000ED550 File Offset: 0x000EB750
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x000ED56A File Offset: 0x000EB76A
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x000ED584 File Offset: 0x000EB784
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x000ED5B6 File Offset: 0x000EB7B6
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x000ED5E7 File Offset: 0x000EB7E7
	public double NextDouble()
	{
		return this.NextState() % 268435457U / 268435456.0;
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x000ED601 File Offset: 0x000EB801
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x000ED624 File Offset: 0x000EB824
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

	// Token: 0x06002FC2 RID: 12226 RVA: 0x000ED64F File Offset: 0x000EB84F
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x06002FC3 RID: 12227 RVA: 0x000ED658 File Offset: 0x000EB858
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x000ED663 File Offset: 0x000EB863
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x000ED670 File Offset: 0x000EB870
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x000ED67D File Offset: 0x000EB87D
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x000ED67D File Offset: 0x000EB87D
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x000ED685 File Offset: 0x000EB885
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x000ED698 File Offset: 0x000EB898
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x000ED6B8 File Offset: 0x000EB8B8
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

	// Token: 0x06002FCB RID: 12235 RVA: 0x000ED6E8 File Offset: 0x000EB8E8
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

	// Token: 0x06002FCC RID: 12236 RVA: 0x000ED750 File Offset: 0x000EB950
	public Color32 NextColor32()
	{
		byte b = (byte)this.NextInt(256);
		byte b2 = (byte)this.NextInt(256);
		byte b3 = (byte)this.NextInt(256);
		return new Color32(b, b2, b3, byte.MaxValue);
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x000ED790 File Offset: 0x000EB990
	public Color NextColor()
	{
		float num = this.NextFloat();
		float num2 = this.NextFloat();
		float num3 = this.NextFloat();
		return new Color(num, num2, num3, 1f);
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000ED7C0 File Offset: 0x000EB9C0
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

	// Token: 0x06002FCF RID: 12239 RVA: 0x000ED810 File Offset: 0x000EBA10
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

	// Token: 0x06002FD0 RID: 12240 RVA: 0x000ED868 File Offset: 0x000EBA68
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x000ED53B File Offset: 0x000EB73B
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x000ED53B File Offset: 0x000EB73B
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x000ED550 File Offset: 0x000EB750
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x000ED56A File Offset: 0x000EB76A
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x000ED584 File Offset: 0x000EB784
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x000ED5B6 File Offset: 0x000EB7B6
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x000ED878 File Offset: 0x000EBA78
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000ED8A0 File Offset: 0x000EBAA0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = ((x >> 16) ^ x) * 73244475U;
		x = ((x >> 16) ^ x) * 73244475U;
		x = (x >> 16) ^ x;
		return x;
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000ED8C7 File Offset: 0x000EBAC7
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x000ED8DC File Offset: 0x000EBADC
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[] { "SRand", "_seed", this._seed, "_state", this._state });
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x000ED92D File Offset: 0x000EBB2D
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x000ED939 File Offset: 0x000EBB39
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x000ED941 File Offset: 0x000EBB41
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x000ED949 File Offset: 0x000EBB49
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x000ED951 File Offset: 0x000EBB51
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x000ED959 File Offset: 0x000EBB59
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x000ED961 File Offset: 0x000EBB61
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x04003632 RID: 13874
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x04003633 RID: 13875
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x04003634 RID: 13876
	[SerializeField]
	private uint _seed;

	// Token: 0x04003635 RID: 13877
	[SerializeField]
	private uint _state;
}
