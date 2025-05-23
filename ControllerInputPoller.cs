using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x0200052C RID: 1324
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x1700033C RID: 828
	// (get) Token: 0x0600200C RID: 8204 RVA: 0x000A180A File Offset: 0x0009FA0A
	// (set) Token: 0x0600200D RID: 8205 RVA: 0x000A1812 File Offset: 0x0009FA12
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x0600200E RID: 8206 RVA: 0x000A181B File Offset: 0x0009FA1B
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000A1850 File Offset: 0x0009FA50
	public static void AddUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Add(callback);
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000A18B8 File Offset: 0x0009FAB8
	public static void RemoveUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Remove(callback);
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000A1924 File Offset: 0x0009FB24
	private void Update()
	{
		InputDevice inputDevice = this.leftControllerDevice;
		if (!this.leftControllerDevice.isValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			if (this.leftControllerDevice.isValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		InputDevice inputDevice2 = this.rightControllerDevice;
		if (!this.rightControllerDevice.isValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		InputDevice inputDevice3 = this.headDevice;
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		InputDevice inputDevice4 = this.leftControllerDevice;
		InputDevice inputDevice5 = this.rightControllerDevice;
		InputDevice inputDevice6 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.leftControllerRotation);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.leftControllerPrimary2DAxis);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.rightControllerPrimary2DAxis);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.headRotation);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, ref this.leftGrabMomentary, ref this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, ref this.rightGrabMomentary, ref this.rightGrabReleaseMomentary, 0.75f, 0.65f);
		}
		else if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, ref this.leftGrabMomentary, ref this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, ref this.rightGrabMomentary, ref this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
		if (this.didModifyOnUpdate)
		{
			List<Action> list = this.onUpdateNext;
			List<Action> list2 = this.onUpdate;
			this.onUpdate = list;
			this.onUpdateNext = list2;
			this.didModifyOnUpdate = false;
		}
		foreach (Action action in this.onUpdate)
		{
			action();
		}
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000A1DE4 File Offset: 0x0009FFE4
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, ref bool grabMomentary, ref bool grabReleaseMomentary, float grabThreshold, float grabReleaseThreshold)
	{
		bool flag = grabValue >= grabThreshold;
		bool flag2 = grabValue <= grabReleaseThreshold;
		grabMomentary = flag && flag != grab;
		grabReleaseMomentary = flag2 && flag2 != grabRelease;
		grab = flag;
		grabRelease = flag2;
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000A1E2B File Offset: 0x000A002B
	public static bool GetGrab(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x000A1E50 File Offset: 0x000A0050
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x000A1E75 File Offset: 0x000A0075
	public static bool GetGrabMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabMomentary;
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x000A1E9A File Offset: 0x000A009A
	public static bool GetGrabReleaseMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabReleaseMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabReleaseMomentary;
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000A1EBF File Offset: 0x000A00BF
	public static Vector2 Primary2DAxis(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimary2DAxis;
		}
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000A1EDE File Offset: 0x000A00DE
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000A1F03 File Offset: 0x000A0103
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000A1F28 File Offset: 0x000A0128
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000A1F4D File Offset: 0x000A014D
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x000A1F72 File Offset: 0x000A0172
	public static float GripFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x000A1F9B File Offset: 0x000A019B
	public static float TriggerFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000A1FC4 File Offset: 0x000A01C4
	public static float TriggerTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000A1FED File Offset: 0x000A01ED
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000A2027 File Offset: 0x000A0227
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000A2064 File Offset: 0x000A0264
	public static bool PositionValid(XRNode node)
	{
		if (node == XRNode.Head)
		{
			InputDevice inputDevice = ControllerInputPoller.instance.headDevice;
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == XRNode.LeftHand)
		{
			InputDevice inputDevice2 = ControllerInputPoller.instance.leftControllerDevice;
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		if (node == XRNode.RightHand)
		{
			InputDevice inputDevice3 = ControllerInputPoller.instance.rightControllerDevice;
			return ControllerInputPoller.instance.rightControllerDevice.isValid;
		}
		return false;
	}

	// Token: 0x0400240B RID: 9227
	[OnEnterPlay_SetNull]
	public static volatile ControllerInputPoller instance;

	// Token: 0x0400240C RID: 9228
	public float leftControllerIndexFloat;

	// Token: 0x0400240D RID: 9229
	public float leftControllerGripFloat;

	// Token: 0x0400240E RID: 9230
	public float rightControllerIndexFloat;

	// Token: 0x0400240F RID: 9231
	public float rightControllerGripFloat;

	// Token: 0x04002410 RID: 9232
	public float leftControllerIndexTouch;

	// Token: 0x04002411 RID: 9233
	public float rightControllerIndexTouch;

	// Token: 0x04002412 RID: 9234
	public float rightStickLRFloat;

	// Token: 0x04002413 RID: 9235
	public Vector3 leftControllerPosition;

	// Token: 0x04002414 RID: 9236
	public Vector3 rightControllerPosition;

	// Token: 0x04002415 RID: 9237
	public Vector3 headPosition;

	// Token: 0x04002416 RID: 9238
	public Quaternion leftControllerRotation;

	// Token: 0x04002417 RID: 9239
	public Quaternion rightControllerRotation;

	// Token: 0x04002418 RID: 9240
	public Quaternion headRotation;

	// Token: 0x04002419 RID: 9241
	public InputDevice leftControllerDevice;

	// Token: 0x0400241A RID: 9242
	public InputDevice rightControllerDevice;

	// Token: 0x0400241B RID: 9243
	public InputDevice headDevice;

	// Token: 0x0400241C RID: 9244
	public bool leftControllerPrimaryButton;

	// Token: 0x0400241D RID: 9245
	public bool leftControllerSecondaryButton;

	// Token: 0x0400241E RID: 9246
	public bool rightControllerPrimaryButton;

	// Token: 0x0400241F RID: 9247
	public bool rightControllerSecondaryButton;

	// Token: 0x04002420 RID: 9248
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x04002421 RID: 9249
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x04002422 RID: 9250
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x04002423 RID: 9251
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x04002424 RID: 9252
	public bool leftGrab;

	// Token: 0x04002425 RID: 9253
	public bool leftGrabRelease;

	// Token: 0x04002426 RID: 9254
	public bool rightGrab;

	// Token: 0x04002427 RID: 9255
	public bool rightGrabRelease;

	// Token: 0x04002428 RID: 9256
	public bool leftGrabMomentary;

	// Token: 0x04002429 RID: 9257
	public bool leftGrabReleaseMomentary;

	// Token: 0x0400242A RID: 9258
	public bool rightGrabMomentary;

	// Token: 0x0400242B RID: 9259
	public bool rightGrabReleaseMomentary;

	// Token: 0x0400242D RID: 9261
	public Vector2 leftControllerPrimary2DAxis;

	// Token: 0x0400242E RID: 9262
	public Vector2 rightControllerPrimary2DAxis;

	// Token: 0x0400242F RID: 9263
	private List<Action> onUpdate = new List<Action>();

	// Token: 0x04002430 RID: 9264
	private List<Action> onUpdateNext = new List<Action>();

	// Token: 0x04002431 RID: 9265
	private bool didModifyOnUpdate;
}
