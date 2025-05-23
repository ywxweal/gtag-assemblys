using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005DD RID: 1501
internal class GorillaSerializerScene : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x17000372 RID: 882
	// (get) Token: 0x060024A6 RID: 9382 RVA: 0x000B83C1 File Offset: 0x000B65C1
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000B83D0 File Offset: 0x000B65D0
	protected virtual void Start()
	{
		if (!this.targetComponent.IsNull())
		{
			IGorillaSerializeableScene gorillaSerializeableScene = this.targetComponent as IGorillaSerializeableScene;
			if (gorillaSerializeableScene != null)
			{
				gorillaSerializeableScene.OnSceneLinking(this);
				this.serializeTarget = gorillaSerializeableScene;
				this.sceneSerializeTarget = gorillaSerializeableScene;
				this.successfullInstantiate = true;
				this.photonView.AddCallbackTarget(this);
				return;
			}
		}
		Debug.LogError("GorillaSerializerscene: missing target component or invalid target", base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x000B843E File Offset: 0x000B663E
	private void OnEnable()
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		if (!this.validDisable)
		{
			this.validDisable = true;
			return;
		}
		this.OnValidEnable();
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000B845F File Offset: 0x000B665F
	protected virtual void OnValidEnable()
	{
		this.sceneSerializeTarget.OnNetworkObjectEnable();
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000B846C File Offset: 0x000B666C
	private void OnDisable()
	{
		if (!this.successfullInstantiate || !this.validDisable)
		{
			return;
		}
		this.OnValidDisable();
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000B8485 File Offset: 0x000B6685
	protected virtual void OnValidDisable()
	{
		this.sceneSerializeTarget.OnNetworkObjectDisable();
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000B8494 File Offset: 0x000B6694
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		GorillaNot.instance.SendReport("bad net obj creation", info.Sender.UserId, info.Sender.NickName);
		if (info.photonView.IsMine)
		{
			PhotonNetwork.Destroy(info.photonView);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000B84EC File Offset: 0x000B66EC
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.validDisable = false;
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000B84F5 File Offset: 0x000B66F5
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		if (!this.transferrable)
		{
			return info.Sender == PhotonNetwork.MasterClient;
		}
		return base.ValidOnSerialize(stream, in info);
	}

	// Token: 0x040029B4 RID: 10676
	[SerializeField]
	private bool transferrable;

	// Token: 0x040029B5 RID: 10677
	[SerializeField]
	private MonoBehaviour targetComponent;

	// Token: 0x040029B6 RID: 10678
	private IGorillaSerializeableScene sceneSerializeTarget;

	// Token: 0x040029B7 RID: 10679
	protected bool validDisable = true;
}
