using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200077E RID: 1918
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x06002FFB RID: 12283 RVA: 0x000EDCE0 File Offset: 0x000EBEE0
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1;
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x000EDD9C File Offset: 0x000EBF9C
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x000EDDAC File Offset: 0x000EBFAC
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x000EDDCC File Offset: 0x000EBFCC
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x000EDDD4 File Offset: 0x000EBFD4
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x000EDE04 File Offset: 0x000EC004
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x000EDE6F File Offset: 0x000EC06F
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x000EDE81 File Offset: 0x000EC081
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x000EDE92 File Offset: 0x000EC092
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04003637 RID: 13879
	public ParticleSystem target;

	// Token: 0x04003638 RID: 13880
	public ParticleSystem subEmitter;

	// Token: 0x04003639 RID: 13881
	public int subEmitterIndex;

	// Token: 0x0400363A RID: 13882
	public UnityEvent onSubEmit;

	// Token: 0x0400363B RID: 13883
	public float intervalScale = 1f;

	// Token: 0x0400363C RID: 13884
	public float interval;

	// Token: 0x0400363D RID: 13885
	[NonSerialized]
	private bool _canListen;

	// Token: 0x0400363E RID: 13886
	[NonSerialized]
	private bool _listening;

	// Token: 0x0400363F RID: 13887
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x04003640 RID: 13888
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
