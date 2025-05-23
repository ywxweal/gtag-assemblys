using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020008C9 RID: 2249
public class FriendDisplay : MonoBehaviour
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x060036A9 RID: 13993 RVA: 0x001075DD File Offset: 0x001057DD
	public bool InRemoveMode
	{
		get
		{
			return this.inRemoveMode;
		}
	}

	// Token: 0x060036AA RID: 13994 RVA: 0x001075E8 File Offset: 0x001057E8
	private void Start()
	{
		this.InitFriendCards();
		this.InitLocalPlayerCard();
		this.UpdateLocalPlayerPrivacyButtons();
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x0010764C File Offset: 0x0010584C
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnJoinedRoom;
		}
		if (this.triggerNotifier != null)
		{
			this.triggerNotifier.TriggerEnterEvent -= this.TriggerEntered;
			this.triggerNotifier.TriggerExitEvent -= this.TriggerExited;
		}
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x001076B8 File Offset: 0x001058B8
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh += this.OnGetFriendsReceived;
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
			this.localPlayerAtDisplay = true;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x00107718 File Offset: 0x00105918
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh -= this.OnGetFriendsReceived;
			this.ClearFriendCards();
			this.ClearLocalPlayerCard();
			this.localPlayerAtDisplay = false;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x060036AE RID: 13998 RVA: 0x00107770 File Offset: 0x00105970
	private void OnJoinedRoom()
	{
		this.Refresh();
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x00107778 File Offset: 0x00105978
	private void Refresh()
	{
		if (this.localPlayerAtDisplay)
		{
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
		}
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x00107794 File Offset: 0x00105994
	public void LocalPlayerFullyVisiblePress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Visible);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x001077AF File Offset: 0x001059AF
	public void LocalPlayerPublicOnlyPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.PublicOnly);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x001077CA File Offset: 0x001059CA
	public void LocalPlayerFullyHiddenPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Hidden);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x001077E8 File Offset: 0x001059E8
	private void UpdateLocalPlayerPrivacyButtons()
	{
		FriendSystem.PlayerPrivacy localPlayerPrivacy = FriendSystem.Instance.LocalPlayerPrivacy;
		this.SetButtonAppearance(this._localPlayerFullyVisibleButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Visible);
		this.SetButtonAppearance(this._localPlayerPublicOnlyButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly);
		this.SetButtonAppearance(this._localPlayerFullyHiddenButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden);
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x00107832 File Offset: 0x00105A32
	private void SetButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x00107844 File Offset: 0x00105A44
	private void SetButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		Material[] array;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			array = this._buttonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			array = this._buttonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			array = this._buttonAlertMaterials;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = array;
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x0010789C File Offset: 0x00105A9C
	public void ToggleRemoveFriendMode()
	{
		this.inRemoveMode = !this.inRemoveMode;
		FriendCard[] array = this.friendCards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRemoveEnabled(this.inRemoveMode);
		}
		this.SetButtonAppearance(this._removeFriendButton, this.inRemoveMode ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x001078F4 File Offset: 0x00105AF4
	private void InitFriendCards()
	{
		float num = this.gridWidth / (float)this.gridDimension;
		float num2 = this.gridHeight / (float)this.gridDimension;
		Vector3 right = this.gridRoot.right;
		Vector3 vector = -this.gridRoot.up;
		Vector3 vector2 = this.gridRoot.position - right * (this.gridWidth * 0.5f - num * 0.5f) - vector * (this.gridHeight * 0.5f - num2 * 0.5f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.gridDimension; i++)
		{
			for (int j = 0; j < this.gridDimension; j++)
			{
				FriendCard friendCard = this.friendCards[num4];
				friendCard.gameObject.SetActive(true);
				friendCard.transform.localScale = Vector3.one * (num / friendCard.Width);
				friendCard.transform.position = vector2 + right * num * (float)j + vector * num2 * (float)i;
				friendCard.transform.rotation = this.gridRoot.transform.rotation;
				friendCard.Init(this);
				friendCard.SetButton(this._friendCardButtons[num3++], this._buttonDefaultMaterials, this._buttonActiveMaterials, this._buttonAlertMaterials, this._friendCardButtonText[num4]);
				friendCard.SetEmpty();
				num4++;
			}
		}
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x00107A94 File Offset: 0x00105C94
	public void RandomizeFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].Randomize();
		}
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x00107AC4 File Offset: 0x00105CC4
	private void ClearFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x00107AF1 File Offset: 0x00105CF1
	public void OnGetFriendsReceived(List<FriendBackendController.Friend> friendsList)
	{
		this.PopulateFriendCards(friendsList);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x00107B08 File Offset: 0x00105D08
	private void PopulateFriendCards(List<FriendBackendController.Friend> friendsList)
	{
		int num = 0;
		while (num < friendsList.Count && friendsList[num] != null)
		{
			this.friendCards[num].Populate(friendsList[num]);
			num++;
		}
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x00107B43 File Offset: 0x00105D43
	private void InitLocalPlayerCard()
	{
		this._localPlayerCard.Init(this);
		this.ClearLocalPlayerCard();
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x00107B58 File Offset: 0x00105D58
	private void PopulateLocalPlayerCard()
	{
		string text = PhotonNetworkController.Instance.CurrentRoomZone.GetName<GTZone>().ToUpper();
		this._localPlayerCard.SetName(NetworkSystem.Instance.LocalPlayer.NickName.ToUpper());
		if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.RoomName) || NetworkSystem.Instance.RoomName.Length <= 0)
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		bool flag = NetworkSystem.Instance.RoomName[0] == '@';
		bool flag2 = !NetworkSystem.Instance.SessionIsPrivate;
		if (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden || (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly && !flag2))
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		if (flag)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.Substring(1).ToUpper());
			this._localPlayerCard.SetZone("CUSTOM");
			return;
		}
		if (!flag2)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
			this._localPlayerCard.SetZone("PRIVATE");
			return;
		}
		this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
		this._localPlayerCard.SetZone(text);
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x00107CE5 File Offset: 0x00105EE5
	private void ClearLocalPlayerCard()
	{
		this._localPlayerCard.SetEmpty();
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x00107CF4 File Offset: 0x00105EF4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		float num = this.gridWidth * 0.5f;
		float num2 = this.gridHeight * 0.5f;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, -num4, 0f);
		for (int i = 0; i <= this.gridDimension; i++)
		{
			float num5 = (float)i / (float)this.gridDimension;
			Vector3 vector5 = Vector3.Lerp(vector, vector2, num5);
			Vector3 vector6 = Vector3.Lerp(vector3, vector4, num5);
			Gizmos.DrawLine(vector5, vector6);
			Vector3 vector7 = Vector3.Lerp(vector, vector3, num5);
			Vector3 vector8 = Vector3.Lerp(vector2, vector4, num5);
			Gizmos.DrawLine(vector7, vector8);
		}
	}

	// Token: 0x04003C41 RID: 15425
	[FormerlySerializedAs("gridCenter")]
	[SerializeField]
	private FriendCard[] friendCards = new FriendCard[9];

	// Token: 0x04003C42 RID: 15426
	[SerializeField]
	private Transform gridRoot;

	// Token: 0x04003C43 RID: 15427
	[SerializeField]
	private float gridWidth = 2f;

	// Token: 0x04003C44 RID: 15428
	[SerializeField]
	private float gridHeight = 1f;

	// Token: 0x04003C45 RID: 15429
	[SerializeField]
	private int gridDimension = 3;

	// Token: 0x04003C46 RID: 15430
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04003C47 RID: 15431
	[FormerlySerializedAs("_joinButtons")]
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableDelayButton[] _friendCardButtons;

	// Token: 0x04003C48 RID: 15432
	[SerializeField]
	private TextMeshProUGUI[] _friendCardButtonText;

	// Token: 0x04003C49 RID: 15433
	[SerializeField]
	private MeshRenderer _localPlayerFullyVisibleButton;

	// Token: 0x04003C4A RID: 15434
	[SerializeField]
	private MeshRenderer _localPlayerPublicOnlyButton;

	// Token: 0x04003C4B RID: 15435
	[SerializeField]
	private MeshRenderer _localPlayerFullyHiddenButton;

	// Token: 0x04003C4C RID: 15436
	[SerializeField]
	private MeshRenderer _removeFriendButton;

	// Token: 0x04003C4D RID: 15437
	[SerializeField]
	private FriendCard _localPlayerCard;

	// Token: 0x04003C4E RID: 15438
	[SerializeField]
	private Material[] _buttonDefaultMaterials;

	// Token: 0x04003C4F RID: 15439
	[SerializeField]
	private Material[] _buttonActiveMaterials;

	// Token: 0x04003C50 RID: 15440
	[SerializeField]
	private Material[] _buttonAlertMaterials;

	// Token: 0x04003C51 RID: 15441
	private MeshRenderer[] _joinButtonRenderers;

	// Token: 0x04003C52 RID: 15442
	private bool inRemoveMode;

	// Token: 0x04003C53 RID: 15443
	private bool localPlayerAtDisplay;

	// Token: 0x020008CA RID: 2250
	public enum ButtonState
	{
		// Token: 0x04003C55 RID: 15445
		Default,
		// Token: 0x04003C56 RID: 15446
		Active,
		// Token: 0x04003C57 RID: 15447
		Alert
	}
}
