using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;

namespace FXP
{
	// Token: 0x02000BA3 RID: 2979
	public class CosmeticItemPrefab : MonoBehaviour
	{
		// Token: 0x060049E3 RID: 18915 RVA: 0x00160A34 File Offset: 0x0015EC34
		private void Awake()
		{
			this.JonsAwakeCode();
		}

		// Token: 0x060049E4 RID: 18916 RVA: 0x00160A3C File Offset: 0x0015EC3C
		private void JonsAwakeCode()
		{
			this.lastUpdated = -this.updateClock;
			this.isValid = this.goPedestal && this.goMannequin && this.goCosmeticItem && this.goCosmeticItemNameplate && this.goClock && this.goPreviewMode && this.goAttractMode && this.goPurchaseMode;
			this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			this.goAttractModeSFX = this.goAttractMode.transform.FindChildRecursive("SFXAttractMode").GetComponent<AudioSource>();
			this.goPurchaseModeSFX = this.goPurchaseMode.transform.FindChildRecursive("SFXPurchaseMode").GetComponent<AudioSource>();
			this.goAttractModeVFX = this.goAttractMode.transform.FindChildRecursive("VFXAttractMode").GetComponent<ParticleSystem>();
			this.goPurchaseModeVFX = this.goPurchaseMode.transform.FindChildRecursive("VFXPurchaseMode").GetComponent<ParticleSystem>();
			this.clockTextMesh = this.goClock.GetComponent<TextMeshPro>();
			this.clockTextMeshIsValid = this.clockTextMesh != null;
			if (this.clockTextMeshIsValid)
			{
				this.defaultCountdownTextTemplate = this.clockTextMesh.text;
			}
			this.isValid = this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX;
		}

		// Token: 0x060049E5 RID: 18917 RVA: 0x00160BC5 File Offset: 0x0015EDC5
		private void OnDisable()
		{
			if (StoreUpdater.instance != null)
			{
				this.countdownTimerCoRoutine = null;
				this.StopCountdownCoroutine();
				StoreUpdater.instance.PedestalAsleep(this);
			}
		}

		// Token: 0x060049E6 RID: 18918 RVA: 0x00160BF0 File Offset: 0x0015EDF0
		private void OnEnable()
		{
			if (this.goPreviewModeSFX == null)
			{
				this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goAttractModeSFX == null)
			{
				this.goAttractModeSFX = this.goAttractMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goPurchaseModeSFX == null)
			{
				this.goPurchaseModeSFX = this.goPurchaseMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			this.isValid = this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX;
			if (StoreUpdater.instance != null)
			{
				StoreUpdater.instance.PedestalAwakened(this);
			}
		}

		// Token: 0x060049E7 RID: 18919 RVA: 0x00160CC0 File Offset: 0x0015EEC0
		public void SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode NewDisplayMode)
		{
			if (!this.isValid)
			{
				return;
			}
			if (NewDisplayMode.Equals(CosmeticItemPrefab.EDisplayMode.NULL))
			{
				return;
			}
			if (NewDisplayMode == this.currentDisplayMode)
			{
				return;
			}
			switch (NewDisplayMode)
			{
			case CosmeticItemPrefab.EDisplayMode.HIDDEN:
			{
				this.goPedestal.SetActive(false);
				this.goMannequin.SetActive(false);
				this.goCosmeticItem.SetActive(false);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				AudioSource audioSource = this.goPreviewModeSFX;
				if (audioSource != null)
				{
					audioSource.GTStop();
				}
				this.goAttractMode.SetActive(false);
				AudioSource audioSource2 = this.goAttractModeSFX;
				if (audioSource2 != null)
				{
					audioSource2.GTStop();
				}
				this.goPurchaseMode.SetActive(false);
				AudioSource audioSource3 = this.goPurchaseModeSFX;
				if (audioSource3 != null)
				{
					audioSource3.GTStop();
				}
				this.StopPreviewTimer();
				this.StopAttractTimer();
				break;
			}
			case CosmeticItemPrefab.EDisplayMode.PREVIEW:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(true);
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goPreviewMode.SetActive(true);
				this.goPreviewModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.ATTRACT:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(true);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goAttractMode.SetActive(true);
				this.goAttractModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartAttractTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.PURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(true);
				this.goPurchaseModeSFX.GTPlay();
				this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = "Purchased!";
				this.StopPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.POSTPURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.StopPreviewTimer();
				break;
			}
			this.currentDisplayMode = NewDisplayMode;
		}

