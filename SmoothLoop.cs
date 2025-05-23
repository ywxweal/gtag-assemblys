using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class SmoothLoop : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06001AC4 RID: 6852 RVA: 0x00082E45 File Offset: 0x00081045
	public bool BuildValidationCheck()
	{
		if (this.source == null)
		{
			Debug.LogError("missing audio source, this will fail", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x00082E68 File Offset: 0x00081068
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.GTStop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x00082EE8 File Offset: 0x000810E8
	public void SliceUpdate()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.source.time > this.source.clip.length * this.loopEnd)
		{
			this.source.time = this.loopStart;
		}
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x00082F28 File Offset: 0x00081128
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.sourceCheck())
		{
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x00082F8C File Offset: 0x0008118C
	private bool sourceCheck()
	{
		if (!this.source || !this.source.clip)
		{
			Debug.LogError("SmoothLoop: Disabling because AudioSource is null or has no clip assigned. Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			base.StopAllCoroutines();
			return false;
		}
		return true;
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x00082FE3 File Offset: 0x000811E3
	public IEnumerator DelayedStart()
	{
		if (!this.sourceCheck())
		{
			yield break;
		}
		yield return new WaitForSeconds(this.delay);
		this.source.GTPlay();
		yield break;
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04001DCC RID: 7628
	public AudioSource source;

	// Token: 0x04001DCD RID: 7629
	public float delay;

	// Token: 0x04001DCE RID: 7630
	public bool randomStart;

	// Token: 0x04001DCF RID: 7631
	[SerializeField]
	[Range(0f, 1f)]
	private float loopStart = 0.1f;

	// Token: 0x04001DD0 RID: 7632
	[SerializeField]
	[Range(0f, 1f)]
	private float loopEnd = 0.95f;
}
