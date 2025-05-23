using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020006F1 RID: 1777
public class GorillaFireballControllerManager : MonoBehaviour
{
	// Token: 0x06002C46 RID: 11334 RVA: 0x000DA16C File Offset: 0x000D836C
	private void Update()
	{
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			List<InputDevice> list = new List<InputDevice>();
			List<InputDevice> list2 = new List<InputDevice>();
			InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, list);
			InputDevices.GetDevicesAtXRNode(XRNode.RightHand, list2);
			if (list.Count == 1)
			{
				this.leftHand = list[0];
			}
			if (list2.Count == 1)
			{
				this.rightHand = list2[0];
			}
		}
		float num = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		if (this.leftHandLastState <= this.throwingThreshold && num > this.throwingThreshold)
		{
			this.CreateFireball(true);
		}
		else if (this.leftHandLastState >= this.throwingThreshold && num < this.throwingThreshold)
		{
			this.TryThrowFireball(true);
		}
		this.leftHandLastState = num;
		num = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		if (this.rightHandLastState <= this.throwingThreshold && num > this.throwingThreshold)
		{
			this.CreateFireball(false);
		}
		else if (this.rightHandLastState >= this.throwingThreshold && num < this.throwingThreshold)
		{
			this.TryThrowFireball(false);
		}
		this.rightHandLastState = num;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000DA270 File Offset: 0x000D8470
	public void TryThrowFireball(bool isLeftHand)
	{
		if (isLeftHand && GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
			return;
		}
		if (!isLeftHand && GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
		}
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x000DA2F0 File Offset: 0x000D84F0
	public void CreateFireball(bool isLeftHand)
	{
		object[] array = new object[1];
		Vector3 vector;
		if (isLeftHand)
		{
			array[0] = true;
			vector = GorillaPlaySpace.Instance.myVRRig.leftHandTransform.position;
		}
		else
		{
			array[0] = false;
			vector = GorillaPlaySpace.Instance.myVRRig.rightHandTransform.position;
		}
		PhotonNetwork.Instantiate("GorillaPrefabs/GorillaFireball", vector, Quaternion.identity, 0, array);
	}

	// Token: 0x0400326C RID: 12908
	public InputDevice leftHand;

	// Token: 0x0400326D RID: 12909
	public InputDevice rightHand;

	// Token: 0x0400326E RID: 12910
	public bool hasInitialized;

	// Token: 0x0400326F RID: 12911
	public float leftHandLastState;

	// Token: 0x04003270 RID: 12912
	public float rightHandLastState;

	// Token: 0x04003271 RID: 12913
	public float throwingThreshold = 0.9f;
}
