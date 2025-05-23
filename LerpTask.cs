using System;
using UnityEngine;

// Token: 0x02000673 RID: 1651
public class LerpTask<T>
{
	// Token: 0x0600293F RID: 10559 RVA: 0x000CD026 File Offset: 0x000CB226
	public void Reset()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 0f);
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x000CD056 File Offset: 0x000CB256
	public void Start(T from, T to, float duration)
	{
		this.lerpFrom = from;
		this.lerpTo = to;
		this.duration = duration;
		this.elapsed = 0f;
		this.active = true;
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x000CD080 File Offset: 0x000CB280
	public void Finish()
	{
		this.onLerp(this.lerpFrom, this.lerpTo, 1f);
		Action action = this.onLerpEnd;
		if (action != null)
		{
			action();
		}
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x000CD0CC File Offset: 0x000CB2CC
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.elapsed < this.duration)
		{
			float num = ((this.elapsed + deltaTime >= this.duration) ? 1f : (this.elapsed / this.duration));
			this.onLerp(this.lerpFrom, this.lerpTo, num);
			this.elapsed += deltaTime;
			return;
		}
		this.Finish();
	}

	// Token: 0x04002E3E RID: 11838
	public float elapsed;

	// Token: 0x04002E3F RID: 11839
	public float duration;

	// Token: 0x04002E40 RID: 11840
	public T lerpFrom;

	// Token: 0x04002E41 RID: 11841
	public T lerpTo;

	// Token: 0x04002E42 RID: 11842
	public Action<T, T, float> onLerp;

	// Token: 0x04002E43 RID: 11843
	public Action onLerpEnd;

	// Token: 0x04002E44 RID: 11844
	public bool active;
}
