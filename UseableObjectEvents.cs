using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000474 RID: 1140
public class UseableObjectEvents : MonoBehaviour
{
	// Token: 0x06001C10 RID: 7184 RVA: 0x00089E38 File Offset: 0x00088038
	public void Init(NetPlayer player)
	{
		bool isLocal = player.IsLocal;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string text;
		if (isLocal && instance != null)
		{
			text = instance.GetPlayFabPlayerId();
		}
		else
		{
			text = player.NickName;
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x00089EF9 File Offset: 0x000880F9
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

	// Token: 0x06001C12 RID: 7186 RVA: 0x00089F1C File Offset: 0x0008811C
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

	// Token: 0x06001C13 RID: 7187 RVA: 0x00089F3F File Offset: 0x0008813F
	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x00089F47 File Offset: 0x00088147
	private void DisposeEvents()
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

	// Token: 0x04001F30 RID: 7984
	[NonSerialized]
	private string PlayerIdString;

	// Token: 0x04001F31 RID: 7985
	[NonSerialized]
	private int PlayerId;

	// Token: 0x04001F32 RID: 7986
	public PhotonEvent Activate;

	// Token: 0x04001F33 RID: 7987
	public PhotonEvent Deactivate;
}
