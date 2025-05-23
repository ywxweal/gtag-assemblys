using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x0200083E RID: 2110
public class ControllerBehaviour : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000528 RID: 1320
	// (get) Token: 0x06003377 RID: 13175 RVA: 0x000FDFDE File Offset: 0x000FC1DE
	public bool ButtonDown
	{
		get
		{
			return this.buttonDown;
		}
	}

	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x06003378 RID: 13176 RVA: 0x000FDFE6 File Offset: 0x000FC1E6
	public bool LeftButtonDown
	{
		get
		{
			return this.leftButtonDown;
		}
	}

	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x06003379 RID: 13177 RVA: 0x000FDFEE File Offset: 0x000FC1EE
	public bool RightButtonDown
	{
		get
		{
			return this.rightButtonDown;
		}
	}

	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x0600337A RID: 13178 RVA: 0x000FDFF6 File Offset: 0x000FC1F6
	public bool IsLeftStick
	{
		get
		{
			return this.isLeftStick;
		}
	}

	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x0600337B RID: 13179 RVA: 0x000FDFFE File Offset: 0x000FC1FE
	public bool IsRightStick
	{
		get
		{
			return this.isRightStick;
		}
	}

	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x0600337C RID: 13180 RVA: 0x000FE006 File Offset: 0x000FC206
	public bool IsUpStick
	{
		get
		{
			return this.isUpStick;
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x0600337D RID: 13181 RVA: 0x000FE00E File Offset: 0x000FC20E
	public bool IsDownStick
	{
		get
		{
			return this.isDownStick;
		}
	}

	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x0600337E RID: 13182 RVA: 0x000FE016 File Offset: 0x000FC216
	public float StickXValue
	{
		get
		{
			return this.stickXValue;
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x0600337F RID: 13183 RVA: 0x000FE01E File Offset: 0x000FC21E
	public float StickYValue
	{
		get
		{
			return this.stickYValue;
		}
	}

	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x06003380 RID: 13184 RVA: 0x000FE026 File Offset: 0x000FC226
	// (set) Token: 0x06003381 RID: 13185 RVA: 0x000FE02E File Offset: 0x000FC22E
	public bool TriggerDown { get; private set; }

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06003382 RID: 13186 RVA: 0x000FE038 File Offset: 0x000FC238
	// (remove) Token: 0x06003383 RID: 13187 RVA: 0x000FE070 File Offset: 0x000FC270
	public event ControllerBehaviour.OnActionEvent OnAction;

	// Token: 0x06003384 RID: 13188 RVA: 0x000FE0A8 File Offset: 0x000FC2A8
	private void Update()
	{
		this.leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		Vector2 axis;
		this.leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
		bool state;
		this.leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state);
		bool state2;
		this.leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state2);
		bool state3;
		this.leftHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out state3);
		this.rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		Vector2 axis2;
		this.rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis2);
		bool state4;
		this.rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state4);
		bool state5;
		this.rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state5);
		bool state6;
		this.rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out state6);
		axis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
		state = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		state2 = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		state3 = SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(SteamVR_Input_Sources.LeftHand);
		axis2 = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		state4 = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		state5 = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		state6 = SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(SteamVR_Input_Sources.RightHand);
		this.buttonDown = state || state4 || state2 || state5;
		this.leftButtonDown = state || state2 || state3;
		this.rightButtonDown = state4 || state5 || state6;
		bool flag;
		bool flag2;
		if (this.triggersAsSticks)
		{
			flag = Mathf.Min(axis.x, axis2.x) < -this.uxSettings.StickSensitvity || state3;
			flag2 = Mathf.Max(axis.x, axis2.x) > this.uxSettings.StickSensitvity || state6;
		}
		else
		{
			flag = Mathf.Min(axis.x, axis2.x) < -this.uxSettings.StickSensitvity;
			flag2 = Mathf.Max(axis.x, axis2.x) > this.uxSettings.StickSensitvity;
		}
		bool flag3 = Mathf.Max(axis.y, axis2.y) > this.uxSettings.StickSensitvity;
		bool flag4 = Mathf.Min(axis.y, axis2.y) < -this.uxSettings.StickSensitvity;
		bool flag5 = (this.isLeftStick && flag) || (this.IsRightStick && flag2) || (this.isUpStick && flag3) || (this.isDownStick && flag4);
		this.TriggerDown = state3 || state6;
		if (Mathf.Abs(axis.x) > Mathf.Abs(axis2.x))
		{
			this.stickXValue = axis.x;
		}
		else
		{
			this.stickXValue = axis2.x;
		}
		if (Mathf.Abs(axis.y) > Mathf.Abs(axis2.y))
		{
			this.stickYValue = axis.y;
		}
		else
		{
			this.stickYValue = axis2.y;
		}
		if (Time.time - this.actionTime < this.actionDelay / this.repeatAction)
		{
			return;
		}
		if (flag5)
		{
			this.repeatAction += this.actionRepeatDelayReduction;
		}
		else
		{
			this.repeatAction = 1f;
		}
		this.isLeftStick = flag;
		this.isRightStick = flag2;
		this.isUpStick = flag3;
		this.isDownStick = flag4;
		if (this.isLeftStick || this.isRightStick || this.isUpStick || this.isDownStick || this.buttonDown)
		{
			this.actionTime = Time.time;
		}
		if (this.OnAction != null)
		{
			this.OnAction();
		}
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x000FE410 File Offset: 0x000FC610
	private void OnDisable()
	{
		this.buttonDown = (this.isLeftStick = (this.isRightStick = (this.isUpStick = (this.isDownStick = false))));
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x000FE448 File Offset: 0x000FC648
	public bool BuildValidationCheck()
	{
		if (this.uxSettings == null)
		{
			Debug.LogError("ControllerBehaviour must set UXSettings");
			return false;
		}
		return true;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x000FE465 File Offset: 0x000FC665
	public static ControllerBehaviour CreateNewControllerBehaviour(GameObject gameObject, UXSettings settings)
	{
		ControllerBehaviour controllerBehaviour = gameObject.AddComponent<ControllerBehaviour>();
		controllerBehaviour.uxSettings = settings;
		return controllerBehaviour;
	}

	// Token: 0x04003A65 RID: 14949
	private InputDevice leftHandDevice;

	// Token: 0x04003A66 RID: 14950
	private InputDevice rightHandDevice;

	// Token: 0x04003A67 RID: 14951
	private float actionTime;

	// Token: 0x04003A68 RID: 14952
	private bool buttonDown;

	// Token: 0x04003A69 RID: 14953
	private bool leftButtonDown;

	// Token: 0x04003A6A RID: 14954
	private bool rightButtonDown;

	// Token: 0x04003A6B RID: 14955
	private float repeatAction = 1f;

	// Token: 0x04003A6C RID: 14956
	private bool isLeftStick;

	// Token: 0x04003A6D RID: 14957
	private bool isRightStick;

	// Token: 0x04003A6E RID: 14958
	private bool isUpStick;

	// Token: 0x04003A6F RID: 14959
	private bool isDownStick;

	// Token: 0x04003A70 RID: 14960
	private float stickXValue;

	// Token: 0x04003A71 RID: 14961
	private float stickYValue;

	// Token: 0x04003A72 RID: 14962
	[SerializeField]
	private UXSettings uxSettings;

	// Token: 0x04003A73 RID: 14963
	[SerializeField]
	private float actionDelay = 0.5f;

	// Token: 0x04003A74 RID: 14964
	[SerializeField]
	private float actionRepeatDelayReduction = 0.5f;

	// Token: 0x04003A75 RID: 14965
	[Tooltip("Should the triggers modify the x axis like the sticks do?")]
	[SerializeField]
	private bool triggersAsSticks = true;

	// Token: 0x0200083F RID: 2111
	// (Invoke) Token: 0x0600338A RID: 13194
	public delegate void OnActionEvent();
}
