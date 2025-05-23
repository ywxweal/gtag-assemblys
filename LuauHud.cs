using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020008B0 RID: 2224
public class LuauHud : MonoBehaviour
{
	// Token: 0x17000536 RID: 1334
	// (get) Token: 0x060035D0 RID: 13776 RVA: 0x001043F4 File Offset: 0x001025F4
	public static LuauHud Instance
	{
		get
		{
			return LuauHud._instance;
		}
	}

	// Token: 0x060035D1 RID: 13777 RVA: 0x001043FC File Offset: 0x001025FC
	private void Awake()
	{
		if (LuauHud._instance != null && LuauHud._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			LuauHud._instance = this;
		}
		this.path = Path.Combine(Application.persistentDataPath, "script.luau");
	}

	// Token: 0x060035D2 RID: 13778 RVA: 0x0010444B File Offset: 0x0010264B
	private void OnDestroy()
	{
		if (LuauHud._instance == this)
		{
			LuauHud._instance = null;
		}
	}

	// Token: 0x060035D3 RID: 13779 RVA: 0x00104460 File Offset: 0x00102660
	private void Start()
	{
		this.useLuauHud = true;
		DebugHudStats instance = DebugHudStats.Instance;
		instance.enabled = false;
		this.debugHud = instance.gameObject;
		this.text = instance.text;
		this.builder = new StringBuilder(50);
	}

	// Token: 0x060035D4 RID: 13780 RVA: 0x001044A8 File Offset: 0x001026A8
	private void Update()
	{
		MapDescriptor loadedMapDescriptor = CustomMapLoader.LoadedMapDescriptor;
		if (loadedMapDescriptor == null || !loadedMapDescriptor.DevMode)
		{
			if (this.showLog && this.useLuauHud)
			{
				this.showLog = false;
				this.text.gameObject.SetActive(false);
			}
			return;
		}
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance == null || instance.GameType() != GameModeType.Custom)
		{
			return;
		}
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		if (flag != this.buttonDown && this.useLuauHud)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					this.text.gameObject.SetActive(true);
					this.showLog = true;
				}
				else
				{
					this.text.gameObject.SetActive(false);
					this.showLog = false;
				}
			}
		}
		if (!flag || !flag2)
		{
			this.resetTimer = Time.time;
		}
		if (Time.time - this.resetTimer > 2f)
		{
			this.LuauLog("Restarting Luau Script");
			if (CustomGameMode.GameModeInitialized)
			{
				CustomGameMode.StopScript();
			}
			if (File.Exists(this.path))
			{
				this.script = File.ReadAllText(this.path);
			}
			if (this.script != "")
			{
				CustomGameMode.LuaScript = this.script;
			}
			CustomGameMode.LuaStart();
			this.resetTimer = Time.time;
		}
		if (this.useLuauHud && this.showLog)
		{
			this.builder.AppendLine();
			for (int i = 0; i < this.luauLogs.Count; i++)
			{
				this.builder.AppendLine(this.luauLogs[i]);
			}
		}
	}

	// Token: 0x060035D5 RID: 13781 RVA: 0x0010464F File Offset: 0x0010284F
	public void LuauLog(string log)
	{
		Debug.Log(log);
		this.luauLogs.Add(log);
		if (this.luauLogs.Count > 6)
		{
			this.luauLogs.RemoveAt(0);
		}
	}

	// Token: 0x04003B91 RID: 15249
	private bool useLuauHud;

	// Token: 0x04003B92 RID: 15250
	private bool buttonDown;

	// Token: 0x04003B93 RID: 15251
	private bool showLog;

	// Token: 0x04003B94 RID: 15252
	private GameObject debugHud;

	// Token: 0x04003B95 RID: 15253
	private TMP_Text text;

	// Token: 0x04003B96 RID: 15254
	private StringBuilder builder;

	// Token: 0x04003B97 RID: 15255
	private float resetTimer;

	// Token: 0x04003B98 RID: 15256
	private string path = "";

	// Token: 0x04003B99 RID: 15257
	private string script = "";

	// Token: 0x04003B9A RID: 15258
	private static LuauHud _instance;

	// Token: 0x04003B9B RID: 15259
	private List<string> luauLogs = new List<string>();
}
