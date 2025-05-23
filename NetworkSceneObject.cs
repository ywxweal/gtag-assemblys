using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002A6 RID: 678
[RequireComponent(typeof(PhotonView))]
public class NetworkSceneObject : SimulationBehaviour
{
	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0004F150 File Offset: 0x0004D350
	public bool IsMine
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x0004F15D File Offset: 0x0004D35D
	protected virtual void Start()
	{
		if (this.photonView == null)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x0004F179 File Offset: 0x0004D379
	protected virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0004F181 File Offset: 0x0004D381
	protected virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0004F18C File Offset: 0x0004D38C
	private void RegisterOnRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.AddGlobal(this);
		}
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0004F1C4 File Offset: 0x0004D3C4
	private void RemoveFromRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.RemoveGlobal(this);
		}
	}

	// Token: 0x040012A8 RID: 4776
	public PhotonView photonView;
}
