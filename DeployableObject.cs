using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200016B RID: 363
public class DeployableObject : TransferrableObject
{
	// Token: 0x0600091D RID: 2333 RVA: 0x0003169F File Offset: 0x0002F89F
	protected override void Awake()
	{
		this._deploySignal.OnSignal += this.DeployRPC;
		base.Awake();
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000316C0 File Offset: 0x0002F8C0
	internal override void OnEnable()
	{
		this._deploySignal.Enable();
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		for (int i = 0; i < this._rigAwareObjects.Length; i++)
		{
			IRigAware rigAware = this._rigAwareObjects[i] as IRigAware;
			if (rigAware != null)
			{
				rigAware.SetRig(componentInParent);
			}
		}
		this.m_VRRig = componentInParent;
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00031734 File Offset: 0x0002F934
	internal override void OnDisable()
	{
		this.m_VRRig = null;
		this._deploySignal.Disable();
		if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
		base.OnDisable();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00031761 File Offset: 0x0002F961
	protected override void OnDestroy()
	{
		this._deploySignal.Dispose();
		base.OnDestroy();
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00031774 File Offset: 0x0002F974
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this._objectToDeploy.activeSelf)
			{
				this.DeployChild();
				return;
			}
		}
		else if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000317C8 File Offset: 0x0002F9C8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRig.LocalRig != this.ownerRig)
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.rightHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(flag);
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(Vector3.zero);
		Quaternion rotation = transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		this.DeployLocal(vector, rotation, averageVelocity, false);
		this._deploySignal.Raise(ReceiverGroup.Others, BitPackUtils.PackWorldPosForNetwork(vector), BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f));
		return true;
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0003186B File Offset: 0x0002FA6B
	protected virtual void DeployLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this.DisableWhileDeployed(true);
		this._child.Deploy(this, launchPos, launchRot, releaseVel, isRemote);
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00031888 File Offset: 0x0002FA88
	private void DeployRPC(long packedPos, int packedRot, long packedVel, PhotonSignalInfo info)
	{
		if (info.sender != base.OwningPlayer())
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "DeployRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos);
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRot);
		Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork(packedVel) / 100f;
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || !this.m_VRRig.IsPositionInRange(vector, this._maxDeployDistance))
		{
			return;
		}
		this.DeployLocal(vector, quaternion, this.m_VRRig.ClampVelocityRelativeToPlayerSafe(vector2, this._maxThrowVelocity), true);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00031920 File Offset: 0x0002FB20
	private void DisableWhileDeployed(bool active)
	{
		if (this._disabledWhileDeployed.IsNullOrEmpty<GameObject>())
		{
			return;
		}
		for (int i = 0; i < this._disabledWhileDeployed.Length; i++)
		{
			this._disabledWhileDeployed[i].SetActive(!active);
		}
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0003195F File Offset: 0x0002FB5F
	public void DeployChild()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this._objectToDeploy.SetActive(true);
		this.DisableWhileDeployed(true);
		UnityEvent onDeploy = this._onDeploy;
		if (onDeploy == null)
		{
			return;
		}
		onDeploy.Invoke();
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00031992 File Offset: 0x0002FB92
	public void ReturnChild()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this._objectToDeploy.SetActive(false);
		this.DisableWhileDeployed(false);
		UnityEvent onReturn = this._onReturn;
		if (onReturn == null)
		{
			return;
		}
		onReturn.Invoke();
	}

	// Token: 0x04000AEA RID: 2794
	[SerializeField]
	private GameObject _objectToDeploy;

	// Token: 0x04000AEB RID: 2795
	[SerializeField]
	private DeployedChild _child;

	// Token: 0x04000AEC RID: 2796
	[SerializeField]
	private GameObject[] _disabledWhileDeployed = new GameObject[0];

	// Token: 0x04000AED RID: 2797
	[SerializeField]
	private SoundBankPlayer deploySound;

	// Token: 0x04000AEE RID: 2798
	[SerializeField]
	private PhotonSignal<long, int, long> _deploySignal = "_deploySignal";

	// Token: 0x04000AEF RID: 2799
	[SerializeField]
	private float _maxDeployDistance = 4f;

	// Token: 0x04000AF0 RID: 2800
	[SerializeField]
	private float _maxThrowVelocity = 50f;

	// Token: 0x04000AF1 RID: 2801
	[SerializeField]
	private UnityEvent _onDeploy;

	// Token: 0x04000AF2 RID: 2802
	[SerializeField]
	private UnityEvent _onReturn;

	// Token: 0x04000AF3 RID: 2803
	[SerializeField]
	private Component[] _rigAwareObjects = new Component[0];

	// Token: 0x04000AF4 RID: 2804
	private VRRig m_VRRig;
}
