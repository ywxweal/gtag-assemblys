using System;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200031F RID: 799
public class AppDeeplinkUI : MonoBehaviour
{
	// Token: 0x060012F3 RID: 4851 RVA: 0x00059ED8 File Offset: 0x000580D8
	private void Start()
	{
		DebugUIBuilder instance = DebugUIBuilder.instance;
		this.uiLaunchType = instance.AddLabel("UnityDeeplinkSample", 0);
		instance.AddDivider(0);
		instance.AddButton("launch OtherApp", new DebugUIBuilder.OnClick(this.LaunchOtherApp), -1, 0, false);
		instance.AddButton("launch UnrealDeeplinkSample", new DebugUIBuilder.OnClick(this.LaunchUnrealDeeplinkSample), -1, 0, false);
		this.deeplinkAppId = CustomDebugUI.instance.AddTextField(3535750239844224UL.ToString(), 0);
		this.deeplinkMessage = CustomDebugUI.instance.AddTextField("MSG_UNITY_SAMPLE", 0);
		instance.AddButton("LaunchSelf", new DebugUIBuilder.OnClick(this.LaunchSelf), -1, 0, false);
		if (global::UnityEngine.Application.platform == RuntimePlatform.Android && !Core.IsInitialized())
		{
			Core.Initialize(null);
		}
		this.uiLaunchType = instance.AddLabel("LaunchType: ", 0);
		this.uiLaunchSource = instance.AddLabel("LaunchSource: ", 0);
		this.uiDeepLinkMessage = instance.AddLabel("DeeplinkMessage: ", 0);
		instance.ToggleLaserPointer(true);
		instance.Show();
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00059FE8 File Offset: 0x000581E8
	private void Update()
	{
		DebugUIBuilder instance = DebugUIBuilder.instance;
		if (global::UnityEngine.Application.platform == RuntimePlatform.Android)
		{
			LaunchDetails launchDetails = ApplicationLifecycle.GetLaunchDetails();
			this.uiLaunchType.GetComponentInChildren<Text>().text = "LaunchType: " + launchDetails.LaunchType.ToString();
			this.uiLaunchSource.GetComponentInChildren<Text>().text = "LaunchSource: " + launchDetails.LaunchSource;
			this.uiDeepLinkMessage.GetComponentInChildren<Text>().text = "DeeplinkMessage: " + launchDetails.DeeplinkMessage;
		}
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x0005A0C0 File Offset: 0x000582C0
	private void LaunchUnrealDeeplinkSample()
	{
		Debug.Log(string.Format("LaunchOtherApp({0})", 4055411724486843UL));
		ApplicationOptions applicationOptions = new ApplicationOptions();
		applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
		Oculus.Platform.Application.LaunchOtherApp(4055411724486843UL, applicationOptions);
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x0005A118 File Offset: 0x00058318
	private void LaunchSelf()
	{
		ulong num;
		if (ulong.TryParse(PlatformSettings.MobileAppID, out num))
		{
			Debug.Log(string.Format("LaunchSelf({0})", num));
			ApplicationOptions applicationOptions = new ApplicationOptions();
			applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
			Oculus.Platform.Application.LaunchOtherApp(num, applicationOptions);
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x0005A16C File Offset: 0x0005836C
	private void LaunchOtherApp()
	{
		ulong num;
		if (ulong.TryParse(this.deeplinkAppId.GetComponentInChildren<Text>().text, out num))
		{
			Debug.Log(string.Format("LaunchOtherApp({0})", num));
			ApplicationOptions applicationOptions = new ApplicationOptions();
			applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
			Oculus.Platform.Application.LaunchOtherApp(num, applicationOptions);
		}
	}

	// Token: 0x0400151E RID: 5406
	private const ulong UNITY_COMPANION_APP_ID = 3535750239844224UL;

	// Token: 0x0400151F RID: 5407
	private const ulong UNREAL_COMPANION_APP_ID = 4055411724486843UL;

	// Token: 0x04001520 RID: 5408
	private RectTransform deeplinkAppId;

	// Token: 0x04001521 RID: 5409
	private RectTransform deeplinkMessage;

	// Token: 0x04001522 RID: 5410
	private RectTransform uiLaunchType;

	// Token: 0x04001523 RID: 5411
	private RectTransform uiLaunchSource;

	// Token: 0x04001524 RID: 5412
	private RectTransform uiDeepLinkMessage;

	// Token: 0x04001525 RID: 5413
	private bool inMenu = true;
}
