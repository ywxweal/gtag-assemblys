using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004A7 RID: 1191
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x06001CC6 RID: 7366 RVA: 0x0008BD5F File Offset: 0x00089F5F
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x0008BD80 File Offset: 0x00089F80
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice inputDevice in list)
		{
			this.InputDevices_deviceConnected(inputDevice);
		}
		InputDevices.deviceConnected += this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected += this.InputDevices_deviceDisconnected;
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x0008BDFC File Offset: 0x00089FFC
	private void OnDisable()
	{
		InputDevices.deviceConnected -= this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected -= this.InputDevices_deviceDisconnected;
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x0008BE2C File Offset: 0x0008A02C
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x0008BE55 File Offset: 0x0008A055
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x0008BE74 File Offset: 0x0008A074
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out flag2) && flag2) || flag;
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x04002006 RID: 8198
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x04002007 RID: 8199
	private bool lastButtonState;

	// Token: 0x04002008 RID: 8200
	private List<InputDevice> devicesWithPrimaryButton;
}