		// Token: 0x060049E8 RID: 18920 RVA: 0x0016100E File Offset: 0x0015F20E
		private void Update()
		{
			if (Time.time > this.lastUpdated + this.updateClock)
			{
				this.lastUpdated = Time.time;
				this.UpdateClock();
			}
		}

		// Token: 0x060049E9 RID: 18921 RVA: 0x00161038 File Offset: 0x0015F238
		private void UpdateClock()
		{
			if (this.currentUpdateEvent != null && this.clockTextMeshIsValid && this.clockTextMesh.isActiveAndEnabled)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				this.clockTextMesh.text = CountdownText.GetTimeDisplay(timeSpan, this.defaultCountdownTextTemplate);
			}
		}

		// Token: 0x060049EA RID: 18922 RVA: 0x0016109C File Offset: 0x0015F29C
		public void SetDefaultProperties()
		{
			if (!this.isValid)
			{
				return;
			}
			this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.defaultPedestalMesh;
			this.goPedestal.GetComponent<MeshRenderer>().sharedMaterial = this.defaultPedestalMaterial;
			this.goMannequin.GetComponent<MeshFilter>().sharedMesh = this.defaultMannequinMesh;
			this.goMannequin.GetComponent<MeshRenderer>().sharedMaterial = this.defaultMannequinMaterial;
			this.goCosmeticItem.GetComponent<MeshFilter>().sharedMesh = this.defaultCosmeticMesh;
			this.goCosmeticItem.GetComponent<MeshRenderer>().sharedMaterial = this.defaultCosmeticMaterial;
			this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = this.defaultItemText;
			this.goPreviewModeSFX.clip = this.defaultSFXPreviewMode;
			this.goAttractModeSFX.clip = this.defaultSFXAttractMode;
			this.goPurchaseModeSFX.clip = this.defaultSFXPurchaseMode;
		}

		// Token: 0x060049EB RID: 18923 RVA: 0x0016117F File Offset: 0x0015F37F
		private void ClearCosmeticMesh()
		{
			Object.Destroy(this.goCosmeticItemGameObject);
		}

		// Token: 0x060049EC RID: 18924 RVA: 0x0016118C File Offset: 0x0015F38C
		private void ClearCosmeticAtlas()
		{
			if (this.goCosmeticItemMeshAtlas.IsNotNull())
			{
				Object.Destroy(this.goCosmeticItemMeshAtlas);
			}
		}

