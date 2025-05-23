using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200073E RID: 1854
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x06002E61 RID: 11873 RVA: 0x000E7994 File Offset: 0x000E5B94
	public void NotifyPlayerTeleporting(short teleportVFXIdx, AudioSource localPlayerTeleporterAudioSource)
	{
		if ((int)teleportVFXIdx >= this.teleporterVFX.Count)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[] { false, teleportVFXIdx });
		}
		this.ActivateTeleportVFXLocal(teleportVFXIdx, true);
		if (localPlayerTeleporterAudioSource.IsNotNull() && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
		{
			localPlayerTeleporterAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
			localPlayerTeleporterAudioSource.Play();
		}
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000E7A20 File Offset: 0x000E5C20
	public void NotifyPlayerReturning(short teleportVFXIdx)
	{
		if ((int)teleportVFXIdx >= this.returnVFX.Count)
		{
			return;
		}
		Debug.Log(string.Format("[VRTeleporterSerializer::NotifyPlayerReturning] Sending RPC to activate VFX at idx: {0}", teleportVFXIdx));
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[] { true, teleportVFXIdx });
		}
		this.ActivateReturnVFXLocal(teleportVFXIdx, true);
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x000E7A84 File Offset: 0x000E5C84
	[PunRPC]
	private void ActivateTeleportVFX(bool returning, short teleportVFXIdx, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ActivateTeleportVFX");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[13].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (returning)
		{
			this.ActivateReturnVFXLocal(teleportVFXIdx, false);
			return;
		}
		this.ActivateTeleportVFXLocal(teleportVFXIdx, false);
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000E7AF8 File Offset: 0x000E5CF8
	private void ActivateTeleportVFXLocal(short teleportVFXIdx, bool isTeleporter = false)
	{
		if ((int)teleportVFXIdx >= this.teleporterVFX.Count)
		{
			return;
		}
		ParticleSystem particleSystem = this.teleporterVFX[(int)teleportVFXIdx];
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
		if (isTeleporter)
		{
			return;
		}
		AudioSource audioSource = this.teleportAudioSource[(int)teleportVFXIdx];
		if (audioSource.IsNotNull())
		{
			audioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
			audioSource.Play();
		}
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000E7B70 File Offset: 0x000E5D70
	private void ActivateReturnVFXLocal(short teleportVFXIdx, bool isTeleporter = false)
	{
		if ((int)teleportVFXIdx >= this.returnVFX.Count)
		{
			return;
		}
		ParticleSystem particleSystem = this.returnVFX[(int)teleportVFXIdx];
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
		AudioSource audioSource = this.teleportAudioSource[(int)teleportVFXIdx];
		if (audioSource.IsNotNull())
		{
			audioSource.clip = (isTeleporter ? this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)] : this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)]);
			audioSource.Play();
		}
	}

	// Token: 0x040034DD RID: 13533
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x040034DE RID: 13534
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x040034DF RID: 13535
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x040034E0 RID: 13536
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040034E1 RID: 13537
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
