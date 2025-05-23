using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020008C8 RID: 2248
public class FriendCard : MonoBehaviour
{
	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x0600368D RID: 13965 RVA: 0x00106B2C File Offset: 0x00104D2C
	public TextMeshProUGUI NameText
	{
		get
		{
			return this.nameText;
		}
	}

	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x0600368E RID: 13966 RVA: 0x00106B34 File Offset: 0x00104D34
	public TextMeshProUGUI RoomText
	{
		get
		{
			return this.roomText;
		}
	}

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x0600368F RID: 13967 RVA: 0x00106B3C File Offset: 0x00104D3C
	public TextMeshProUGUI ZoneText
	{
		get
		{
			return this.zoneText;
		}
	}

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x06003690 RID: 13968 RVA: 0x00106B44 File Offset: 0x00104D44
	public float Width
	{
		get
		{
			return this.width;
		}
	}

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x06003691 RID: 13969 RVA: 0x00106B4C File Offset: 0x00104D4C
	// (set) Token: 0x06003692 RID: 13970 RVA: 0x00106B54 File Offset: 0x00104D54
	public float Height { get; private set; } = 0.25f;

	// Token: 0x06003693 RID: 13971 RVA: 0x00106B5D File Offset: 0x00104D5D
	private void Awake()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x00106B7D File Offset: 0x00104D7D
	private void OnDestroy()
	{
		if (this._button)
		{
			this._button.onPressed -= this.OnButtonPressed;
		}
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x00106BA3 File Offset: 0x00104DA3
	public void Init(FriendDisplay owner)
	{
		this.friendDisplay = owner;
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x00106BAC File Offset: 0x00104DAC
	private void UpdateComponentStates()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(this.canRemove);
		}
		if (this.canRemove)
		{
			this.SetButtonState((this.currentFriend != null) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
			return;
		}
		if (this.joinable)
		{
			this.SetButtonState(FriendDisplay.ButtonState.Active);
			return;
		}
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x00106C10 File Offset: 0x00104E10
	private void SetButtonState(FriendDisplay.ButtonState newState)
	{
		if (this._button == null)
		{
			return;
		}
		if (this._buttonState == newState)
		{
			return;
		}
		this._buttonState = newState;
		MeshRenderer buttonRenderer = this._button.buttonRenderer;
		FriendDisplay.ButtonState buttonState = this._buttonState;
		Material[] array;
		switch (buttonState)
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
			throw new SwitchExpressionException(buttonState);
		}
		buttonRenderer.sharedMaterials = array;
		this._button.delayTime = (float)((this._buttonState == FriendDisplay.ButtonState.Alert) ? 3 : 0);
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x00106CAC File Offset: 0x00104EAC
	public void Populate(FriendBackendController.Friend friend)
	{
		this.SetEmpty();
		if (friend != null && friend.Presence != null)
		{
			if (friend.Presence.UserName != null)
			{
				this.SetName(friend.Presence.UserName.ToUpper());
			}
			if (!string.IsNullOrEmpty(friend.Presence.RoomId) && friend.Presence.RoomId.Length > 0)
			{
				bool? isPublic = friend.Presence.IsPublic;
				bool flag = true;
				bool flag2 = (isPublic.GetValueOrDefault() == flag) & (isPublic != null);
				bool flag3 = friend.Presence.RoomId[0] == '@';
				bool flag4 = friend.Presence.RoomId.Equals(NetworkSystem.Instance.RoomName);
				bool flag5 = false;
				if (!flag4 && flag2 && !friend.Presence.Zone.IsNullOrEmpty())
				{
					string text = friend.Presence.Zone.ToLower();
					foreach (GTZone gtzone in ZoneManagement.instance.activeZones)
					{
						if (text.Contains(gtzone.GetName<GTZone>().ToLower()))
						{
							flag5 = true;
						}
					}
				}
				this.joinable = !flag3 && !flag4 && (!flag2 || flag5);
				if (flag3)
				{
					this.SetRoom(friend.Presence.RoomId.Substring(1).ToUpper());
					this.SetZone("CUSTOM");
				}
				else if (!flag2)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone("PRIVATE");
				}
				else if (friend.Presence.Zone != null)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone(friend.Presence.Zone.ToUpper());
				}
			}
			else
			{
				this.joinable = false;
				this.SetRoom("OFFLINE");
			}
			this.currentFriend = friend;
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x00106EC0 File Offset: 0x001050C0
	public void SetName(string friendName)
	{
		TMP_Text tmp_Text = this.nameText;
		this._friendName = friendName;
		tmp_Text.text = friendName;
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x00106EE4 File Offset: 0x001050E4
	public void SetRoom(string friendRoom)
	{
		TMP_Text tmp_Text = this.roomText;
		this._friendRoom = friendRoom;
		tmp_Text.text = friendRoom;
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x00106F08 File Offset: 0x00105108
	public void SetZone(string friendZone)
	{
		TMP_Text tmp_Text = this.zoneText;
		this._friendZone = friendZone;
		tmp_Text.text = friendZone;
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x00106F2C File Offset: 0x0010512C
	public void Randomize()
	{
		this.SetEmpty();
		int num = Random.Range(0, this.randomNames.Length);
		this.SetName(this.randomNames[num].ToUpper());
		this.SetRoom(string.Format("{0}{1}{2}{3}", new object[]
		{
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91)
		}));
		bool flag = Random.Range(0f, 1f) > 0.5f;
		this.joinable = flag && Random.Range(0f, 1f) > 0.5f;
		if (flag)
		{
			int num2 = Random.Range(0, 17);
			GTZone gtzone = (GTZone)num2;
			this.SetZone(gtzone.ToString().ToUpper());
		}
		else
		{
			this.SetZone(this.privateString);
		}
		this.UpdateComponentStates();
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x00107032 File Offset: 0x00105232
	public void SetEmpty()
	{
		this.SetName(this.emptyString);
		this.SetRoom(this.emptyString);
		this.SetZone(this.emptyString);
		this.joinable = false;
		this.currentFriend = null;
		this.UpdateComponentStates();
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x0010706C File Offset: 0x0010526C
	public void SetRemoveEnabled(bool enabled)
	{
		this.canRemove = enabled;
		this.UpdateComponentStates();
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x0010707C File Offset: 0x0010527C
	private void JoinButtonPressed()
	{
		if (this.joinable && this.currentFriend != null && this.currentFriend.Presence != null)
		{
			bool? isPublic = this.currentFriend.Presence.IsPublic;
			bool flag = true;
			JoinType joinType = (((isPublic.GetValueOrDefault() == flag) & (isPublic != null)) ? JoinType.FriendStationPublic : JoinType.FriendStationPrivate);
			GorillaComputer.instance.roomToJoin = this._friendRoom;
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this._friendRoom, joinType);
			this.joinable = false;
			this.UpdateComponentStates();
		}
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x00107104 File Offset: 0x00105304
	private void RemoveFriendButtonPressed()
	{
		if (this.friendDisplay.InRemoveMode)
		{
			FriendSystem.Instance.RemoveFriend(this.currentFriend, null);
			this.SetEmpty();
		}
	}

	// Token: 0x060036A1 RID: 13985 RVA: 0x0010712C File Offset: 0x0010532C
	private void OnDrawGizmosSelected()
	{
		float num = this.width * 0.5f * base.transform.lossyScale.x;
		float num2 = this.Height * 0.5f * base.transform.lossyScale.y;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = base.transform.position + base.transform.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = base.transform.position + base.transform.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = base.transform.position + base.transform.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = base.transform.position + base.transform.rotation * new Vector3(num3, -num4, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x00107260 File Offset: 0x00105460
	public void SetButton(GorillaPressableDelayButton friendCardButton, Material[] normalMaterials, Material[] activeMaterials, Material[] alertMaterials, TextMeshProUGUI buttonText)
	{
		this._button = friendCardButton;
		this._button.SetFillBar(this.removeProgressBar);
		this._button.onPressBegin += this.OnButtonPressBegin;
		this._button.onPressAbort += this.OnButtonPressAbort;
		this._button.onPressed += this.OnButtonPressed;
		this._buttonDefaultMaterials = normalMaterials;
		this._buttonActiveMaterials = activeMaterials;
		this._buttonAlertMaterials = alertMaterials;
		this._buttonText = buttonText;
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x001072EF File Offset: 0x001054EF
	private void OnRemoveFriendBegin()
	{
		this.nameText.text = "REMOVING";
		this.roomText.text = "FRIEND";
		this.zoneText.text = this.emptyString;
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x00107322 File Offset: 0x00105522
	private void OnRemoveFriendEnd()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x00107358 File Offset: 0x00105558
	private void OnButtonPressBegin()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendBegin();
			break;
		default:
			return;
		}
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x00107388 File Offset: 0x00105588
	private void OnButtonPressAbort()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendEnd();
			break;
		default:
			return;
		}
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x001073B8 File Offset: 0x001055B8
	private void OnButtonPressed(GorillaPressableButton button, bool isLeftHand)
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			break;
		case FriendDisplay.ButtonState.Active:
			this.JoinButtonPressed();
			return;
		case FriendDisplay.ButtonState.Alert:
			this.RemoveFriendButtonPressed();
			break;
		default:
			return;
		}
	}

	// Token: 0x04003C2B RID: 15403
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x04003C2C RID: 15404
	[SerializeField]
	private TextMeshProUGUI roomText;

	// Token: 0x04003C2D RID: 15405
	[SerializeField]
	private TextMeshProUGUI zoneText;

	// Token: 0x04003C2E RID: 15406
	[SerializeField]
	private Transform removeProgressBar;

	// Token: 0x04003C2F RID: 15407
	[SerializeField]
	private float width = 0.25f;

	// Token: 0x04003C31 RID: 15409
	private string emptyString = "";

	// Token: 0x04003C32 RID: 15410
	private string privateString = "PRIVATE";

	// Token: 0x04003C33 RID: 15411
	private bool joinable;

	// Token: 0x04003C34 RID: 15412
	private bool canRemove;

	// Token: 0x04003C35 RID: 15413
	private GorillaPressableDelayButton _button;

	// Token: 0x04003C36 RID: 15414
	private TextMeshProUGUI _buttonText;

	// Token: 0x04003C37 RID: 15415
	private string _friendName;

	// Token: 0x04003C38 RID: 15416
	private string _friendRoom;

	// Token: 0x04003C39 RID: 15417
	private string _friendZone;

	// Token: 0x04003C3A RID: 15418
	private FriendBackendController.Friend currentFriend;

	// Token: 0x04003C3B RID: 15419
	private FriendDisplay friendDisplay;

	// Token: 0x04003C3C RID: 15420
	private string[] randomNames = new string[]
	{
		"Veronica", "Roman", "Janiyah", "Dalton", "Bellamy", "Eithan", "Celeste", "Isaac", "Astrid", "Azariah",
		"Keilani", "Zeke", "Jayleen", "Yosef", "Jaylee", "Bodie", "Greta", "Cain", "Ella", "Everly",
		"Finnley", "Paisley", "Kaison", "Luna", "Nina", "Maison", "Monroe", "Ricardo", "Zariyah", "Travis",
		"Lacey", "Elian", "Frankie", "Otis", "Adele", "Edison", "Amira", "Ivan", "Raelynn", "Eliel",
		"Aliana", "Beckett", "Mylah", "Melvin", "Magdalena", "Leroy", "Madeleine"
	};

	// Token: 0x04003C3D RID: 15421
	private FriendDisplay.ButtonState _buttonState = (FriendDisplay.ButtonState)(-1);

	// Token: 0x04003C3E RID: 15422
	private Material[] _buttonDefaultMaterials;

	// Token: 0x04003C3F RID: 15423
	private Material[] _buttonActiveMaterials;

	// Token: 0x04003C40 RID: 15424
	private Material[] _buttonAlertMaterials;
}
