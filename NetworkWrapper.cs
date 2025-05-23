using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BA RID: 698
public class NetworkWrapper : MonoBehaviour
{
	// Token: 0x060010C7 RID: 4295 RVA: 0x000505EF File Offset: 0x0004E7EF
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void AutoInstantiate()
	{
		Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(Resources.Load<GameObject>("P_NetworkWrapper")));
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x00050608 File Offset: 0x0004E808
	private void Awake()
	{
		if (this.titleRef != null)
		{
			this.titleRef.text = "PUN";
		}
		this.activeNetworkSystem = base.gameObject.AddComponent<NetworkSystemPUN>();
		this.activeNetworkSystem.AddVoiceSettings(this.VoiceSettings);
		this.activeNetworkSystem.config = this.netSysConfig;
		this.activeNetworkSystem.regionNames = this.networkRegionNames;
		this.activeNetworkSystem.OnPlayerJoined += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnPlayerLeft += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnMultiplayerStarted += this.UpdatePlayerCount;
		this.activeNetworkSystem.OnReturnedToSinglePlayer += this.UpdatePlayerCount;
		Debug.Log("<color=green>initialize Network System</color>");
		this.activeNetworkSystem.Initialise();
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x000506E8 File Offset: 0x0004E8E8
	private void UpdatePlayerCountWrapper(NetPlayer player)
	{
		this.UpdatePlayerCount();
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x000506F0 File Offset: 0x0004E8F0
	private void UpdatePlayerCount()
	{
		if (this.playerCountTextRef == null)
		{
			return;
		}
		if (!this.activeNetworkSystem.IsOnline)
		{
			this.playerCountTextRef.text = string.Format("0/{0}", this.netSysConfig.MaxPlayerCount);
			Debug.Log("Player count updated");
			return;
		}
		Debug.Log("Player count not updated");
		this.playerCountTextRef.text = string.Format("{0}/{1}", this.activeNetworkSystem.AllNetPlayers.Length, this.netSysConfig.MaxPlayerCount);
	}

	// Token: 0x0400130A RID: 4874
	[HideInInspector]
	public NetworkSystem activeNetworkSystem;

	// Token: 0x0400130B RID: 4875
	public Text titleRef;

	// Token: 0x0400130C RID: 4876
	[Header("NetSys settings")]
	public NetworkSystemConfig netSysConfig;

	// Token: 0x0400130D RID: 4877
	public string[] networkRegionNames;

	// Token: 0x0400130E RID: 4878
	public string[] devNetworkRegionNames;

	// Token: 0x0400130F RID: 4879
	[Header("Debug output refs")]
	public Text stateTextRef;

	// Token: 0x04001310 RID: 4880
	public Text playerCountTextRef;

	// Token: 0x04001311 RID: 4881
	[SerializeField]
	private SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x04001312 RID: 4882
	private const string WrapperResourcePath = "P_NetworkWrapper";
}
