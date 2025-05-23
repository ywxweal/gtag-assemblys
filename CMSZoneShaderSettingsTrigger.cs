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
	// Token: 0x06002C81 RID: 11393 RVA: 0x000DB720 File Offset: 0x000D9920
	public void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000DB730 File Offset: 0x000D9930
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

	// Token: 0x06002C83 RID: 11395 RVA: 0x000DB782 File Offset: 0x000D9982
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.bodyCollider)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000DB79C File Offset: 0x000D999C
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

	// Token: 0x040032CA RID: 13002
	public GameObject shaderSettingsObject;

	// Token: 0x040032CB RID: 13003
	public bool activateCustomMapDefaults;

	// Token: 0x040032CC RID: 13004
	public bool activateOnEnable;
}
