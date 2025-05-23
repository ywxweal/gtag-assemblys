using System;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	// Token: 0x06001CB4 RID: 7348 RVA: 0x0008B97F File Offset: 0x00089B7F
	private void Awake()
	{
		if (this.sameSceneSettingsRef != null)
		{
			this.settings = this.sameSceneSettingsRef;
			return;
		}
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x0008B9B0 File Offset: 0x00089BB0
	public override void OnBoxTriggered()
	{
		if (this.settings == null)
		{
			if (this.sameSceneSettingsRef != null)
			{
				this.settings = this.sameSceneSettingsRef;
			}
			else
			{
				this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
			}
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance(false);
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	// Token: 0x04001FF0 RID: 8176
	[SerializeField]
	private XSceneRef settingsRef;

	// Token: 0x04001FF1 RID: 8177
	[SerializeField]
	private ZoneShaderSettings sameSceneSettingsRef;

	// Token: 0x04001FF2 RID: 8178
	private ZoneShaderSettings settings;
}
