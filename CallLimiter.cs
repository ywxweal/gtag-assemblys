using System;
using UnityEngine;

// Token: 0x02000924 RID: 2340
[Serializable]
public class CallLimiter
{
	// Token: 0x06003902 RID: 14594 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimiter()
	{
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x00112A98 File Offset: 0x00110C98
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = (double)latencyMax;
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x00112AE8 File Offset: 0x00110CE8
	public bool CheckCallServerTime(double time)
	{
		double simTime = NetworkSystem.Instance.SimTime;
		double num = this.maxLatency;
		double num2 = 4294967.295 - this.maxLatency;
		double num3;
		if (simTime > num || time < num)
		{
			if (time > simTime)
			{
				return false;
			}
			num3 = simTime - time;
		}
		else
		{
			double num4 = num2 + simTime;
			if (time > simTime && time < num4)
			{
				return false;
			}
			num3 = simTime + (4294967.295 - time);
		}
		if (num3 > this.maxLatency)
		{
			return false;
		}
		int num5 = ((this.oldTimeIndex > 0) ? (this.oldTimeIndex - 1) : (this.callHistoryLength - 1));
		double num6 = (double)this.callTimeHistory[num5];
		if (num6 > num2 && time < num6)
		{
			this.Reset();
		}
		else if (time < num6)
		{
			return false;
		}
		return this.CheckCallTime((float)time);
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x00112BA0 File Offset: 0x00110DA0
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x00112C08 File Offset: 0x00110E08
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x04003E31 RID: 15921
	protected const double k_serverMaxTime = 4294967.295;

	// Token: 0x04003E32 RID: 15922
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x04003E33 RID: 15923
	[Space]
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x04003E34 RID: 15924
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x04003E35 RID: 15925
	[SerializeField]
	protected double maxLatency;

	// Token: 0x04003E36 RID: 15926
	private int oldTimeIndex;

	// Token: 0x04003E37 RID: 15927
	protected bool blockCall;

	// Token: 0x04003E38 RID: 15928
	protected float blockStartTime;
}
