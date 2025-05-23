using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200096E RID: 2414
public class AverageVector3
{
	// Token: 0x06003A36 RID: 14902 RVA: 0x0011706D File Offset: 0x0011526D
	public AverageVector3(float averagingWindow = 0.1f)
	{
		this.timeWindow = averagingWindow;
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x00117094 File Offset: 0x00115294
	public void AddSample(Vector3 sample, float time)
	{
		this.samples.Add(new AverageVector3.Sample
		{
			timeStamp = time,
			value = sample
		});
		this.RefreshSamples();
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x001170CC File Offset: 0x001152CC
	public Vector3 GetAverage()
	{
		this.RefreshSamples();
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.samples.Count; i++)
		{
			vector += this.samples[i].value;
		}
		return vector / (float)this.samples.Count;
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x00117127 File Offset: 0x00115327
	public void Clear()
	{
		this.samples.Clear();
	}

	// Token: 0x06003A3A RID: 14906 RVA: 0x00117134 File Offset: 0x00115334
	private void RefreshSamples()
	{
		float num = Time.time - this.timeWindow;
		for (int i = this.samples.Count - 1; i >= 0; i--)
		{
			if (this.samples[i].timeStamp < num)
			{
				this.samples.RemoveAt(i);
			}
		}
	}

	// Token: 0x04003F3B RID: 16187
	private List<AverageVector3.Sample> samples = new List<AverageVector3.Sample>();

	// Token: 0x04003F3C RID: 16188
	private float timeWindow = 0.1f;

	// Token: 0x0200096F RID: 2415
	public struct Sample
	{
		// Token: 0x04003F3D RID: 16189
		public float timeStamp;

		// Token: 0x04003F3E RID: 16190
		public Vector3 value;
	}
}
