using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000260 RID: 608
[NetworkBehaviourWeaved(1)]
public class LckSocialCamera : NetworkComponent, IGorillaSliceableSimple
{
	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x00047781 File Offset: 0x00045981
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe ref LckSocialCamera.CameraData _networkedData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing LckSocialCamera._networkedData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ref *(LckSocialCamera.CameraData*)(this.Ptr + 0);
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x000477A6 File Offset: 0x000459A6
	// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x000477B4 File Offset: 0x000459B4
	private LckSocialCamera.CameraState currentState
	{
		get
		{
			return this._localData.currentState;
		}
		set
		{
			this._localData.currentState = value;
			if (base.IsLocallyOwned)
			{
				this.CoconutCamera.SetVisualsActive(false);
				this.CoconutCamera.SetRecordingState(false);
				return;
			}
			this.CoconutCamera.SetVisualsActive(this.visible);
			this.CoconutCamera.SetRecordingState(this.recording);
		}
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00047810 File Offset: 0x00045A10
	private static bool GetFlag(LckSocialCamera.CameraState cameraState, LckSocialCamera.CameraState flag)
	{
		return (cameraState & flag) == flag;
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00047818 File Offset: 0x00045A18
	private static LckSocialCamera.CameraState SetFlag(LckSocialCamera.CameraState cameraState, LckSocialCamera.CameraState flag, bool value)
	{
		if (value)
		{
			cameraState |= flag;
		}
		else
		{
			cameraState &= ~flag;
		}
		return cameraState;
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x0004782B File Offset: 0x00045A2B
	// (set) Token: 0x06000DE9 RID: 3561 RVA: 0x00047839 File Offset: 0x00045A39
	public bool visible
	{
		get
		{
			return LckSocialCamera.GetFlag(this.currentState, LckSocialCamera.CameraState.Visible);
		}
		set
		{
			this.currentState = LckSocialCamera.SetFlag(this.currentState, LckSocialCamera.CameraState.Visible, value);
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000DEA RID: 3562 RVA: 0x0004784E File Offset: 0x00045A4E
	// (set) Token: 0x06000DEB RID: 3563 RVA: 0x0004785C File Offset: 0x00045A5C
	public bool recording
	{
		get
		{
			return LckSocialCamera.GetFlag(this.currentState, LckSocialCamera.CameraState.Recording);
		}
		set
		{
			this.currentState = LckSocialCamera.SetFlag(this.currentState, LckSocialCamera.CameraState.Recording, value);
		}
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00047871 File Offset: 0x00045A71
	public unsafe override void WriteDataFusion()
	{
		*this._networkedData = new LckSocialCamera.CameraData(this._localData.currentState);
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0004788E File Offset: 0x00045A8E
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this._networkedData.currentState);
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x000478A1 File Offset: 0x00045AA1
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.currentState);
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x000478B4 File Offset: 0x00045AB4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		LckSocialCamera.CameraState cameraState = (LckSocialCamera.CameraState)stream.ReceiveNext();
		this.ReadDataShared(cameraState);
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x000478E8 File Offset: 0x00045AE8
	protected override void Start()
	{
		base.Start();
		this.visible = this.visible;
		if (base.IsLocallyOwned)
		{
			this.StoreRigReference();
			LckSocialCameraManager instance = LckSocialCameraManager.Instance;
			if (instance != null)
			{
				instance.SetLckSocialCamera(this);
				return;
			}
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Combine(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0004794C File Offset: 0x00045B4C
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
		if (!info.photonView.IsMine)
		{
			this.StoreRigReference();
		}
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x00047968 File Offset: 0x00045B68
	private void StoreRigReference()
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(base.Owner, out rigContainer))
		{
			this._vrrig = rigContainer.Rig;
		}
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00047998 File Offset: 0x00045B98
	public void SliceUpdate()
	{
		if (this._vrrig == null)
		{
			this.StoreRigReference();
			if (this._vrrig.IsNull())
			{
				base.enabled = false;
				return;
			}
		}
		else
		{
			this.CoconutCamera.transform.localScale = Vector3.one * this._vrrig.scaleFactor;
		}
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x000479F3 File Offset: 0x00045BF3
	public new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00047A08 File Offset: 0x00045C08
	public new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00047A1D File Offset: 0x00045C1D
	private void OnManagerSpawned(LckSocialCameraManager manager)
	{
		manager.SetLckSocialCamera(this);
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00047A26 File Offset: 0x00045C26
	private void ReadDataShared(LckSocialCamera.CameraState newState)
	{
		this.currentState = newState;
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00047A2F File Offset: 0x00045C2F
	[WeaverGenerated]
	public unsafe override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		*this._networkedData = this.__networkedData;
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x00047A4C File Offset: 0x00045C4C
	[WeaverGenerated]
	public unsafe override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this.__networkedData = *this._networkedData;
	}

	// Token: 0x0400115A RID: 4442
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x0400115B RID: 4443
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x0400115C RID: 4444
	[SerializeField]
	private List<GameObject> _visualObjects;

	// Token: 0x0400115D RID: 4445
	[SerializeField]
	private VRRig _vrrig;

	// Token: 0x0400115E RID: 4446
	private LckSocialCamera.CameraDataLocal _localData;

	// Token: 0x0400115F RID: 4447
	[WeaverGenerated]
	[DefaultForProperty("_networkedData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private LckSocialCamera.CameraData __networkedData;

	// Token: 0x02000261 RID: 609
	private enum CameraState
	{
		// Token: 0x04001161 RID: 4449
		Empty,
		// Token: 0x04001162 RID: 4450
		Visible,
		// Token: 0x04001163 RID: 4451
		Recording
	}

	// Token: 0x02000262 RID: 610
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct CameraData : INetworkStruct
	{
		// Token: 0x06000DFC RID: 3580 RVA: 0x00047A65 File Offset: 0x00045C65
		public CameraData(LckSocialCamera.CameraState currentState)
		{
			this.currentState = currentState;
		}

		// Token: 0x04001164 RID: 4452
		[FieldOffset(0)]
		public LckSocialCamera.CameraState currentState;
	}

	// Token: 0x02000263 RID: 611
	private struct CameraDataLocal
	{
		// Token: 0x04001165 RID: 4453
		public LckSocialCamera.CameraState currentState;
	}
}
