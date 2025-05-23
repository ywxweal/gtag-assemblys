using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Oculus.Platform;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// Token: 0x0200038D RID: 909
public class GorillaMetaReport : MonoBehaviour
{
	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060014FC RID: 5372 RVA: 0x0006618D File Offset: 0x0006438D
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x00066194 File Offset: 0x00064394
	private void Start()
	{
		this.localPlayer.inOverlay = false;
		if (!Core.IsInitialized())
		{
			Core.AsyncInitialize(null);
		}
		AbuseReport.SetReportButtonPressedNotificationCallback(new Message<string>.Callback(this.OnReportButtonIntentNotif));
		MothershipClientApiUnity.OnMessageNotificationSocket += this.OnNotification;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000661E9 File Offset: 0x000643E9
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x00066208 File Offset: 0x00064408
	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Unhandled);
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.ReportText.SetActive(true);
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
			return;
		}
		if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
		}
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x00066260 File Offset: 0x00064460
	private void OnNotification(NotificationsMessageResponse notification, [NativeInteger] IntPtr _)
	{
		string title = notification.Title;
		if (title == "Warning")
		{
			this.OnWarning(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Warning");
			return;
		}
		if (title == "Mute")
		{
			this.OnMuteSanction(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Mute");
			return;
		}
		if (!(title == "Unmute"))
		{
			return;
		}
		if (GorillaTagger.hasInstance)
		{
			GorillaTagger.moderationMutedTime = -1f;
		}
		GorillaTelemetry.PostNotificationEvent("Unmute");
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000662E8 File Offset: 0x000644E8
	private void OnWarning(string warningNotification)
	{
		string[] array = warningNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 2)
		{
			Debug.LogError("Invalid warning notification");
			return;
		}
		string text = array[0];
		string[] array2 = array[1].Split(',', StringSplitOptions.None);
		if (array2.Length == 0)
		{
			Debug.LogError("Missing warning notification reasons");
			return;
		}
		string text2 = GorillaMetaReport.FormatListToString(in array2);
		this.ReportText.GetComponent<Text>().text = text.ToUpper() + " WARNING FOR " + text2.ToUpper() + "\nNEXT COMES MUTE";
		this.StartOverlay(true);
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0006636C File Offset: 0x0006456C
	private void OnMuteSanction(string muteNotification)
	{
		string[] array = muteNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 3)
		{
			Debug.LogError("Invalid mute notification");
			return;
		}
		if (!array[0].Equals("voice", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		int num;
		if (array[2].Length > 0 && int.TryParse(array[2], out num))
		{
			int num2 = num / 60;
			this.ReportText.GetComponent<Text>().text = string.Format("MUTED FOR {0} MINUTES\nBAD MONKE", num2);
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = (float)num;
			}
		}
		else
		{
			this.ReportText.GetComponent<Text>().text = "MUTED FOREVER";
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = float.PositiveInfinity;
			}
		}
		this.StartOverlay(true);
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x00066420 File Offset: 0x00064620
	private static string FormatListToString(in string[] list)
	{
		int num = list.Length;
		string text3;
		if (num != 1)
		{
			if (num != 2)
			{
				string text = RuntimeHelpers.GetSubArray<string>(list, Range.EndAt(new Index(1, true))).Join(", ");
				string text2 = ", AND ";
				string[] array = list;
				text3 = text + text2 + array[array.Length - 1];
			}
			else
			{
				text3 = list[0] + " AND " + list[1];
			}
		}
		else
		{
			text3 = list[0];
		}
		return text3;
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00066489 File Offset: 0x00064689
	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x00066498 File Offset: 0x00064698
	private void DuplicateScoreboard()
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x00066504 File Offset: 0x00064704
	private void ToggleLevelVisibility(bool state)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (state)
		{
			component.cullingMask = this.savedCullingLayers;
			return;
		}
		this.savedCullingLayers = component.cullingMask;
		component.cullingMask = this.visibleLayers;
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00066550 File Offset: 0x00064750
	private void Teardown()
	{
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		base.gameObject.SetActive(false);
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			gorillaPlayerScoreboardLine.doneReporting = false;
		}
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x0006662C File Offset: 0x0006482C
	private void CheckReportSubmit()
	{
		if (this.currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			if (gorillaPlayerScoreboardLine.doneReporting)
			{
				this.ReportText.SetActive(true);
				this.ReportText.GetComponent<Text>().text = "REPORTED " + gorillaPlayerScoreboardLine.playerNameVisible;
				this.currentScoreboard.gameObject.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000666E0 File Offset: 0x000648E0
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * this.playerLocalScreenPosition * scale.x;
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x0006676C File Offset: 0x0006496C
	private void StartOverlay(bool isSanction = false)
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.localScale = vector2 * 2f;
		this.reportScoreboard.transform.localScale = vector2;
		this.leftHandObject.transform.localScale = vector2;
		this.rightHandObject.transform.localScale = vector2;
		this.occluder.transform.localScale = vector2;
		if (this.localPlayer.InReportMenu && !PhotonNetwork.InRoom)
		{
			return;
		}
		this.localPlayer.InReportMenu = true;
		this.localPlayer.disableMovement = true;
		this.localPlayer.inOverlay = true;
		base.gameObject.SetActive(true);
		if (PhotonNetwork.InRoom && !isSanction)
		{
			this.DuplicateScoreboard();
		}
		else
		{
			this.ReportText.SetActive(true);
			this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
			this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		}
		this.ToggleLevelVisibility(false);
		this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
		if (isSanction)
		{
			this.currentScoreboard.gameObject.SetActive(false);
			return;
		}
		this.currentScoreboard.gameObject.SetActive(true);
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x000668F8 File Offset: 0x00064AF8
	private void CheckDistance()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		float num = Vector3.Distance(this.reportScoreboard.transform.position, vector);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = this.movementTime;
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, quaternion, num3));
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, quaternion, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x00066A18 File Offset: 0x00064C18
	private void Update()
	{
		if (this.blockButtonsUntilTimestamp > Time.time)
		{
			return;
		}
		if (SteamVR_Actions.gorillaTag_System.GetState(SteamVR_Input_Sources.LeftHand) && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			this.blockButtonsUntilTimestamp = Time.time + 0.75f;
		}
		if (this.localPlayer.InReportMenu)
		{
			this.localPlayer.inOverlay = true;
			this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
			this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
			this.CheckDistance();
			this.CheckReportSubmit();
		}
		if (this.closeButton.selected)
		{
			this.Teardown();
		}
		if (this.testPress)
		{
			this.testPress = false;
			this.StartOverlay(false);
		}
	}

	// Token: 0x04001750 RID: 5968
	[SerializeField]
	private GameObject occluder;

	// Token: 0x04001751 RID: 5969
	[SerializeField]
	private GameObject reportScoreboard;

	// Token: 0x04001752 RID: 5970
	[SerializeField]
	private GameObject ReportText;

	// Token: 0x04001753 RID: 5971
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x04001754 RID: 5972
	[SerializeField]
	private GorillaReportButton closeButton;

	// Token: 0x04001755 RID: 5973
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04001756 RID: 5974
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04001757 RID: 5975
	[SerializeField]
	private Vector3 playerLocalScreenPosition;

	// Token: 0x04001758 RID: 5976
	private float blockButtonsUntilTimestamp;

	// Token: 0x04001759 RID: 5977
	[SerializeField]
	private GorillaScoreBoard currentScoreboard;

	// Token: 0x0400175A RID: 5978
	private int savedCullingLayers;

	// Token: 0x0400175B RID: 5979
	public bool testPress;

	// Token: 0x0400175C RID: 5980
	public bool isMoving;

	// Token: 0x0400175D RID: 5981
	private float movementTime;
}
