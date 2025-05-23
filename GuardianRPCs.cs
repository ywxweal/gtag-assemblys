using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000932 RID: 2354
internal class GuardianRPCs : RPCNetworkBase
{
	// Token: 0x0600392B RID: 14635 RVA: 0x001130AA File Offset: 0x001112AA
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.guardianManager = (GorillaGuardianManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x0600392C RID: 14636 RVA: 0x001130C4 File Offset: 0x001112C4
	[PunRPC]
	public void GuardianRequestEject(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianRequestEject");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender != null)
		{
			this.guardianManager.EjectGuardian(photonMessageInfoWrapped.Sender);
		}
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x00113100 File Offset: 0x00111300
	[PunRPC]
	public void GuardianLaunchPlayer(Vector3 velocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianLaunchPlayer");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent LaunchPlayer when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (!(in velocity).IsValid(in num))
		{
			return;
		}
		if (!this.launchCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.guardianManager.LaunchPlayer(photonMessageInfoWrapped.Sender, velocity);
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x00113198 File Offset: 0x00111398
	[PunRPC]
	public void ShowSlapEffects(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlapEffects");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlapEffects when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if ((in location).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in direction).IsValid(in num2))
			{
				if (!this.slapFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlapEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x0011323C File Offset: 0x0011143C
	[PunRPC]
	public void ShowSlamEffect(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlamEffect");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlamEffect when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if ((in location).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in direction).IsValid(in num2))
			{
				if (!this.slamFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlamEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x04003E58 RID: 15960
	private GameModeSerializer serializer;

	// Token: 0x04003E59 RID: 15961
	private GorillaGuardianManager guardianManager;

	// Token: 0x04003E5A RID: 15962
	private CallLimiter launchCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04003E5B RID: 15963
	private CallLimiter slapFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04003E5C RID: 15964
	private CallLimiter slamFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);
}
