using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000529 RID: 1321
internal class ConnectedControllerHandler : MonoBehaviour
{
	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001FF5 RID: 8181 RVA: 0x000A11D6 File Offset: 0x0009F3D6
	// (set) Token: 0x06001FF6 RID: 8182 RVA: 0x000A11DD File Offset: 0x0009F3DD
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001FF7 RID: 8183 RVA: 0x000A11E5 File Offset: 0x0009F3E5
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x000A11ED File Offset: 0x0009F3ED
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000A11F8 File Offset: 0x0009F3F8
	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		InputDevice deviceAtXRNode2 = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		Debug.Log(string.Format("right controller? {0}", (deviceAtXRNode.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)));
		this.rightControllerValid = deviceAtXRNode.isValid;
		this.leftControllerValid = deviceAtXRNode2.isValid;
		InputDevices.deviceConnected += this.DeviceConnected;
		InputDevices.deviceDisconnected += this.DeviceDisconnected;
		this.UpdateControllerStates();
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000A1320 File Offset: 0x0009F520
	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = GTPlayer.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x000A13A7 File Offset: 0x0009F5A7
	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000A13B6 File Offset: 0x0009F5B6
	private void OnDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x000A13C4 File Offset: 0x0009F5C4
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= this.DeviceConnected;
		InputDevices.deviceDisconnected -= this.DeviceDisconnected;
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x000A1413 File Offset: 0x0009F613
	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000A143B File Offset: 0x0009F63B
	private IEnumerator ControllerValidator()
	{
		yield return null;
		this.lastRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
		this.lastLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
		for (;;)
		{
			yield return new WaitForSeconds(this.overridePollRate);
			this.updateControllers = false;
			if (!this.playerHandler.inOverlay)
			{
				if (this.rightControllerValid)
				{
					this.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
					if (this.tempRightPos == this.lastRightPos)
					{
						if ((this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController)
						{
							this.overrideController |= OverrideControllers.RightController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.RightController) == OverrideControllers.RightController)
					{
						this.overrideController &= ~OverrideControllers.RightController;
						this.updateControllers = true;
					}
					this.lastRightPos = this.tempRightPos;
				}
				if (this.leftControllerValid)
				{
					this.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
					if (this.tempLeftPos == this.lastLeftPos)
					{
						if ((this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController)
						{
							this.overrideController |= OverrideControllers.LeftController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.LeftController) == OverrideControllers.LeftController)
					{
						this.overrideController &= ~OverrideControllers.LeftController;
						this.updateControllers = true;
					}
					this.lastLeftPos = this.tempLeftPos;
				}
				if (this.updateControllers)
				{
					this.overrideEnabled = this.overrideController > OverrideControllers.None;
					this.UpdateControllerStates();
				}
			}
		}
		yield break;
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000A144A File Offset: 0x0009F64A
	private void DeviceDisconnected(InputDevice device)
	{
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right))
		{
			this.rightControllerValid = false;
		}
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left))
		{
			this.leftControllerValid = false;
		}
		this.UpdateControllerStates();
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000A1488 File Offset: 0x0009F688
	private void DeviceConnected(InputDevice device)
	{
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right))
		{
			this.rightControllerValid = true;
		}
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left))
		{
			this.leftControllerValid = true;
		}
		this.UpdateControllerStates();
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000A14C8 File Offset: 0x0009F6C8
	private void UpdateControllerStates()
	{
		if (this.overrideEnabled && this.overrideController != OverrideControllers.None)
		{
			this.rightValid = this.rightControllerValid && (this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController;
			this.leftValid = this.leftControllerValid && (this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController;
		}
		else
		{
			this.rightValid = this.rightControllerValid;
			this.leftValid = this.leftControllerValid;
		}
		this.rightXRController.enabled = this.rightValid;
		this.leftXRController.enabled = this.leftValid;
		this.AssignSnapturnController();
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000A1568 File Offset: 0x0009F768
	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000A15CC File Offset: 0x0009F7CC
	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool flag;
		if (controllerNode != XRNode.LeftHand)
		{
			flag = controllerNode != XRNode.RightHand || this.rightValid;
		}
		else
		{
			flag = this.leftValid;
		}
		return flag;
	}

	// Token: 0x040023EF RID: 9199
	[SerializeField]
	private HandTransformFollowOffest rightHandFollower;

	// Token: 0x040023F0 RID: 9200
	[SerializeField]
	private HandTransformFollowOffest leftHandFollower;

	// Token: 0x040023F1 RID: 9201
	[SerializeField]
	private XRController rightXRController;

	// Token: 0x040023F2 RID: 9202
	[SerializeField]
	private XRController leftXRController;

	// Token: 0x040023F3 RID: 9203
	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	// Token: 0x040023F4 RID: 9204
	private List<XRController> rightControllerList;

	// Token: 0x040023F5 RID: 9205
	private List<XRController> leftcontrollerList;

	// Token: 0x040023F6 RID: 9206
	private const InputDeviceCharacteristics rightCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;

	// Token: 0x040023F7 RID: 9207
	private const InputDeviceCharacteristics leftCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;

	// Token: 0x040023F8 RID: 9208
	private bool rightControllerValid = true;

	// Token: 0x040023F9 RID: 9209
	private bool leftControllerValid = true;

	// Token: 0x040023FA RID: 9210
	[SerializeField]
	private bool rightValid = true;

	// Token: 0x040023FB RID: 9211
	[SerializeField]
	private bool leftValid = true;

	// Token: 0x040023FC RID: 9212
	[SerializeField]
	private Vector3 lastRightPos;

	// Token: 0x040023FD RID: 9213
	[SerializeField]
	private Vector3 lastLeftPos;

	// Token: 0x040023FE RID: 9214
	private Vector3 tempRightPos;

	// Token: 0x040023FF RID: 9215
	private Vector3 tempLeftPos;

	// Token: 0x04002400 RID: 9216
	private bool updateControllers;

	// Token: 0x04002401 RID: 9217
	private GTPlayer playerHandler;

	// Token: 0x04002402 RID: 9218
	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float overridePollRate = 15f;

	// Token: 0x04002403 RID: 9219
	[SerializeField]
	private bool overrideEnabled;

	// Token: 0x04002404 RID: 9220
	[SerializeField]
	private OverrideControllers overrideController;
}
