using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200073E RID: 1854
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x06002E60 RID: 11872 RVA: 0x000E78F0 File Offset: 0x000E5AF0
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

	// Token: 0x06002E61 RID: 11873 RVA: 0x000E797C File Offset: 0x000E5B7C
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

	// Token: 0x06002E62 RID: 11874 RVA: 0x000E79E0 File Offset: 0x000E5BE0
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

	// Token: 0x06002E63 RID: 11875 RVA: 0x000E7A54 File Offset: 0x000E5C54
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

	// Token: 0x06002E64 RID: 11876 RVA: 0x000E7ACC File Offset: 0x000E5CCC
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

	// Token: 0x040034DB RID: 13531
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x040034DC RID: 13532
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x040034DD RID: 13533
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x040034DE RID: 13534
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040034DF RID: 13535
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
