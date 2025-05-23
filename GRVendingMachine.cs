using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005D3 RID: 1491
public class GRVendingMachine : MonoBehaviour
{
	// Token: 0x0600245D RID: 9309 RVA: 0x000B6D2C File Offset: 0x000B4F2C
	public Transform GetSpawnMarker()
	{
		return this.itemSpawnLocation;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000B6D34 File Offset: 0x000B4F34
	public void NavButtonPressedLeft()
	{
		this.hIndex = Mathf.Max(0, this.hIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000B6D50 File Offset: 0x000B4F50
	public void NavButtonPressedRight()
	{
		this.hIndex = Mathf.Min(this.hIndex + 1, this.horizontalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000B6D73 File Offset: 0x000B4F73
	public void NavButtonPressedUp()
	{
		this.vIndex = Mathf.Max(0, this.vIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000B6D8F File Offset: 0x000B4F8F
	public void NavButtonPressedDown()
	{
		this.vIndex = Mathf.Min(this.vIndex + 1, this.verticalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000B6DB4 File Offset: 0x000B4FB4
	public void RequestPurchase()
	{
		if (!this.currentlyVending)
		{
			int num = this.vIndex * this.horizontalSteps + this.hIndex;
			if (num >= 0 && num < this.vendingEntries.Count)
			{
				this.vendingIndex = num;
				if (this.vendingCoroutine != null)
				{
					base.StopCoroutine(this.vendingCoroutine);
				}
				this.vendingCoroutine = base.StartCoroutine(this.VendingCoroutine());
			}
		}
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000B6E20 File Offset: 0x000B5020
	private void RefreshCardReaderDisplay()
	{
		int num = this.vIndex * this.horizontalSteps + this.hIndex;
		if (num >= 0 && num < this.vendingEntries.Count)
		{
			int entityTypeId = this.vendingEntries[num].GetEntityTypeId();
			int itemCost = GhostReactor.instance.GetItemCost(entityTypeId);
			this.cardDisplayText.text = this.vendingEntries[num].itemName + "\n" + itemCost.ToString();
		}
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000B6EA2 File Offset: 0x000B50A2
	private void Update()
	{
		if (!this.currentlyVending)
		{
			this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime);
		}
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000B6EDC File Offset: 0x000B50DC
	private bool MoveTransportToSlot(int x, int y, int rows, int cols, float xSpeed, float ySpeed, float dt)
	{
		Vector3 vector = Vector3.Lerp(this.horizontalMin.position, this.horizontalMax.position, (float)x / (float)(rows - 1));
		Vector3 vector2 = Vector3.Lerp(this.verticalMin.position, this.verticalMax.position, (float)y / (float)(cols - 1));
		this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, vector, xSpeed * dt);
		this.verticalTransport.position = Vector3.MoveTowards(this.verticalTransport.position, vector2, ySpeed * dt);
		float sqrMagnitude = (this.horizontalTransport.position - vector).sqrMagnitude;
		float sqrMagnitude2 = (this.verticalTransport.position - vector2).sqrMagnitude;
		return sqrMagnitude > 0.001f || sqrMagnitude2 > 0.001f;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000B6FB6 File Offset: 0x000B51B6
	private IEnumerator VendingCoroutine()
	{
		this.currentlyVending = true;
		while (this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
		{
			yield return null;
		}
		int entityTypeId = this.vendingEntries[this.vendingIndex].GetEntityTypeId();
		int itemCost = GhostReactor.instance.GetItemCost(entityTypeId);
		if (this.debugUnlimitedPurchasing || VRRig.LocalRig.GetComponent<GRPlayer>().currency >= itemCost)
		{
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(true);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
			float depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
			while (depositPosSqDist > 0.001f)
			{
				this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, this.depositLocation.position, this.horizontalSpeed * Time.deltaTime);
				depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
				yield return null;
			}
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(false);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
		}
		this.currentlyVending = false;
		yield break;
	}

	// Token: 0x04002970 RID: 10608
	[SerializeField]
	private Transform horizontalTransport;

	// Token: 0x04002971 RID: 10609
	[SerializeField]
	private Transform verticalTransport;

	// Token: 0x04002972 RID: 10610
	[SerializeField]
	private Transform horizontalMin;

	// Token: 0x04002973 RID: 10611
	[SerializeField]
	private Transform horizontalMax;

	// Token: 0x04002974 RID: 10612
	[SerializeField]
	private Transform verticalMin;

	// Token: 0x04002975 RID: 10613
	[SerializeField]
	private Transform verticalMax;

	// Token: 0x04002976 RID: 10614
	[SerializeField]
	private Transform depositLocation;

	// Token: 0x04002977 RID: 10615
	[SerializeField]
	private Transform itemSpawnLocation;

	// Token: 0x04002978 RID: 10616
	[SerializeField]
	private TMP_Text cardDisplayText;

	// Token: 0x04002979 RID: 10617
	[SerializeField]
	private int horizontalSteps = 4;

	// Token: 0x0400297A RID: 10618
	[SerializeField]
	private int verticalSteps = 3;

	// Token: 0x0400297B RID: 10619
	[SerializeField]
	private float horizontalSpeed = 0.25f;

	// Token: 0x0400297C RID: 10620
	[SerializeField]
	private float verticalSpeed = 0.25f;

	// Token: 0x0400297D RID: 10621
	[SerializeField]
	private bool debugUnlimitedPurchasing;

	// Token: 0x0400297E RID: 10622
	[SerializeField]
	private List<GRVendingMachine.VendingEntry> vendingEntries = new List<GRVendingMachine.VendingEntry>();

	// Token: 0x0400297F RID: 10623
	private int hIndex;

	// Token: 0x04002980 RID: 10624
	private int vIndex;

	// Token: 0x04002981 RID: 10625
	private bool currentlyVending;

	// Token: 0x04002982 RID: 10626
	private int vendingIndex;

	// Token: 0x04002983 RID: 10627
	private Coroutine vendingCoroutine;

	// Token: 0x04002984 RID: 10628
	public int VendingMachineId;

	// Token: 0x020005D4 RID: 1492
	[Serializable]
	public struct VendingEntry
	{
		// Token: 0x06002468 RID: 9320 RVA: 0x000B6FFC File Offset: 0x000B51FC
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x04002985 RID: 10629
		public Transform transportVisual;

		// Token: 0x04002986 RID: 10630
		public GameEntity entityPrefab;

		// Token: 0x04002987 RID: 10631
		public string itemName;

		// Token: 0x04002988 RID: 10632
		private int entityTypeId;

		// Token: 0x04002989 RID: 10633
		private bool entityTypeIdSet;
	}
}
