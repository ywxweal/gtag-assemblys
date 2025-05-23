using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public class CoconutMystic : MonoBehaviour
{
	// Token: 0x06001896 RID: 6294 RVA: 0x00077494 File Offset: 0x00075694
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x000774A2 File Offset: 0x000756A2
	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this.OnPhotonEvent;
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x000774BA File Offset: 0x000756BA
	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.OnPhotonEvent;
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x000774D4 File Offset: 0x000756D4
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != CoconutMystic.kUpdateLabelEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
		if (player != owningNetPlayer)
		{
			return;
		}
		int num2 = (int)array[1];
		this.label.text = this.answers.GetItem(num2);
		this.soundPlayer.Play();
		this.breakEffect.Play();
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x00077574 File Offset: 0x00075774
	public void UpdateLabel()
	{
		bool flag = this.geodeItem.currentState == TransferrableObject.PositionState.InLeftHand;
		this.label.rectTransform.localRotation = Quaternion.Euler(0f, flag ? 270f : 90f, 0f);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x000775C0 File Offset: 0x000757C0
	public void ShowAnswer()
	{
		this.answers.distinct = this.distinct;
		this.label.text = this.answers.NextItem();
		this.soundPlayer.Play();
		this.breakEffect.Play();
		object obj = new object[]
		{
			CoconutMystic.kUpdateLabelEvent,
			this.answers.lastItemIndex
		};
		PhotonNetwork.RaiseEvent(176, obj, RaiseEventOptions.Default, SendOptions.SendReliable);
	}

	// Token: 0x04001B5D RID: 7005
	public VRRig rig;

	// Token: 0x04001B5E RID: 7006
	public GeodeItem geodeItem;

	// Token: 0x04001B5F RID: 7007
	public SoundBankPlayer soundPlayer;

	// Token: 0x04001B60 RID: 7008
	public ParticleSystem breakEffect;

	// Token: 0x04001B61 RID: 7009
	public RandomStrings answers;

	// Token: 0x04001B62 RID: 7010
	public TMP_Text label;

	// Token: 0x04001B63 RID: 7011
	public bool distinct;

	// Token: 0x04001B64 RID: 7012
	private static readonly int kUpdateLabelEvent = "CoconutMystic.UpdateLabel".GetStaticHash();
}
