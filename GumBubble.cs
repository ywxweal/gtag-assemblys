using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200045C RID: 1116
public class GumBubble : LerpComponent
{
	// Token: 0x06001B6C RID: 7020 RVA: 0x00086CF9 File Offset: 0x00084EF9
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x00086D0E File Offset: 0x00084F0E
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x00086D1C File Offset: 0x00084F1C
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x00086D3C File Offset: 0x00084F3C
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxInflate, 1f);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x00086DC0 File Offset: 0x00084FC0
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxPop, 1f);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x00086E44 File Offset: 0x00085044
	private void Update()
	{
		float num = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, num);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num2 = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num2)
		{
			this.Pop();
		}
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x00086ED8 File Offset: 0x000850D8
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError<string>("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x04001E60 RID: 7776
	public Transform target;

	// Token: 0x04001E61 RID: 7777
	public Vector3 targetScale = Vector3.one;

	// Token: 0x04001E62 RID: 7778
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x04001E63 RID: 7779
	public AudioSource audioSource;

	// Token: 0x04001E64 RID: 7780
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04001E65 RID: 7781
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x04001E66 RID: 7782
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x04001E67 RID: 7783
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x04001E68 RID: 7784
	[SerializeField]
	private bool _animating;

	// Token: 0x04001E69 RID: 7785
	public UnityEvent onPop;

	// Token: 0x04001E6A RID: 7786
	public UnityEvent onInflate;

	// Token: 0x04001E6B RID: 7787
	[NonSerialized]
	private bool _done;

	// Token: 0x04001E6C RID: 7788
	[NonSerialized]
	private TimeSince _sinceInflate;
}
