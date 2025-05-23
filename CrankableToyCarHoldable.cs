using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class CrankableToyCarHoldable : TransferrableObject
{
	// Token: 0x06000909 RID: 2313 RVA: 0x00030DE6 File Offset: 0x0002EFE6
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00030E08 File Offset: 0x0002F008
	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += this.OnDeployRPC;
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00030EF3 File Offset: 0x0002F0F3
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00030F14 File Offset: 0x0002F114
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00030F68 File Offset: 0x0002F168
	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float num = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? num2 = new float?(num);
				soundBankPlayer.Play(null, num2);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0003107C File Offset: 0x0002F27C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.rightHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(flag);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000311AB File Offset: 0x0002F3AB
	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000311D0 File Offset: 0x0002F3D0
	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float num = (float)args[3];
		float num2 = 10000f;
		if ((in vector).IsValid(in num2) && (in quaternion).IsValid())
		{
			float num3 = 10000f;
			if ((in vector2).IsValid(in num3))
			{
				this.DeployCarLocal(vector, quaternion, vector2, num, true);
				return;
			}
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00031285 File Offset: 0x0002F485
	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000312AD File Offset: 0x0002F4AD
	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	// Token: 0x04000AC6 RID: 2758
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	// Token: 0x04000AC7 RID: 2759
	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	// Token: 0x04000AC8 RID: 2760
	[SerializeField]
	private GameObject deployablePart;

	// Token: 0x04000AC9 RID: 2761
	[SerializeField]
	private GameObject disabledWhileDeployed;

	// Token: 0x04000ACA RID: 2762
	[SerializeField]
	private float crankAnglePerClick;

	// Token: 0x04000ACB RID: 2763
	[SerializeField]
	private float maxCrankStrength;

	// Token: 0x04000ACC RID: 2764
	[SerializeField]
	private float minClickPitch;

	// Token: 0x04000ACD RID: 2765
	[SerializeField]
	private float maxClickPitch;

	// Token: 0x04000ACE RID: 2766
	[SerializeField]
	private float minLifetime;

	// Token: 0x04000ACF RID: 2767
	[SerializeField]
	private float maxLifetime;

	// Token: 0x04000AD0 RID: 2768
	[SerializeField]
	private SoundBankPlayer clickSound;

	// Token: 0x04000AD1 RID: 2769
	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	// Token: 0x04000AD2 RID: 2770
	[SerializeField]
	private float crankHapticStrength = 0.1f;

	// Token: 0x04000AD3 RID: 2771
	[SerializeField]
	private float crankHapticDuration = 0.05f;

	// Token: 0x04000AD4 RID: 2772
	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	// Token: 0x04000AD5 RID: 2773
	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	// Token: 0x04000AD6 RID: 2774
	private float currentCrankStrength;

	// Token: 0x04000AD7 RID: 2775
	private float currentCrankClickAmount;

	// Token: 0x04000AD8 RID: 2776
	private RubberDuckEvents _events;
}
