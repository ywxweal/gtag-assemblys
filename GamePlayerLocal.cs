using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000576 RID: 1398
public class GamePlayerLocal : MonoBehaviour
{
	// Token: 0x06002231 RID: 8753 RVA: 0x000AB0E8 File Offset: 0x000A92E8
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000AB138 File Offset: 0x000A9338
	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000AB17C File Offset: 0x000A937C
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion inputDataMotion = default(GamePlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out inputDataMotion.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out inputDataMotion.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out inputDataMotion.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out inputDataMotion.angVelocity);
		inputDataMotion.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(inputDataMotion);
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000AB208 File Offset: 0x000A9408
	private void UpdateHand(int handIndex)
	{
		if (!this.gamePlayer.GetGameEntityId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000AB23C File Offset: 0x000A943C
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		if (handIndex == 1)
		{
			EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
		}
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000AB2BC File Offset: 0x000A94BC
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000AB2F5 File Offset: 0x000A94F5
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000AB304 File Offset: 0x000A9504
	private void UpdateStuckState()
	{
		bool flag = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameEntityId(i).IsValid())
			{
				flag = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = flag;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000AB34C File Offset: 0x000A954C
	private void UpdateHandEmpty(int handIndex)
	{
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Transform handTransform = this.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Quaternion rotation = handTransform.rotation;
			GameEntityId gameEntityId = GameEntityManager.instance.TryGrabLocal(position);
			if (gameEntityId.IsValid())
			{
				bool flag2 = GamePlayerLocal.IsLeftHand(handIndex);
				Transform handTransform2 = this.GetHandTransform(handIndex);
				GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(gameEntityId);
				Vector3 vector = gameEntity.transform.position;
				Quaternion quaternion = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				if (component != null)
				{
					GameGrab gameGrab;
					component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab);
					vector = gameGrab.position;
					quaternion = gameGrab.rotation;
				}
				Vector3 vector2 = handTransform2.InverseTransformPoint(vector);
				Quaternion quaternion2 = Quaternion.Inverse(handTransform2.rotation) * quaternion;
				handTransform2.InverseTransformPoint(gameEntity.transform.position);
				GameEntityManager.instance.RequestGrabEntity(gameEntityId, flag2, vector2, quaternion2);
			}
		}
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000AB4B8 File Offset: 0x000A96B8
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (this.gamePlayer.IsGrabbingDisabled() || ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion quaternion;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation * -(Quaternion.Inverse(quaternion) * vector);
			GameEntityId gameEntityId = this.gamePlayer.GetGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameEntityManager.instance.RequestThrowEntity(gameEntityId, GamePlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x0008E3A0 File Offset: 0x0008C5A0
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000AB5F3 File Offset: 0x000A97F3
	private Transform GetHandTransform(int handIndex)
	{
		return GamePlayer.GetHandTransform(GorillaTagger.Instance.offlineVRRig, handIndex);
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000AB608 File Offset: 0x000A9808
	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000AB67F File Offset: 0x000A987F
	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x0008DD6B File Offset: 0x0008BF6B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x0008DD71 File Offset: 0x0008BF71
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x0008E3DB File Offset: 0x0008C5DB
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x0008E3F7 File Offset: 0x0008C5F7
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x04002636 RID: 9782
	public GamePlayer gamePlayer;

	// Token: 0x04002637 RID: 9783
	private GamePlayerLocal.HandData[] hands;

	// Token: 0x04002638 RID: 9784
	public const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04002639 RID: 9785
	private GamePlayerLocal.InputData[] inputData;

	// Token: 0x0400263A RID: 9786
	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	// Token: 0x02000577 RID: 1399
	private enum HandGrabState
	{
		// Token: 0x0400263C RID: 9788
		Empty,
		// Token: 0x0400263D RID: 9789
		Holding
	}

	// Token: 0x02000578 RID: 1400
	private struct HandData
	{
		// Token: 0x0400263E RID: 9790
		public GamePlayerLocal.HandGrabState grabState;

		// Token: 0x0400263F RID: 9791
		public bool gripWasHeld;

		// Token: 0x04002640 RID: 9792
		public double gripPressedTime;

		// Token: 0x04002641 RID: 9793
		public GameEntityId grabbedGameBallId;
	}

	// Token: 0x02000579 RID: 1401
	public struct InputDataMotion
	{
		// Token: 0x04002642 RID: 9794
		public double time;

		// Token: 0x04002643 RID: 9795
		public Vector3 position;

		// Token: 0x04002644 RID: 9796
		public Quaternion rotation;

		// Token: 0x04002645 RID: 9797
		public Vector3 velocity;

		// Token: 0x04002646 RID: 9798
		public Vector3 angVelocity;
	}

	// Token: 0x0200057A RID: 1402
	public class InputData
	{
		// Token: 0x06002244 RID: 8772 RVA: 0x000AB698 File Offset: 0x000A9898
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x000AB6B3 File Offset: 0x000A98B3
		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x000AB6E0 File Offset: 0x000A98E0
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x000AB75C File Offset: 0x000A995C
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					vector += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return vector / (float)num3;
		}

		// Token: 0x04002647 RID: 9799
		public int maxInputs;

		// Token: 0x04002648 RID: 9800
		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
