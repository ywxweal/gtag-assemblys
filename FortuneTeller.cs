using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000554 RID: 1364
public class FortuneTeller : MonoBehaviourPunCallbacks
{
	// Token: 0x060020FD RID: 8445 RVA: 0x000A5BF4 File Offset: 0x000A3DF4
	private void Awake()
	{
		if (this.changeMaterialsInGreyZone && GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Combine(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Combine(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x000A5C68 File Offset: 0x000A3E68
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Remove(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Remove(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000A5CD4 File Offset: 0x000A3ED4
	public override void OnEnable()
	{
		base.OnEnable();
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		if (this.button)
		{
			this.button.onPressed += this.HandlePressedButton;
		}
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000A5D12 File Offset: 0x000A3F12
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.button)
		{
			this.button.onPressed -= this.HandlePressedButton;
		}
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x000A5D3E File Offset: 0x000A3F3E
	private void GreyZoneActivated()
	{
		this.boothRenderer.material = this.boothGreyZoneMaterial;
		this.beardRenderer.material = this.beardGreyZoneMaterial;
		this.tellerRenderer.SetMaterials(this.tellerGreyZoneMaterials);
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000A5D73 File Offset: 0x000A3F73
	private void GreyZoneDeactivated()
	{
		this.boothRenderer.material = this.boothDefaultMaterial;
		this.beardRenderer.material = this.beardDefaultMaterial;
		this.tellerRenderer.SetMaterials(this.tellerDefaultMaterials);
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000A5DA8 File Offset: 0x000A3FA8
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			base.photonView.RPC("TriggerUpdateFortuneRPC", newPlayer, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000A5E0C File Offset: 0x000A400C
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000A5E0C File Offset: 0x000A400C
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000A5E1B File Offset: 0x000A401B
	private void HandlePressedButton(GorillaPressableButton button, bool isLeft)
	{
		if (base.photonView.IsMine)
		{
			this.SendNewFortune();
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("RequestFortuneRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000A5E50 File Offset: 0x000A4050
	[PunRPC]
	private void RequestFortuneRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestFortune");
		RigContainer rigContainer;
		if (info.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			CallLimitType<CallLimiter> callLimitType = rigContainer.Rig.fxSettings.callSettings[(int)this.limiterType];
			if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
			{
				this.SendNewFortune();
			}
		}
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x000A5ED0 File Offset: 0x000A40D0
	private void SendNewFortune()
	{
		if (this.playable.time > 0.0 && this.playable.time < this.playable.duration)
		{
			return;
		}
		this.latestFortune = this.results.GetResult();
		this.UpdateFortune(this.latestFortune, true);
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("TriggerNewFortuneRPC", RpcTarget.Others, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x000A5F70 File Offset: 0x000A4170
	[PunRPC]
	private void TriggerUpdateFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerUpdateFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerUpdateFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerUpdateFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.UpdateFortune(this.latestFortune, false);
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x000A5FEC File Offset: 0x000A41EC
	[PunRPC]
	private void TriggerNewFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerNewFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerNewFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerNewFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		this.UpdateFortune(this.latestFortune, true);
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x000A6078 File Offset: 0x000A4278
	private void StartAttractModeMonitor()
	{
		if (this.attractModeMonitor == null)
		{
			this.attractModeMonitor = base.StartCoroutine(this.AttractModeMonitor());
		}
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x000A6094 File Offset: 0x000A4294
	private IEnumerator AttractModeMonitor()
	{
		while (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			if (Time.time >= this.nextAttractAnimTimestamp)
			{
				this.SendAttractAnim();
			}
			yield return new WaitForSeconds(this.nextAttractAnimTimestamp - Time.time);
		}
		this.attractModeMonitor = null;
		yield break;
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000A60A3 File Offset: 0x000A42A3
	private void SendAttractAnim()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			base.photonView.RPC("TriggerAttractAnimRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x000A60CC File Offset: 0x000A42CC
	[PunRPC]
	private void TriggerAttractAnimRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerAttractAnim");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerAttractAnim when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.animator.SetTrigger(this.trigger_attract);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000A6144 File Offset: 0x000A4344
	private void UpdateFortune(FortuneResults.FortuneResult result, bool newFortune)
	{
		if (this.results)
		{
			PlayableAsset resultFanfare = this.GetResultFanfare(result.fortuneType);
			if (resultFanfare)
			{
				this.playable.initialTime = (newFortune ? 0.0 : resultFanfare.duration);
				this.playable.Play(resultFanfare, DirectorWrapMode.Hold);
				this.animator.SetTrigger(this.trigger_prediction);
				this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
			}
		}
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000A61C7 File Offset: 0x000A43C7
	public void ApplyFortuneText()
	{
		this.text.text = this.results.GetResultText(this.latestFortune).ToUpper();
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000A61EC File Offset: 0x000A43EC
	private PlayableAsset GetResultFanfare(FortuneResults.FortuneCategoryType fortuneType)
	{
		foreach (FortuneTeller.FortuneTellerResultFanfare fortuneTellerResultFanfare in this.resultFanfares)
		{
			if (fortuneTellerResultFanfare.type == fortuneType)
			{
				return fortuneTellerResultFanfare.fanfare;
			}
		}
		return null;
	}

	// Token: 0x0400252D RID: 9517
	[SerializeField]
	private FXType limiterType;

	// Token: 0x0400252E RID: 9518
	[SerializeField]
	private FortuneTellerButton button;

	// Token: 0x0400252F RID: 9519
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x04002530 RID: 9520
	[SerializeField]
	private FortuneResults results;

	// Token: 0x04002531 RID: 9521
	[SerializeField]
	private PlayableDirector playable;

	// Token: 0x04002532 RID: 9522
	[SerializeField]
	private Animator animator;

	// Token: 0x04002533 RID: 9523
	[SerializeField]
	private float waitDurationBeforeAttractAnim;

	// Token: 0x04002534 RID: 9524
	[SerializeField]
	private FortuneTeller.FortuneTellerResultFanfare[] resultFanfares;

	// Token: 0x04002535 RID: 9525
	[Header("Grey Zone Visuals")]
	[SerializeField]
	private bool changeMaterialsInGreyZone;

	// Token: 0x04002536 RID: 9526
	[SerializeField]
	private MeshRenderer boothRenderer;

	// Token: 0x04002537 RID: 9527
	[SerializeField]
	private Material boothDefaultMaterial;

	// Token: 0x04002538 RID: 9528
	[SerializeField]
	private Material boothGreyZoneMaterial;

	// Token: 0x04002539 RID: 9529
	[SerializeField]
	private MeshRenderer beardRenderer;

	// Token: 0x0400253A RID: 9530
	[SerializeField]
	private Material beardDefaultMaterial;

	// Token: 0x0400253B RID: 9531
	[SerializeField]
	private Material beardGreyZoneMaterial;

	// Token: 0x0400253C RID: 9532
	[SerializeField]
	private SkinnedMeshRenderer tellerRenderer;

	// Token: 0x0400253D RID: 9533
	[SerializeField]
	private List<Material> tellerDefaultMaterials;

	// Token: 0x0400253E RID: 9534
	[SerializeField]
	private List<Material> tellerGreyZoneMaterials;

	// Token: 0x0400253F RID: 9535
	private FortuneResults.FortuneResult latestFortune;

	// Token: 0x04002540 RID: 9536
	private CallLimiter triggerNewFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x04002541 RID: 9537
	private CallLimiter triggerUpdateFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x04002542 RID: 9538
	private AnimHashId trigger_attract = "Attract";

	// Token: 0x04002543 RID: 9539
	private AnimHashId trigger_prediction = "Prediction";

	// Token: 0x04002544 RID: 9540
	private float nextAttractAnimTimestamp;

	// Token: 0x04002545 RID: 9541
	private Coroutine attractModeMonitor;

	// Token: 0x02000555 RID: 1365
	[Serializable]
	public struct FortuneTellerResultFanfare
	{
		// Token: 0x04002546 RID: 9542
		public FortuneResults.FortuneCategoryType type;

		// Token: 0x04002547 RID: 9543
		public PlayableAsset fanfare;
	}
}
