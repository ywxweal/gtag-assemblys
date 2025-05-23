using System;

// Token: 0x02000780 RID: 1920
public struct TimeSince
{
	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06003008 RID: 12296 RVA: 0x000EDF58 File Offset: 0x000EC158
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
	// (get) Token: 0x06003009 RID: 12297 RVA: 0x000EDF95 File Offset: 0x000EC195
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x0600300A RID: 12298 RVA: 0x000EDF9E File Offset: 0x000EC19E
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x0600300B RID: 12299 RVA: 0x000EDFA7 File Offset: 0x000EC1A7
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x0600300C RID: 12300 RVA: 0x000EDFB0 File Offset: 0x000EC1B0
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x0600300D RID: 12301 RVA: 0x000EDFB9 File Offset: 0x000EC1B9
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x000EDFC6 File Offset: 0x000EC1C6
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000EDFD0 File Offset: 0x000EC1D0
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000EDFF4 File Offset: 0x000EC1F4
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000EE024 File Offset: 0x000EC224
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000EE048 File Offset: 0x000EC248
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000EE06C File Offset: 0x000EC26C
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000EE090 File Offset: 0x000EC290
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x000EE0B6 File Offset: 0x000EC2B6
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x000EE0C4 File Offset: 0x000EC2C4
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000EE0D2 File Offset: 0x000EC2D2
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x000EE0E0 File Offset: 0x000EC2E0
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000EE0EE File Offset: 0x000EC2EE
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x000EE0FC File Offset: 0x000EC2FC
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x000EE10A File Offset: 0x000EC30A
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x000EE117 File Offset: 0x000EC317
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

	// Token: 0x0600301D RID: 12317 RVA: 0x000EE13B File Offset: 0x000EC33B
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

	// Token: 0x0600301E RID: 12318 RVA: 0x000EE15F File Offset: 0x000EC35F
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

	// Token: 0x0600301F RID: 12319 RVA: 0x000EE183 File Offset: 0x000EC383
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

	// Token: 0x06003020 RID: 12320 RVA: 0x000EE1A7 File Offset: 0x000EC3A7
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

	// Token: 0x06003021 RID: 12321 RVA: 0x000EE1CB File Offset: 0x000EC3CB
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

	// Token: 0x06003022 RID: 12322 RVA: 0x000EE1F4 File Offset: 0x000EC3F4
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x000EE216 File Offset: 0x000EC416
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000EE223 File Offset: 0x000EC423
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x000EE22F File Offset: 0x000EC42F
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x000EE238 File Offset: 0x000EC438
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x000EE241 File Offset: 0x000EC441
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x000EE24A File Offset: 0x000EC44A
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x000EE253 File Offset: 0x000EC453
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x000EE25C File Offset: 0x000EC45C
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x000EE265 File Offset: 0x000EC465
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x000EE26D File Offset: 0x000EC46D
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x000EE275 File Offset: 0x000EC475
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x000EE27D File Offset: 0x000EC47D
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x000EE285 File Offset: 0x000EC485
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x000EE28D File Offset: 0x000EC48D
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x000EE295 File Offset: 0x000EC495
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x04003648 RID: 13896
	private DateTime _dt;

	// Token: 0x04003649 RID: 13897
	private const double INT32_MAX = 2147483647.0;
}
