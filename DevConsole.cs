using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001AE RID: 430
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000A84 RID: 2692 RVA: 0x00039D93 File Offset: 0x00037F93
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindObjectOfType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000A85 RID: 2693 RVA: 0x00039DB1 File Offset: 0x00037FB1
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00039DC0 File Offset: 0x00037FC0
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000CC0 RID: 3264
	private static DevConsole _instance;

	// Token: 0x04000CC1 RID: 3265
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04000CC2 RID: 3266
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000CC3 RID: 3267
	[SerializeField]
	private float maxHeight;

	// Token: 0x04000CC4 RID: 3268
	public static readonly string[] tracebackScrubbing = new string[] { "ExitGames.Client.Photon", "Photon.Realtime.LoadBalancingClient", "Photon.Pun.PhotonHandler" };

	// Token: 0x04000CC5 RID: 3269
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x04000CC6 RID: 3270
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x04000CC7 RID: 3271
	public int targetLogIndex = -1;

	// Token: 0x04000CC8 RID: 3272
	public int currentLogIndex;

	// Token: 0x04000CC9 RID: 3273
	public bool isMuted;

	// Token: 0x04000CCA RID: 3274
	public float currentZoomLevel = 1f;

	// Token: 0x04000CCB RID: 3275
	public List<GameObject> disableWhileActive;

	// Token: 0x04000CCC RID: 3276
	public List<GameObject> enableWhileActive;

	// Token: 0x04000CCD RID: 3277
	public int expandAmount = 20;

	// Token: 0x04000CCE RID: 3278
	public int expandedMessageIndex = -1;

	// Token: 0x04000CCF RID: 3279
	public bool canExpand = true;

	// Token: 0x04000CD0 RID: 3280
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04000CD1 RID: 3281
	public float lineStartHeight;

	// Token: 0x04000CD2 RID: 3282
	public float textStartHeight;

	// Token: 0x04000CD3 RID: 3283
	public float lineStartTextWidth;

	// Token: 0x04000CD4 RID: 3284
	public double textScale = 0.5;

	// Token: 0x04000CD5 RID: 3285
	public List<DevConsoleInstance> instances;

	// Token: 0x020001AF RID: 431
	[Serializable]
	public class LogEntry
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000A8A RID: 2698 RVA: 0x00039EAA File Offset: 0x000380AA
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00039ED8 File Offset: 0x000380D8
		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!DevConsole.tracebackScrubbing.Any((string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		// Token: 0x04000CD6 RID: 3286
		private static int TotalIndex;

		// Token: 0x04000CD7 RID: 3287
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x04000CD8 RID: 3288
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x04000CD9 RID: 3289
		public readonly string Trace;

		// Token: 0x04000CDA RID: 3290
		public bool forwarded;

		// Token: 0x04000CDB RID: 3291
		public int repeatCount = 1;

		// Token: 0x04000CDC RID: 3292
		public bool filtered;

		// Token: 0x04000CDD RID: 3293
		public int index;
	}

	// Token: 0x020001B1 RID: 433
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000A8E RID: 2702 RVA: 0x00039F92 File Offset: 0x00038192
		// (set) Token: 0x06000A8F RID: 2703 RVA: 0x00039F9A File Offset: 0x0003819A
		public Type data { get; set; }

		// Token: 0x06000A90 RID: 2704 RVA: 0x00039FA4 File Offset: 0x000381A4
		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		// Token: 0x04000CDF RID: 3295
		public GorillaDevButton[] buttons;

		// Token: 0x04000CE0 RID: 3296
		public Text lineText;

		// Token: 0x04000CE1 RID: 3297
		public RectTransform transform;

		// Token: 0x04000CE2 RID: 3298
		public int targetMessage;

		// Token: 0x04000CE3 RID: 3299
		public GorillaDevButton maximizeButton;

		// Token: 0x04000CE4 RID: 3300
		public GorillaDevButton forwardButton;

		// Token: 0x04000CE5 RID: 3301
		public SpriteRenderer backdrop;

		// Token: 0x04000CE6 RID: 3302
		private bool expanded;

		// Token: 0x04000CE7 RID: 3303
		public DevInspector inspector;
	}

	// Token: 0x020001B2 RID: 434
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x06000A91 RID: 2705 RVA: 0x0003A020 File Offset: 0x00038220
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string text3 in array)
				{
					text2 = text2 + "    " + text3 + "\n";
				}
				string text4 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text4.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		// Token: 0x04000CE9 RID: 3305
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x020001B3 RID: 435
		[Serializable]
		public class Block
		{
			// Token: 0x06000A93 RID: 2707 RVA: 0x0003A1F0 File Offset: 0x000383F0
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x04000CEA RID: 3306
			public string type;

			// Token: 0x04000CEB RID: 3307
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x020001B4 RID: 436
		[Serializable]
		public class TextBlock
		{
			// Token: 0x04000CEC RID: 3308
			public string type;

			// Token: 0x04000CED RID: 3309
			public string text;
		}
	}
}
