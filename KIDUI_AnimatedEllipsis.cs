using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000819 RID: 2073
public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	// Token: 0x060032C9 RID: 13001 RVA: 0x000FA68E File Offset: 0x000F888E
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x000FA69F File Offset: 0x000F889F
	private void OnDisable()
	{
		this.StopAnimation();
	}

	// Token: 0x060032CC RID: 13004 RVA: 0x000FA6A8 File Offset: 0x000F88A8
	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x000FA77E File Offset: 0x000F897E
	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float num2 = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * num2;
			}
			int num3 = currIndex;
			currIndex = num3 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x000FA78D File Offset: 0x000F898D
	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float num = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num2 = this.LerpLoop(this._startingScale, this._endScale, time, num, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num2, num2, num2);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060032CF RID: 13007 RVA: 0x000FA79C File Offset: 0x000F899C
	public async Task StartAnimation()
	{
		if (this._ellipsisObjects == null)
		{
			this.SetupEllipsis();
		}
		if (this._animationCoroutine != null)
		{
			Debug.LogWarningFormat("[KID::UI::ELLIPSIS] Animation is already running.", Array.Empty<object>());
			await this.StopAnimation();
		}
		for (int i = 0; i < this._ellipsisCount; i++)
		{
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(this._ellipsisObjects[i].Item2, this._ellipsisObjects[i].Item2, this._ellipsisObjects[i].Item2);
		}
		this._ellipsisRoot.SetActive(true);
		this._runAnimation = true;
		if (this._shouldLerp)
		{
			this._animationCoroutine = base.StartCoroutine(this.EllipsisAnimation2());
		}
		else
		{
			this._animationCoroutine = base.StartCoroutine(this.EllipsisAnimation());
		}
	}

	// Token: 0x060032D0 RID: 13008 RVA: 0x000FA7E0 File Offset: 0x000F89E0
	public async Task StopAnimation()
	{
		this._runAnimation = false;
		base.StopAllCoroutines();
		await Task.Delay(100);
		this._animationCoroutine = null;
		this._ellipsisRoot.SetActive(false);
	}

	// Token: 0x060032D1 RID: 13009 RVA: 0x000FA824 File Offset: 0x000F8A24
	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float num = (offsetTime - time) % duration / duration;
		float num2 = this._ellipsisAnimationCurve.Evaluate(num);
		return Mathf.Lerp(start, end, num2);
	}

	// Token: 0x04003988 RID: 14728
	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	// Token: 0x04003989 RID: 14729
	[SerializeField]
	private int _ellipsisCount = 3;

	// Token: 0x0400398A RID: 14730
	[SerializeField]
	private GameObject _ellipsisPrefab;

	// Token: 0x0400398B RID: 14731
	[SerializeField]
	private GameObject _ellipsisRoot;

	// Token: 0x0400398C RID: 14732
	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	// Token: 0x0400398D RID: 14733
	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	// Token: 0x0400398E RID: 14734
	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	// Token: 0x0400398F RID: 14735
	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	// Token: 0x04003990 RID: 14736
	[SerializeField]
	private float _startingScale = 0.33f;

	// Token: 0x04003991 RID: 14737
	[SerializeField]
	private float _intermediaryScale = 0.66f;

	// Token: 0x04003992 RID: 14738
	[SerializeField]
	private float _endScale = 1f;

	// Token: 0x04003993 RID: 14739
	[SerializeField]
	private float _scaleDuration = 0.25f;

	// Token: 0x04003994 RID: 14740
	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	// Token: 0x04003995 RID: 14741
	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	// Token: 0x04003996 RID: 14742
	private bool _runAnimation;

	// Token: 0x04003997 RID: 14743
	private float _nextChange;

	// Token: 0x04003998 RID: 14744
	[TupleElementNames(new string[] { "ellipsis", "startingScale", "currentScale", "lerpT" })]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	// Token: 0x04003999 RID: 14745
	private Coroutine _animationCoroutine;
}