		// Token: 0x060049ED RID: 18925 RVA: 0x001611A8 File Offset: 0x0015F3A8
		public void SetCosmeticItemFromCosmeticController(CosmeticsController.CosmeticItem item)
		{
			if (!this.isValid)
			{
				return;
			}
			this.ClearCosmeticAtlas();
			this.ClearCosmeticMesh();
			this.oldItemID = this.itemID;
			this.itemID = item.itemName;
			this.itemName = item.displayName;
			if (item.overrideDisplayName != string.Empty)
			{
				this.itemName = item.overrideDisplayName;
			}
			this.HeadModel.SetCosmeticActive(this.itemID, false);
			this.SetCosmeticStand();
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x00161224 File Offset: 0x0015F424
		public void SetCosmeticStand()
		{
			this.cosmeticStand.thisCosmeticName = this.itemID;
			this.cosmeticStand.InitializeCosmetic();
			if (this.oldItemID.Length > 0)
			{
				if (this.oldItemID != this.itemID)
				{
					this.cosmeticStand.isOn = false;
				}
				this.cosmeticStand.UpdateColor();
			}
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x00161288 File Offset: 0x0015F488
		public void SetStoreUpdateEvent(StoreUpdateEvent storeUpdateEvent, bool playFX)
		{
			if (!this.isValid)
			{
				return;
			}
			if (playFX)
			{
				this.goAttractMode.SetActive(true);
				this.goAttractModeVFX.Play();
			}
			this.currentUpdateEvent = storeUpdateEvent;
			this.SetCosmeticItemFromCosmeticController(CosmeticsController.instance.GetItemFromDict(storeUpdateEvent.ItemName));
			if (base.isActiveAndEnabled)
			{
				this.countdownTimerCoRoutine = base.StartCoroutine(this.PlayCountdownTimer());
			}
			this.UpdateClock();
		}

		// Token: 0x060049F0 RID: 18928 RVA: 0x001612F7 File Offset: 0x0015F4F7
		private IEnumerator PlayCountdownTimer()
		{
			yield return new WaitForSeconds(Mathf.Clamp((float)((this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted).TotalSeconds - 10.0), 0f, float.MaxValue));
			this.PlaySFX();
			yield break;
		}

		// Token: 0x060049F1 RID: 18929 RVA: 0x00161306 File Offset: 0x0015F506
		public void StopCountdownCoroutine()
		{
			this.CountdownSFX.GTStop();
			this.goAttractModeVFX.Stop();
			if (this.countdownTimerCoRoutine != null)
			{
				base.StopCoroutine(this.countdownTimerCoRoutine);
				this.countdownTimerCoRoutine = null;
			}
		}

		// Token: 0x060049F2 RID: 18930 RVA: 0x0016133C File Offset: 0x0015F53C
		private void PlaySFX()
		{
			if (this.currentUpdateEvent != null)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				if (timeSpan.TotalSeconds >= 10.0)
				{
					this.CountdownSFX.time = 0f;
					this.CountdownSFX.GTPlay();
					return;
				}
				this.CountdownSFX.time = 10f - (float)timeSpan.TotalSeconds;
				this.CountdownSFX.GTPlay();
			}
		}

		// Token: 0x060049F3 RID: 18931 RVA: 0x001613C8 File Offset: 0x0015F5C8
		public void SetCosmeticItemProperties(string WhichGUID, string Name, List<Transform> SocketsList, int Socket, string PedestalMesh = null, string MannequinMesh = null)
		{
			if (!this.isValid)
			{
				return;
			}
			Guid guid;
			if (!Guid.TryParse(WhichGUID, out guid))
			{
				return;
			}
			this.itemName = Name;
			this.itemSocket = Socket;
			if (this.pedestalMesh != null)
			{
				this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.pedestalMesh;
			}
		}

		// Token: 0x060049F4 RID: 18932 RVA: 0x0016141C File Offset: 0x0015F61C
		private void StartPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.coroutinePreviewTimer = this.DoPreviewTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInPreviewMode ?? this.defaultHoursInPreviewMode) * 60 * 60)));
			base.StartCoroutine(this.coroutinePreviewTimer);
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x0016149B File Offset: 0x0015F69B
		private void StopPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.clockTextMesh.text = "Clock";
		}

		// Token: 0x060049F6 RID: 18934 RVA: 0x001614D1 File Offset: 0x0015F6D1
		private IEnumerator DoPreviewTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.clockTextMesh.text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.ATTRACT);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x060049F7 RID: 18935 RVA: 0x001614E8 File Offset: 0x0015F6E8
		public void StartAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.coroutineAttractTimer = this.DoAttractTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInAttractMode ?? this.defaultHoursInAttractMode) * 60 * 60)));
			base.StartCoroutine(this.coroutineAttractTimer);
		}

		// Token: 0x060049F8 RID: 18936 RVA: 0x00161567 File Offset: 0x0015F767
		private void StopAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.goClock.GetComponent<TextMesh>().text = "Clock";
		}

		// Token: 0x060049F9 RID: 18937 RVA: 0x001615A2 File Offset: 0x0015F7A2
		private IEnumerator DoAttractTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.goClock.GetComponent<TextMesh>().text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.HIDDEN);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x04004CAA RID: 19626
		public string PedestalID = "";

		// Token: 0x04004CAB RID: 19627
		public HeadModel HeadModel;

		// Token: 0x04004CAC RID: 19628
		[SerializeField]
		private Guid? itemGUID;

		// Token: 0x04004CAD RID: 19629
		[SerializeField]
		private string itemName = string.Empty;

		// Token: 0x04004CAE RID: 19630
		[SerializeField]
		private List<Transform> sockets = new List<Transform>();

		// Token: 0x04004CAF RID: 19631
		[SerializeField]
		private int itemSocket = int.MinValue;

		// Token: 0x04004CB0 RID: 19632
		[SerializeField]
		private int? hoursInPreviewMode;

		// Token: 0x04004CB1 RID: 19633
		[SerializeField]
		private int? hoursInAttractMode;

		// Token: 0x04004CB2 RID: 19634
		[SerializeField]
		private Mesh pedestalMesh;

		// Token: 0x04004CB3 RID: 19635
		[SerializeField]
		private Mesh mannequinMesh;

		// Token: 0x04004CB4 RID: 19636
		[SerializeField]
		private Mesh cosmeticMesh;

		// Token: 0x04004CB5 RID: 19637
		[SerializeField]
		private AudioClip sfxPreviewMode;

		// Token: 0x04004CB6 RID: 19638
		[SerializeField]
		private AudioClip sfxAttractMode;

		// Token: 0x04004CB7 RID: 19639
		[SerializeField]
		private AudioClip sfxPurchaseMode;

		// Token: 0x04004CB8 RID: 19640
		[SerializeField]
		private ParticleSystem vfxPreviewMode;

		// Token: 0x04004CB9 RID: 19641
		[SerializeField]
		private ParticleSystem vfxAttractMode;

		// Token: 0x04004CBA RID: 19642
		[SerializeField]
		private ParticleSystem vfxPurchaseMode;

		// Token: 0x04004CBB RID: 19643
		[SerializeField]
		private GameObject goPedestal;

		// Token: 0x04004CBC RID: 19644
		[SerializeField]
		private GameObject goMannequin;

		// Token: 0x04004CBD RID: 19645
		[SerializeField]
		private GameObject goCosmeticItem;

		// Token: 0x04004CBE RID: 19646
		[SerializeField]
		private GameObject goCosmeticItemGameObject;

		// Token: 0x04004CBF RID: 19647
		[SerializeField]
		private GameObject goCosmeticItemNameplate;

		// Token: 0x04004CC0 RID: 19648
		[SerializeField]
		private GameObject goClock;

		// Token: 0x04004CC1 RID: 19649
		[SerializeField]
		private GameObject goPreviewMode;

		// Token: 0x04004CC2 RID: 19650
		[SerializeField]
		private GameObject goAttractMode;

		// Token: 0x04004CC3 RID: 19651
		[SerializeField]
		private GameObject goPurchaseMode;

		// Token: 0x04004CC4 RID: 19652
		[SerializeField]
		private Mesh defaultPedestalMesh;

		// Token: 0x04004CC5 RID: 19653
		[SerializeField]
		private Material defaultPedestalMaterial;

		// Token: 0x04004CC6 RID: 19654
		[SerializeField]
		private Mesh defaultMannequinMesh;

		// Token: 0x04004CC7 RID: 19655
		[SerializeField]
		private Material defaultMannequinMaterial;

		// Token: 0x04004CC8 RID: 19656
		[SerializeField]
		private Mesh defaultCosmeticMesh;

		// Token: 0x04004CC9 RID: 19657
		[SerializeField]
		private Material defaultCosmeticMaterial;

		// Token: 0x04004CCA RID: 19658
		[SerializeField]
		private string defaultItemText;

		// Token: 0x04004CCB RID: 19659
		[SerializeField]
		private int defaultHoursInPreviewMode;

		// Token: 0x04004CCC RID: 19660
		[SerializeField]
		private int defaultHoursInAttractMode;

		// Token: 0x04004CCD RID: 19661
		[SerializeField]
		private AudioClip defaultSFXPreviewMode;

		// Token: 0x04004CCE RID: 19662
		[SerializeField]
		private AudioClip defaultSFXAttractMode;

		// Token: 0x04004CCF RID: 19663
		[SerializeField]
		private AudioClip defaultSFXPurchaseMode;

		// Token: 0x04004CD0 RID: 19664
		private GameObject goCosmeticItemMeshAtlas;

		// Token: 0x04004CD1 RID: 19665
		public AudioSource CountdownSFX;

		// Token: 0x04004CD2 RID: 19666
		private CosmeticItemPrefab.EDisplayMode currentDisplayMode;

		// Token: 0x04004CD3 RID: 19667
		private bool isValid;

		// Token: 0x04004CD4 RID: 19668
		[Nullable(2)]
		private AudioSource goPreviewModeSFX;

		// Token: 0x04004CD5 RID: 19669
		[Nullable(2)]
		private AudioSource goAttractModeSFX;

		// Token: 0x04004CD6 RID: 19670
		[Nullable(2)]
		private AudioSource goPurchaseModeSFX;

		// Token: 0x04004CD7 RID: 19671
		[Nullable(2)]
		private ParticleSystem goAttractModeVFX;

		// Token: 0x04004CD8 RID: 19672
		[Nullable(2)]
		private ParticleSystem goPurchaseModeVFX;

		// Token: 0x04004CD9 RID: 19673
		private IEnumerator coroutinePreviewTimer;

		// Token: 0x04004CDA RID: 19674
		private IEnumerator coroutineAttractTimer;

		// Token: 0x04004CDB RID: 19675
		private DateTime startTime;

		// Token: 0x04004CDC RID: 19676
		private TextMeshPro clockTextMesh;

		// Token: 0x04004CDD RID: 19677
		private bool clockTextMeshIsValid;

		// Token: 0x04004CDE RID: 19678
		private StoreUpdateEvent currentUpdateEvent;

		// Token: 0x04004CDF RID: 19679
		private string defaultCountdownTextTemplate = "";

		// Token: 0x04004CE0 RID: 19680
		public CosmeticStand cosmeticStand;

		// Token: 0x04004CE1 RID: 19681
		public string itemID = "";

		// Token: 0x04004CE2 RID: 19682
		public string oldItemID = "";

		// Token: 0x04004CE3 RID: 19683
		private Coroutine countdownTimerCoRoutine;

		// Token: 0x04004CE4 RID: 19684
		private float updateClock = 60f;

		// Token: 0x04004CE5 RID: 19685
		private float lastUpdated;

		// Token: 0x02000BA4 RID: 2980
		[SerializeField]
		public enum EDisplayMode
		{
			// Token: 0x04004CE7 RID: 19687
			NULL,
			// Token: 0x04004CE8 RID: 19688
			HIDDEN,
			// Token: 0x04004CE9 RID: 19689
			PREVIEW,
			// Token: 0x04004CEA RID: 19690
			ATTRACT,
			// Token: 0x04004CEB RID: 19691
			PURCHASE,
			// Token: 0x04004CEC RID: 19692
			POSTPURCHASE
		}
	}
}
