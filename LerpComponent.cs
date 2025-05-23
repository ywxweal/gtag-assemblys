using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000768 RID: 1896
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x06002F42 RID: 12098 RVA: 0x000EBDF0 File Offset: 0x000E9FF0
	// (set) Token: 0x06002F43 RID: 12099 RVA: 0x000EBDF8 File Offset: 0x000E9FF8
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x06002F44 RID: 12100 RVA: 0x000EBE33 File Offset: 0x000EA033
	// (set) Token: 0x06002F45 RID: 12101 RVA: 0x000EBE3B File Offset: 0x000EA03B
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x06002F46 RID: 12102 RVA: 0x00047642 File Offset: 0x00045842
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002F47 RID: 12103
	protected abstract void OnLerp(float t);

	// Token: 0x06002F48 RID: 12104 RVA: 0x000EBE53 File Offset: 0x000EA053
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x000EBE64 File Offset: 0x000EA064
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x000EBE8F File Offset: 0x000EA08F
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x040035A7 RID: 13735
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x040035A8 RID: 13736
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x040035A9 RID: 13737
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x040035AA RID: 13738
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x040035AB RID: 13739
	[NonSerialized]
	private bool _previewing;

	// Token: 0x040035AC RID: 13740
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x040035AD RID: 13741
	[NonSerialized]
	private bool _rendering;

	// Token: 0x040035AE RID: 13742
	[NonSerialized]
	private int _lastState;

	// Token: 0x040035AF RID: 13743
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x040035B0 RID: 13744
	[NonSerialized]
	private float _prevLerpTo;
}
