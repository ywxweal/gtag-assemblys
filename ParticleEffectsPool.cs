using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class ParticleEffectsPool : MonoBehaviour
{
	// Token: 0x060009ED RID: 2541 RVA: 0x00034A90 File Offset: 0x00032C90
	public void Awake()
	{
		this.OnPoolAwake();
		this.Setup();
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnPoolAwake()
	{
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00034AA0 File Offset: 0x00032CA0
	private void Setup()
	{
		this.MoveToSceneWorldRoot();
		this._pools = new RingBuffer<ParticleEffect>[this.effects.Length];
		this._effectToPool = new Dictionary<long, int>(this.effects.Length);
		for (int i = 0; i < this.effects.Length; i++)
		{
			ParticleEffect particleEffect = this.effects[i];
			this._pools[i] = this.InitPoolForPrefab(i, this.effects[i]);
			this._effectToPool.TryAdd(particleEffect.effectID, i);
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00034B1F File Offset: 0x00032D1F
	private void MoveToSceneWorldRoot()
	{
		Transform transform = base.transform;
		transform.parent = null;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00034B50 File Offset: 0x00032D50
	private RingBuffer<ParticleEffect> InitPoolForPrefab(int index, ParticleEffect prefab)
	{
		RingBuffer<ParticleEffect> ringBuffer = new RingBuffer<ParticleEffect>(this.poolSize);
		string text = prefab.name.Trim();
		for (int i = 0; i < this.poolSize; i++)
		{
			ParticleEffect particleEffect = Object.Instantiate<ParticleEffect>(prefab, base.transform);
			particleEffect.gameObject.SetActive(false);
			particleEffect.pool = this;
			particleEffect.poolIndex = index;
			particleEffect.name = ZString.Concat<string, string, int>(text, "*", i);
			ringBuffer.Push(particleEffect);
		}
		return ringBuffer;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00034BC8 File Offset: 0x00032DC8
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos)
	{
		this.PlayEffect(effect.effectID, worldPos);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00034BD7 File Offset: 0x00032DD7
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos, float delay)
	{
		this.PlayEffect(effect.effectID, worldPos, delay);
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00034BE7 File Offset: 0x00032DE7
	public void PlayEffect(long effectID, Vector3 worldPos)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00034BF7 File Offset: 0x00032DF7
	public void PlayEffect(long effectID, Vector3 worldPos, float delay)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos, delay);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00034C08 File Offset: 0x00032E08
	public void PlayEffect(int index, Vector3 worldPos)
	{
		if (index == -1)
		{
			return;
		}
		ParticleEffect particleEffect;
		if (!this._pools[index].TryPop(out particleEffect))
		{
			return;
		}
		particleEffect.transform.localPosition = worldPos;
		particleEffect.Play();
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00034C3E File Offset: 0x00032E3E
	public void PlayEffect(int index, Vector3 worldPos, float delay)
	{
		if (delay.Approx(0f, 1E-06f))
		{
			this.PlayEffect(index, worldPos);
			return;
		}
		base.StartCoroutine(this.PlayDelayed(index, worldPos, delay));
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00034C6B File Offset: 0x00032E6B
	private IEnumerator PlayDelayed(int index, Vector3 worldPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayEffect(index, worldPos);
		yield break;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00034C8F File Offset: 0x00032E8F
	public void Return(ParticleEffect effect)
	{
		this._pools[effect.poolIndex].Push(effect);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00034CA8 File Offset: 0x00032EA8
	public int GetPoolIndex(long effectID)
	{
		int num;
		if (this._effectToPool.TryGetValue(effectID, out num))
		{
			return num;
		}
		return -1;
	}

	// Token: 0x04000C17 RID: 3095
	public ParticleEffect[] effects = new ParticleEffect[0];

	// Token: 0x04000C18 RID: 3096
	[Space]
	public int poolSize = 10;

	// Token: 0x04000C19 RID: 3097
	[Space]
	private RingBuffer<ParticleEffect>[] _pools = new RingBuffer<ParticleEffect>[0];

	// Token: 0x04000C1A RID: 3098
	private Dictionary<long, int> _effectToPool = new Dictionary<long, int>();
}
