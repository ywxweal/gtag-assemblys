using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Rendering;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class CMSZoneShaderSettingsTrigger : MonoBehaviour
{
	// Token: 0x06002C82 RID: 11394 RVA: 0x000DB7C4 File Offset: 0x000D99C4
	public void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x000DB7D4 File Offset: 0x000D99D4
	public void CopySettings(ZoneShaderTriggerSettings triggerSettings)
	{
		base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
		this.activateOnEnable = triggerSettings.activateOnEnable;
		if (triggerSettings.activationType == ZoneShaderTriggerSettings.ActivationType.ActivateCustomMapDefaults)
		{
			this.activateCustomMapDefaults = true;
			return;
		}
		GameObject zoneShaderSettingsObject = triggerSettings.zoneShaderSettingsObject;
		if (zoneShaderSettingsObject.IsNotNull())
		{
			this.shaderSettingsObject = zoneShaderSettingsObject;
		}
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000DB826 File Offset: 0x000D9A26
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.bodyCollider)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x000DB840 File Offset: 0x000D9A40
	private void ActivateShaderSettings()
	{
		if (this.activateCustomMapDefaults)
		{
			CustomMapManager.ActivateDefaultZoneShaderSettings();
			return;
		}
		if (this.shaderSettingsObject.IsNotNull())
		{
			ZoneShaderSettings component = this.shaderSettingsObject.GetComponent<ZoneShaderSettings>();
			if (component.IsNotNull())
			{
				component.BecomeActiveInstance(false);
			}
		}
	}

	// Token: 0x040032CC RID: 13004
	public GameObject shaderSettingsObject;

	// Token: 0x040032CD RID: 13005
	public bool activateCustomMapDefaults;

	// Token: 0x040032CE RID: 13006
	public bool activateOnEnable;
}
