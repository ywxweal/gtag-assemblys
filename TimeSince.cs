using System;

// Token: 0x02000780 RID: 1920
public struct TimeSince
{
	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06003009 RID: 12297 RVA: 0x000EDFFC File Offset: 0x000EC1FC
	public double secondsElapsed
	{
		get
		{
			double totalSeconds = (DateTime.UtcNow - this._dt).TotalSeconds;
			if (totalSeconds <= 2147483647.0)
			{
				return totalSeconds;
			}
			return 2147483647.0;
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x0600300A RID: 12298 RVA: 0x000EE039 File Offset: 0x000EC239
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x0600300B RID: 12299 RVA: 0x000EE042 File Offset: 0x000EC242
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x0600300C RID: 12300 RVA: 0x000EE04B File Offset: 0x000EC24B
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x0600300D RID: 12301 RVA: 0x000EE054 File Offset: 0x000EC254
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x0600300E RID: 12302 RVA: 0x000EE05D File Offset: 0x000EC25D
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000EE06A File Offset: 0x000EC26A
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000EE074 File Offset: 0x000EC274
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000EE098 File Offset: 0x000EC298
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000EE0C8 File Offset: 0x000EC2C8
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000EE0EC File Offset: 0x000EC2EC
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000EE110 File Offset: 0x000EC310
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x000EE134 File Offset: 0x000EC334
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x000EE15A File Offset: 0x000EC35A
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000EE168 File Offset: 0x000EC368
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x000EE176 File Offset: 0x000EC376
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000EE184 File Offset: 0x000EC384
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x000EE192 File Offset: 0x000EC392
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x000EE1A0 File Offset: 0x000EC3A0
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x000EE1AE File Offset: 0x000EC3AE
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x000EE1BB File Offset: 0x000EC3BB
	public bool HasElapsed(int seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedInt >= seconds;
		}
		if (this.secondsElapsedInt < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x000EE1DF File Offset: 0x000EC3DF
	public bool HasElapsed(uint seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedUint >= seconds;
		}
		if (this.secondsElapsedUint < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x000EE203 File Offset: 0x000EC403
	public bool HasElapsed(float seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedFloat >= seconds;
		}
		if (this.secondsElapsedFloat < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x000EE227 File Offset: 0x000EC427
	public bool HasElapsed(double seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsed >= seconds;
		}
		if (this.secondsElapsed < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x000EE24B File Offset: 0x000EC44B
	public bool HasElapsed(long seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedLong >= seconds;
		}
		if (this.secondsElapsedLong < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x000EE26F File Offset: 0x000EC46F
	public bool HasElapsed(TimeSpan seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedSpan >= seconds;
		}
		if (this.secondsElapsedSpan < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x000EE298 File Offset: 0x000EC498
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000EE2BA File Offset: 0x000EC4BA
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x000EE2C7 File Offset: 0x000EC4C7
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x000EE2D3 File Offset: 0x000EC4D3
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x000EE2DC File Offset: 0x000EC4DC
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x000EE2E5 File Offset: 0x000EC4E5
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x000EE2EE File Offset: 0x000EC4EE
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x000EE2F7 File Offset: 0x000EC4F7
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x000EE300 File Offset: 0x000EC500
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x000EE309 File Offset: 0x000EC509
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x000EE311 File Offset: 0x000EC511
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x000EE319 File Offset: 0x000EC519
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x000EE321 File Offset: 0x000EC521
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x000EE329 File Offset: 0x000EC529
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x000EE331 File Offset: 0x000EC531
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x000EE339 File Offset: 0x000EC539
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x0400364A RID: 13898
	private DateTime _dt;

	// Token: 0x0400364B RID: 13899
	private const double INT32_MAX = 2147483647.0;
}
