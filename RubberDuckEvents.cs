using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x06001BEF RID: 7151 RVA: 0x00089798 File Offset: 0x00087998
	public void Init(NetPlayer player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool isLocal = player.IsLocal;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (isLocal && instance != null)
			{
				text = instance.GetPlayFabPlayerId();
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x00089872 File Offset: 0x00087A72
	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00089895 File Offset: 0x00087A95
	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000898B8 File Offset: 0x00087AB8
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000898C0 File Offset: 0x00087AC0
	public void Dispose()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	// Token: 0x04001F12 RID: 7954
	public int PlayerId;

	// Token: 0x04001F13 RID: 7955
	public string PlayerIdString;

	// Token: 0x04001F14 RID: 7956
	public PhotonEvent Activate;

	// Token: 0x04001F15 RID: 7957
	public PhotonEvent Deactivate;
}
