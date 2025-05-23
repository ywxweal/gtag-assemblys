using System;
using GorillaExtensions;
using GorillaTagScripts.ModIO;
using TMPro;
using UnityEngine;

// Token: 0x0200073D RID: 1853
public class VirtualStumpTeleporter : MonoBehaviour, IBuildValidation
{
	// Token: 0x06002E50 RID: 11856 RVA: 0x000E74B5 File Offset: 0x000E56B5
	public bool BuildValidationCheck()
	{
		if (this.mySerializer == null && base.transform.parent.parent.GetComponentInChildren<VirtualStumpTeleporterSerializer>() == null)
		{
			Debug.LogError("This teleporter needs a reference to a VirtualStumpTeleporterSerializer, or to be placed alongside a VirtualStumpTeleporterSerializer. Check out the arcade or the stump", this);
			return false;
		}
		return true;
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x000E74F0 File Offset: 0x000E56F0
	public void OnEnable()
	{
		if (this.mySerializer == null)
		{
			this.mySerializer = base.transform.parent.parent.GetComponentInChildren<VirtualStumpTeleporterSerializer>();
		}
		if (UGCPermissionManager.IsUGCDisabled)
		{
			this.HideHandHolds();
		}
		else
		{
			this.ShowHandHolds();
		}
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000E755D File Offset: 0x000E575D
	public void OnDisable()
	{
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000E7581 File Offset: 0x000E5781
	private void OnUGCEnabled()
	{
		this.ShowHandHolds();
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000E7589 File Offset: 0x000E5789
	private void OnUGCDisabled()
	{
		this.HideHandHolds();
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x000E7594 File Offset: 0x000E5794
	public void OnTriggerEnter(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.teleporting || CustomMapManager.WaitingForRoomJoin || CustomMapManager.WaitingForDisconnect)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = Time.time;
			this.ShowCountdownText();
		}
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x000E75EC File Offset: 0x000E57EC
	public void OnTriggerStay(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject && this.triggerEntryTime >= 0f)
		{
			this.UpdateCountdownText();
			if (!this.teleporting && this.triggerEntryTime + this.stayInTriggerDuration <= Time.time)
			{
				this.TeleportPlayer();
				this.HideCountdownText();
			}
		}
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000E7658 File Offset: 0x000E5858
	public void OnTriggerExit(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = -1f;
			this.HideCountdownText();
		}
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000E7690 File Offset: 0x000E5890
	private void ShowCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			int num = 1 + Mathf.FloorToInt(this.stayInTriggerDuration);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
					this.countdownTexts[i].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000E770C File Offset: 0x000E590C
	private void HideCountdownText()
	{
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = "";
					this.countdownTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000E7770 File Offset: 0x000E5970
	private void UpdateCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			float num = this.stayInTriggerDuration - (Time.time - this.triggerEntryTime);
			int num2 = 1 + Mathf.FloorToInt(num);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num2.ToString();
				}
			}
		}
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x000E77E8 File Offset: 0x000E59E8
	public void TeleportPlayer()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.teleporting)
		{
			this.teleporting = true;
			base.StartCoroutine(CustomMapManager.TeleportToVirtualStump(this.teleporterIndex, new Action<bool>(this.FinishTeleport), this.entrancePoint, this.mySerializer));
		}
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x000E7836 File Offset: 0x000E5A36
	private void FinishTeleport(bool success = true)
	{
		if (this.teleporting)
		{
			this.teleporting = false;
			this.triggerEntryTime = -1f;
		}
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x000E7854 File Offset: 0x000E5A54
	private void HideHandHolds()
	{
		foreach (GameObject gameObject in this.handHoldObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x000E788C File Offset: 0x000E5A8C
	private void ShowHandHolds()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		foreach (GameObject gameObject in this.handHoldObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x040034D3 RID: 13523
	[SerializeField]
	private short teleporterIndex;

	// Token: 0x040034D4 RID: 13524
	[SerializeField]
	private float stayInTriggerDuration = 3f;

	// Token: 0x040034D5 RID: 13525
	[SerializeField]
	private TMP_Text[] countdownTexts;

	// Token: 0x040034D6 RID: 13526
	[SerializeField]
	private GameObject[] handHoldObjects;

	// Token: 0x040034D7 RID: 13527
	private VirtualStumpTeleporterSerializer mySerializer;

	// Token: 0x040034D8 RID: 13528
	private bool teleporting;

	// Token: 0x040034D9 RID: 13529
	private float triggerEntryTime = -1f;

	// Token: 0x040034DA RID: 13530
	public GTZone entrancePoint = GTZone.arcade;
}
