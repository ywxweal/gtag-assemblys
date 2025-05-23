using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004B8 RID: 1208
public class GameBallPlayerLocal : MonoBehaviour
{
	// Token: 0x06001D38 RID: 7480 RVA: 0x0008DE00 File Offset: 0x0008C000
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x0008DE4F File Offset: 0x0008C04F
	private void OnApplicationQuit()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0008DE5B File Offset: 0x0008C05B
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x0008DE4F File Offset: 0x0008C04F
	private void OnDestroy()
	{
		MonkeBallGame.Instance.OnPlayerDestroy();
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x0008DE6C File Offset: 0x0008C06C
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

	// Token: 0x06001D3D RID: 7485 RVA: 0x0008DEB0 File Offset: 0x0008C0B0
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion inputDataMotion = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out inputDataMotion.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out inputDataMotion.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out inputDataMotion.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out inputDataMotion.angVelocity);
		inputDataMotion.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(inputDataMotion);
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0008DF3C File Offset: 0x0008C13C
	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x0008DF80 File Offset: 0x0008C180
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x0008DFBD File Offset: 0x0008C1BD
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x0008DFCC File Offset: 0x0008C1CC
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x0008DFF4 File Offset: 0x0008C1F4
	private void UpdateStuckState()
	{
		bool flag = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				flag = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = flag;
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x0008E03C File Offset: 0x0008C23C
	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
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
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = (flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 vector = gameBall.transform.position;
				Vector3 vector2 = gameBall.transform.position - position;
				if (vector2.sqrMagnitude > num2 * num2)
				{
					vector = position + vector2.normalized * num2;
				}
				object obj2 = obj;
				Vector3 vector3 = obj2.InverseTransformPoint(vector);
				Quaternion quaternion = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, vector3, quaternion);
			}
		}
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x0008E1C8 File Offset: 0x0008C3C8
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion quaternion;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation * -(Quaternion.Inverse(quaternion) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float num = Vector3.Dot(vector3, vector);
				vector = vector3 * num;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x0008E380 File Offset: 0x0008C580
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x0008E388 File Offset: 0x0008C588
	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x0008DD4B File Offset: 0x0008BF4B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x0008DD51 File Offset: 0x0008BF51
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x0008E3BB File Offset: 0x0008C5BB
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x0008E3D7 File Offset: 0x0008C5D7
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x04002063 RID: 8291
	public GameBallPlayer gamePlayer;

	// Token: 0x04002064 RID: 8292
	private const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04002065 RID: 8293
	private GameBallPlayerLocal.HandData[] hands;

	// Token: 0x04002066 RID: 8294
	private GameBallPlayerLocal.InputData[] inputData;

	// Token: 0x04002067 RID: 8295
	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	// Token: 0x020004B9 RID: 1209
	private enum HandGrabState
	{
		// Token: 0x04002069 RID: 8297
		Empty,
		// Token: 0x0400206A RID: 8298
		Holding
	}

	// Token: 0x020004BA RID: 1210
	private struct HandData
	{
		// Token: 0x0400206B RID: 8299
		public GameBallPlayerLocal.HandGrabState grabState;

		// Token: 0x0400206C RID: 8300
		public bool gripWasHeld;

		// Token: 0x0400206D RID: 8301
		public double gripPressedTime;

		// Token: 0x0400206E RID: 8302
		public GameBallId grabbedGameBallId;
	}

	// Token: 0x020004BB RID: 1211
	public struct InputDataMotion
	{
		// Token: 0x0400206F RID: 8303
		public double time;

		// Token: 0x04002070 RID: 8304
		public Vector3 position;

		// Token: 0x04002071 RID: 8305
		public Quaternion rotation;

		// Token: 0x04002072 RID: 8306
		public Vector3 velocity;

		// Token: 0x04002073 RID: 8307
		public Vector3 angVelocity;
	}

	// Token: 0x020004BC RID: 1212
	public class InputData
	{
		// Token: 0x06001D4C RID: 7500 RVA: 0x0008E3F9 File Offset: 0x0008C5F9
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x0008E414 File Offset: 0x0008C614
		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x0008E444 File Offset: 0x0008C644
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x06001D4F RID: 7503 RVA: 0x0008E4C0 File Offset: 0x0008C6C0
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
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

		// Token: 0x04002074 RID: 8308
		public int maxInputs;

		// Token: 0x04002075 RID: 8309
		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
