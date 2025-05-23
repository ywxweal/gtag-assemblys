using System;
using UnityEngine;

// Token: 0x02000781 RID: 1921
[Serializable]
public class VelocityHelper
{
	// Token: 0x06003032 RID: 12338 RVA: 0x000EE29D File Offset: 0x000EC49D
	public VelocityHelper(int historySize = 12)
	{
		this._size = historySize;
		this._samples = new float[historySize * 4];
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x000EE2BC File Offset: 0x000EC4BC
	public void SamplePosition(Transform target, float dt)
	{
		Vector3 position = target.position;
		if (!this._initialized)
		{
			this._InitSamples(position, dt);
		}
		this._SetSample(this._latest, position, dt);
		this._latest = (this._latest + 1) % this._size;
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x000EE304 File Offset: 0x000EC504
	private void _InitSamples(Vector3 position, float dt)
	{
		for (int i = 0; i < this._size; i++)
		{
			this._SetSample(i, position, dt);
		}
		this._initialized = true;
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x000EE332 File Offset: 0x000EC532
	private void _SetSample(int i, Vector3 position, float dt)
	{
		this._samples[i] = position.x;
		this._samples[i + 1] = position.y;
		this._samples[i + 2] = position.z;
		this._samples[i + 3] = dt;
	}

	// Token: 0x0400364A RID: 13898
	private float[] _samples;

	// Token: 0x0400364B RID: 13899
	private int _latest;

	// Token: 0x0400364C RID: 13900
	private int _size;

	// Token: 0x0400364D RID: 13901
	private bool _initialized;
}
