using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000987 RID: 2439
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x170005CE RID: 1486
	// (get) Token: 0x06003AA9 RID: 15017 RVA: 0x00118AAB File Offset: 0x00116CAB
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x00118AB2 File Offset: 0x00116CB2
	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x00118AED File Offset: 0x00116CED
	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
		}
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x00118B04 File Offset: 0x00116D04
	private void Update()
	{
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		if (flag != this.buttonDown)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				switch (this.currentState)
				{
				case DebugHudStats.State.ShowStats:
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
					break;
				}
				switch (this.currentState)
				{
				case DebugHudStats.State.Inactive:
					this.currentState = DebugHudStats.State.Active;
					this.text.gameObject.SetActive(true);
					break;
				case DebugHudStats.State.Active:
					this.currentState = DebugHudStats.State.ShowLog;
					break;
				case DebugHudStats.State.ShowLog:
					this.currentState = DebugHudStats.State.ShowStats;
					this.distanceMoved = (this.distanceSwam = 0f);
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
					break;
				case DebugHudStats.State.ShowStats:
					this.currentState = DebugHudStats.State.Inactive;
					this.text.gameObject.SetActive(false);
					break;
				}
			}
		}
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < 89)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("v: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			num = Mathf.Min(num, 90);
			this.builder.Append((num < 89) ? " - <color=\"red\">" : " - <color=\"white\">");
			this.builder.Append(num);
			this.builder.AppendLine(" fps</color>");
			if (GorillaComputer.instance != null)
			{
				this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
			}
			else
			{
				this.builder.AppendLine("Server Time Unavailable");
			}
			GroupJoinZoneAB groupZone = GorillaTagger.Instance.offlineVRRig.zoneEntity.GroupZone;
			if (groupZone != this.lastGroupJoinZone)
			{
				this.zones = groupZone.ToString();
				this.lastGroupJoinZone = groupZone;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("Z: <color=\"orange\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (this.currentState == DebugHudStats.State.ShowStats)
			{
				this.builder.AppendLine();
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s", magnitude));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s", this.groundVelocity.magnitude));
				this.builder.AppendLine(string.Format("head: {0:F2}\n", headCenterPosition));
				this.builder.AppendLine(string.Format("odo: {0:F2}m", this.distanceMoved));
				this.builder.AppendLine(string.Format("swam: {0:F2}m", this.distanceSwam));
			}
			else if (this.currentState == DebugHudStats.State.ShowLog)
			{
				this.builder.AppendLine();
				for (int i = 0; i < this.logMessages.Count; i++)
				{
					this.builder.AppendLine(this.logMessages[i]);
				}
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x00118FC9 File Offset: 0x001171C9
	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x00118FE1 File Offset: 0x001171E1
	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x00118FF9 File Offset: 0x001171F9
	private void OnEnable()
	{
		Application.logMessageReceived += this.LogMessageReceived;
	}

	// Token: 0x06003AB0 RID: 15024 RVA: 0x0011900C File Offset: 0x0011720C
	private void OnDisable()
	{
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	// Token: 0x06003AB1 RID: 15025 RVA: 0x0011901F File Offset: 0x0011721F
	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		this.logMessages.Add(this.getColorStringFromLogType(type) + condition + "</color>");
		if (this.logMessages.Count > 6)
		{
			this.logMessages.RemoveAt(0);
		}
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x00119058 File Offset: 0x00117258
	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x00119088 File Offset: 0x00117288
	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	// Token: 0x04003F7A RID: 16250
	private const int FPS_THRESHOLD = 89;

	// Token: 0x04003F7B RID: 16251
	private static DebugHudStats _instance;

	// Token: 0x04003F7C RID: 16252
	[SerializeField]
	public TMP_Text text;

	// Token: 0x04003F7D RID: 16253
	[SerializeField]
	private TMP_Text fpsWarning;

	// Token: 0x04003F7E RID: 16254
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x04003F7F RID: 16255
	private float updateTimer;

	// Token: 0x04003F80 RID: 16256
	public float sessionAnytrackingLost;

	// Token: 0x04003F81 RID: 16257
	public float last30SecondsTrackingLost;

	// Token: 0x04003F82 RID: 16258
	private float firstAwake;

	// Token: 0x04003F83 RID: 16259
	private bool leftHandTracked;

	// Token: 0x04003F84 RID: 16260
	private bool rightHandTracked;

	// Token: 0x04003F85 RID: 16261
	private StringBuilder builder;

	// Token: 0x04003F86 RID: 16262
	private Vector3 averagedVelocity;

	// Token: 0x04003F87 RID: 16263
	private Vector3 groundVelocity;

	// Token: 0x04003F88 RID: 16264
	private Vector3 centerHeadPos;

	// Token: 0x04003F89 RID: 16265
	private float distanceMoved;

	// Token: 0x04003F8A RID: 16266
	private float distanceSwam;

	// Token: 0x04003F8B RID: 16267
	private List<string> logMessages = new List<string>();

	// Token: 0x04003F8C RID: 16268
	private bool buttonDown;

	// Token: 0x04003F8D RID: 16269
	private bool showLog;

	// Token: 0x04003F8E RID: 16270
	private int lowFps;

	// Token: 0x04003F8F RID: 16271
	private string zones;

	// Token: 0x04003F90 RID: 16272
	private GroupJoinZoneAB lastGroupJoinZone;

	// Token: 0x04003F91 RID: 16273
	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	// Token: 0x02000988 RID: 2440
	private enum State
	{
		// Token: 0x04003F93 RID: 16275
		Inactive,
		// Token: 0x04003F94 RID: 16276
		Active,
		// Token: 0x04003F95 RID: 16277
		ShowLog,
		// Token: 0x04003F96 RID: 16278
		ShowStats
	}
}
