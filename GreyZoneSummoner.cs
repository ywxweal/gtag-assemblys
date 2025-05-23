using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000117 RID: 279
public class GreyZoneSummoner : MonoBehaviour
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000735 RID: 1845 RVA: 0x000291F0 File Offset: 0x000273F0
	public Vector3 SummoningFocusPoint
	{
		get
		{
			return this.summoningFocusPoint.position;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000736 RID: 1846 RVA: 0x000291FD File Offset: 0x000273FD
	public float SummonerMaxDistance
	{
		get
		{
			return this.areaTriggerCollider.radius + 1f;
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00029210 File Offset: 0x00027410
	private void OnEnable()
	{
		this.greyZoneManager = GreyZoneManager.Instance;
		if (this.greyZoneManager == null)
		{
			return;
		}
		this.greyZoneManager.RegisterSummoner(this);
		this.areaTriggerNotifier.TriggerEnterEvent += this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent += this.ColliderExitedArea;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00029274 File Offset: 0x00027474
	private void OnDisable()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.DeregisterSummoner(this);
		}
		this.areaTriggerNotifier.TriggerEnterEvent -= this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent -= this.ColliderExitedArea;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000292CC File Offset: 0x000274CC
	public void UpdateProgressFeedback(bool greyZoneAvailable)
	{
		if (this.greyZoneManager == null)
		{
			return;
		}
		if (greyZoneAvailable && !this.candlesParent.gameObject.activeSelf)
		{
			this.candlesParent.gameObject.SetActive(true);
		}
		this.candlesTimeline.time = (double)Mathf.Clamp01(this.greyZoneManager.SummoningProgress) * this.candlesTimeline.duration;
		this.candlesTimeline.Evaluate();
		if (!this.greyZoneManager.GreyZoneActive)
		{
			float num = (float)this.summoningTones.Count * this.greyZoneManager.SummoningProgress;
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				float num2 = Mathf.InverseLerp((float)i, (float)i + 1f + this.summoningTonesFadeOverlap, num);
				this.summoningTones[i].volume = num2 * this.summoningTonesMaxVolume;
			}
		}
		this.greyZoneActivationButton.isOn = this.greyZoneManager.GreyZoneActive;
		this.greyZoneActivationButton.UpdateColor();
		for (int j = 0; j < this.greyZoneGravityFactorButtons.Count; j++)
		{
			this.greyZoneGravityFactorButtons[j].isOn = this.greyZoneManager.GravityFactorSelection == j;
			this.greyZoneGravityFactorButtons[j].UpdateColor();
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00029415 File Offset: 0x00027615
	public void OnGreyZoneActivated()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeOutSummoningTones());
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0002942A File Offset: 0x0002762A
	private IEnumerator FadeOutSummoningTones()
	{
		float fadeStartTime = Time.time;
		float fadeRate = 1f / this.summoningTonesFadeTime;
		while (Time.time < fadeStartTime + this.summoningTonesFadeTime)
		{
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				this.summoningTones[i].volume = Mathf.MoveTowards(this.summoningTones[i].volume, 0f, this.summoningTonesMaxVolume * fadeRate * Time.deltaTime);
			}
			yield return null;
		}
		for (int j = 0; j < this.summoningTones.Count; j++)
		{
			this.summoningTones[j].volume = 0f;
		}
		yield break;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0002943C File Offset: 0x0002763C
	public void ColliderEnteredArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntity component = other.GetComponent<ZoneEntity>();
		VRRig vrrig = ((component != null) ? component.entityRig : null);
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigEnteredSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00029488 File Offset: 0x00027688
	public void ColliderExitedArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntity component = other.GetComponent<ZoneEntity>();
		VRRig vrrig = ((component != null) ? component.entityRig : null);
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigExitedSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x040008AD RID: 2221
	[SerializeField]
	private Transform summoningFocusPoint;

	// Token: 0x040008AE RID: 2222
	[SerializeField]
	private Transform candlesParent;

	// Token: 0x040008AF RID: 2223
	[SerializeField]
	private PlayableDirector candlesTimeline;

	// Token: 0x040008B0 RID: 2224
	[SerializeField]
	private TriggerEventNotifier areaTriggerNotifier;

	// Token: 0x040008B1 RID: 2225
	[SerializeField]
	private SphereCollider areaTriggerCollider;

	// Token: 0x040008B2 RID: 2226
	[SerializeField]
	private GorillaPressableButton greyZoneActivationButton;

	// Token: 0x040008B3 RID: 2227
	[SerializeField]
	private List<AudioSource> summoningTones = new List<AudioSource>();

	// Token: 0x040008B4 RID: 2228
	[SerializeField]
	private float summoningTonesMaxVolume = 1f;

	// Token: 0x040008B5 RID: 2229
	[SerializeField]
	private float summoningTonesFadeOverlap = 0.5f;

	// Token: 0x040008B6 RID: 2230
	[SerializeField]
	private float summoningTonesFadeTime = 4f;

	// Token: 0x040008B7 RID: 2231
	[SerializeField]
	private List<GorillaPressableButton> greyZoneGravityFactorButtons = new List<GorillaPressableButton>();

	// Token: 0x040008B8 RID: 2232
	private GreyZoneManager greyZoneManager;
}
