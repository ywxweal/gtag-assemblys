using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005C8 RID: 1480
public class GRToolPurchaseStation : MonoBehaviour
{
	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002416 RID: 9238 RVA: 0x000B5914 File Offset: 0x000B3B14
	public int ActiveEntryIndex
	{
		get
		{
			return this.activeEntryIndex;
		}
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000B591C File Offset: 0x000B3B1C
	public void RequestPurchaseButton(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			GhostReactorManager.instance.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
		}
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000B5941 File Offset: 0x000B3B41
	public void ShiftRightButton()
	{
		GhostReactorManager.instance.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftRight);
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000B5954 File Offset: 0x000B3B54
	public void ShiftLeftButton()
	{
		GhostReactorManager.instance.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftLeft);
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000B5967 File Offset: 0x000B3B67
	public void ShiftRightAuthority()
	{
		this.activeEntryIndex = (this.activeEntryIndex + 1) % this.toolEntries.Count;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000B5983 File Offset: 0x000B3B83
	public void ShiftLeftAuthority()
	{
		this.activeEntryIndex = ((this.activeEntryIndex > 0) ? (this.activeEntryIndex - 1) : (this.toolEntries.Count - 1));
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000B59AC File Offset: 0x000B3BAC
	public void DebugPurchase()
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
		Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
		Quaternion quaternion = this.depositTransform.rotation * localRotation;
		Vector3 vector = this.depositTransform.position + this.depositTransform.rotation * localPosition;
		GameEntityManager.instance.RequestCreateItem(entityTypeId, vector, quaternion, 0L);
		this.OnPurchaseSucceeded();
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000B5A68 File Offset: 0x000B3C68
	public bool TryPurchaseAuthority(GRPlayer player, out int itemCost)
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		itemCost = GhostReactor.instance.GetItemCost(entityTypeId);
		if (this.debugIgnoreToolCost || player.currency >= itemCost)
		{
			Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
			Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
			Quaternion quaternion = this.depositTransform.rotation * localRotation;
			Vector3 vector = this.depositTransform.position + this.depositTransform.rotation * localPosition;
			GameEntityManager.instance.RequestCreateItem(entityTypeId, vector, quaternion, 0L);
			return true;
		}
		return false;
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000B5B44 File Offset: 0x000B3D44
	public void OnSelectionUpdate(int newSelectedIndex)
	{
		this.activeEntryIndex = Mathf.Clamp(newSelectedIndex % this.toolEntries.Count, 0, this.toolEntries.Count - 1);
		this.audioSource.PlayOneShot(this.nextItemAudio, this.nextItemVolume);
		this.displayItemNameText.text = this.toolEntries[this.activeEntryIndex].toolName;
		this.displayItemCostText.text = this.toolEntries[this.activeEntryIndex].toolCost.ToString();
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000B5BD8 File Offset: 0x000B3DD8
	public void OnPurchaseSucceeded()
	{
		this.animatingDeposit = true;
		this.animationStartTime = Time.time;
		this.audioSource.PlayOneShot(this.purchaseAudio, this.purchaseVolume);
		UnityEvent onSucceeded = this.idCardScanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		if (this.displayedEntryIndex < 0 || this.displayedEntryIndex >= this.toolEntries.Count)
		{
			this.displayedEntryIndex = this.activeEntryIndex;
		}
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000B5C4C File Offset: 0x000B3E4C
	public void OnPurchaseFailed()
	{
		this.audioSource.PlayOneShot(this.purchaseFailedAudio, this.purchaseFailedVolume);
		UnityEvent onFailed = this.idCardScanner.onFailed;
		if (onFailed == null)
		{
			return;
		}
		onFailed.Invoke();
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000B5C7A File Offset: 0x000B3E7A
	public Transform GetSpawnMarker()
	{
		return this.toolSpawnLocation;
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000B5C82 File Offset: 0x000B3E82
	private void Awake()
	{
		this.depositLidOpenRot = Quaternion.Euler(this.depositLidOpenEuler);
		this.toolEntryRot = Quaternion.Euler(this.toolEntryRotEuler);
		this.toolExitRot = Quaternion.Euler(this.toolExitRotEuler);
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000B5CB8 File Offset: 0x000B3EB8
	private void Update()
	{
		if (!this.animatingSwap && !this.animatingDeposit && this.activeEntryIndex != this.displayedEntryIndex)
		{
			this.animatingSwap = true;
			this.animationStartTime = Time.time;
			this.animPrevToolIndex = this.displayedEntryIndex;
			this.animNextToolIndex = this.activeEntryIndex;
			this.toolEntryRot = Quaternion.AngleAxis(this.toolEntryRotDegrees, Random.onUnitSphere);
		}
		if (this.animatingSwap)
		{
			float num = (Time.time - this.animationStartTime) / this.nextToolAnimationTime;
			Transform transform = null;
			if (this.animPrevToolIndex >= 0 && this.animPrevToolIndex < this.toolEntries.Count)
			{
				transform = this.toolEntries[this.animPrevToolIndex].displayToolParent;
				transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.toolExitRot, this.toolExitRotTimingCurve.Evaluate(num));
				transform.localPosition = Vector3.Lerp(Vector3.zero, this.toolExitPosOffset, this.toolExitPosTimingCurve.Evaluate(num));
			}
			Transform displayToolParent = this.toolEntries[this.animNextToolIndex].displayToolParent;
			displayToolParent.localRotation = Quaternion.Slerp(this.toolEntryRot, Quaternion.identity, this.toolEntryRotTimingCurve.Evaluate(num));
			displayToolParent.localPosition = Vector3.Lerp(this.toolEntryPosOffset, Vector3.zero, this.toolEntryPosTimingCurve.Evaluate(num));
			displayToolParent.gameObject.SetActive(true);
			if (num >= 1f)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				this.displayedEntryIndex = this.animNextToolIndex;
				this.animatingSwap = false;
				return;
			}
		}
		else if (this.animatingDeposit)
		{
			float num2 = (Time.time - this.animationStartTime) / this.toolDepositAnimationTime;
			Transform displayToolParent2 = this.toolEntries[this.displayedEntryIndex].displayToolParent;
			Vector3 localPosition = displayToolParent2.localPosition;
			localPosition.y = Mathf.Lerp(0f, this.depositTransform.localPosition.y, this.toolDepositMotionCurveY.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			localPosition.z = Mathf.Lerp(0f, this.depositTransform.localPosition.z, this.toolDepositMotionCurveZ.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			displayToolParent2.localPosition = localPosition;
			this.depositLidTransform.localRotation = Quaternion.Slerp(Quaternion.identity, this.depositLidOpenRot, this.depositLidTimingCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.depositLidTransform.localRotation = Quaternion.identity;
				displayToolParent2.gameObject.SetActive(false);
				this.displayedEntryIndex = -1;
				this.animatingDeposit = false;
			}
		}
	}

	// Token: 0x04002907 RID: 10503
	[SerializeField]
	private List<GRToolPurchaseStation.ToolEntry> toolEntries = new List<GRToolPurchaseStation.ToolEntry>();

	// Token: 0x04002908 RID: 10504
	[SerializeField]
	private Transform displayTransform;

	// Token: 0x04002909 RID: 10505
	[SerializeField]
	private Transform depositTransform;

	// Token: 0x0400290A RID: 10506
	[SerializeField]
	private Transform toolSpawnLocation;

	// Token: 0x0400290B RID: 10507
	[SerializeField]
	private TMP_Text displayItemNameText;

	// Token: 0x0400290C RID: 10508
	[SerializeField]
	private TMP_Text displayItemCostText;

	// Token: 0x0400290D RID: 10509
	[SerializeField]
	private float nextToolAnimationTime = 0.5f;

	// Token: 0x0400290E RID: 10510
	[SerializeField]
	private float toolDepositAnimationTime = 1f;

	// Token: 0x0400290F RID: 10511
	[SerializeField]
	private Vector3 toolEntryPosOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x04002910 RID: 10512
	[SerializeField]
	private Vector3 toolEntryRotEuler = new Vector3(0f, 0f, 15f);

	// Token: 0x04002911 RID: 10513
	[SerializeField]
	private float toolEntryRotDegrees = 15f;

	// Token: 0x04002912 RID: 10514
	[SerializeField]
	private Vector3 toolExitPosOffset = new Vector3(0f, 0f, -0.25f);

	// Token: 0x04002913 RID: 10515
	[SerializeField]
	private Vector3 toolExitRotEuler = new Vector3(180f, 0f, 0f);

	// Token: 0x04002914 RID: 10516
	[SerializeField]
	private AnimationCurve toolEntryPosTimingCurve;

	// Token: 0x04002915 RID: 10517
	[SerializeField]
	private AnimationCurve toolEntryRotTimingCurve;

	// Token: 0x04002916 RID: 10518
	[SerializeField]
	private AnimationCurve toolExitPosTimingCurve;

	// Token: 0x04002917 RID: 10519
	[SerializeField]
	private AnimationCurve toolExitRotTimingCurve;

	// Token: 0x04002918 RID: 10520
	[SerializeField]
	private AnimationCurve toolDepositTimingCurve;

	// Token: 0x04002919 RID: 10521
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveY;

	// Token: 0x0400291A RID: 10522
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveZ;

	// Token: 0x0400291B RID: 10523
	[SerializeField]
	private Transform depositLidTransform;

	// Token: 0x0400291C RID: 10524
	[SerializeField]
	private Vector3 depositLidOpenEuler = new Vector3(65f, 0f, 0f);

	// Token: 0x0400291D RID: 10525
	[SerializeField]
	private AnimationCurve depositLidTimingCurve;

	// Token: 0x0400291E RID: 10526
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400291F RID: 10527
	[SerializeField]
	private AudioClip nextItemAudio;

	// Token: 0x04002920 RID: 10528
	[SerializeField]
	private float nextItemVolume = 0.5f;

	// Token: 0x04002921 RID: 10529
	[SerializeField]
	private AudioClip purchaseAudio;

	// Token: 0x04002922 RID: 10530
	[SerializeField]
	private float purchaseVolume = 0.5f;

	// Token: 0x04002923 RID: 10531
	[SerializeField]
	private AudioClip purchaseFailedAudio;

	// Token: 0x04002924 RID: 10532
	[SerializeField]
	private float purchaseFailedVolume = 0.5f;

	// Token: 0x04002925 RID: 10533
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x04002926 RID: 10534
	private int activeEntryIndex = 1;

	// Token: 0x04002927 RID: 10535
	private int displayedEntryIndex = -1;

	// Token: 0x04002928 RID: 10536
	private float animationStartTime;

	// Token: 0x04002929 RID: 10537
	private bool animatingDeposit;

	// Token: 0x0400292A RID: 10538
	private bool animatingSwap;

	// Token: 0x0400292B RID: 10539
	private int animPrevToolIndex;

	// Token: 0x0400292C RID: 10540
	private int animNextToolIndex;

	// Token: 0x0400292D RID: 10541
	private Quaternion depositLidOpenRot = Quaternion.identity;

	// Token: 0x0400292E RID: 10542
	private Quaternion toolEntryRot = Quaternion.identity;

	// Token: 0x0400292F RID: 10543
	private Quaternion toolExitRot = Quaternion.identity;

	// Token: 0x04002930 RID: 10544
	private Coroutine vendingCoroutine;

	// Token: 0x04002931 RID: 10545
	private bool debugIgnoreToolCost;

	// Token: 0x04002932 RID: 10546
	[HideInInspector]
	public int PurchaseStationId;

	// Token: 0x020005C9 RID: 1481
	[Serializable]
	public struct ToolEntry
	{
		// Token: 0x06002425 RID: 9253 RVA: 0x000B6075 File Offset: 0x000B4275
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x04002933 RID: 10547
		public Transform displayToolParent;

		// Token: 0x04002934 RID: 10548
		public GameEntity entityPrefab;

		// Token: 0x04002935 RID: 10549
		public string toolName;

		// Token: 0x04002936 RID: 10550
		public int toolCost;

		// Token: 0x04002937 RID: 10551
		private int entityTypeId;

		// Token: 0x04002938 RID: 10552
		private bool entityTypeIdSet;
	}
}
