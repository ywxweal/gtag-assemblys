using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class CrittersLoudNoise : CrittersActor
{
	// Token: 0x060001A9 RID: 425 RVA: 0x0000A5F8 File Offset: 0x000087F8
	public override void OnEnable()
	{
		base.OnEnable();
		this.SetTimeEnabled();
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000A606 File Offset: 0x00008806
	public void SpawnData(float _soundVolume, float _soundDuration, float _soundMultiplier, bool _soundEnabled)
	{
		this.soundVolume = _soundVolume;
		this.volumeFearAttractionMultiplier = _soundMultiplier;
		this.soundDuration = _soundDuration;
		this.soundEnabled = _soundEnabled;
		this.Initialize();
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000A62C File Offset: 0x0000882C
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.wasSoundEnabled = this.soundEnabled;
		if (PhotonNetwork.InRoom)
		{
			if (PhotonNetwork.Time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > PhotonNetwork.Time)
			{
				this.soundEnabled = false;
			}
		}
		else if ((double)Time.time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > (double)Time.time)
		{
			this.soundEnabled = false;
		}
		if (this.disableWhenSoundDisabled && !this.soundEnabled)
		{
			this.isEnabled = false;
			if (base.gameObject.activeSelf != this.isEnabled)
			{
				base.gameObject.SetActive(this.isEnabled);
			}
		}
		this.updatedSinceLastFrame = flag || this.wasSoundEnabled != this.soundEnabled || this.wasEnabled != this.isEnabled;
		return this.updatedSinceLastFrame;
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000A730 File Offset: 0x00008930
	public override void ProcessRemote()
	{
		if (!this.wasEnabled && this.isEnabled)
		{
			this.SetTimeEnabled();
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000A748 File Offset: 0x00008948
	public void SetTimeEnabled()
	{
		if (PhotonNetwork.InRoom)
		{
			this.timeSoundEnabled = PhotonNetwork.Time;
			return;
		}
		this.timeSoundEnabled = (double)Time.time;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000A76C File Offset: 0x0000896C
	public override void CalculateFear(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000A7E8 File Offset: 0x000089E8
	public override void CalculateAttraction(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000A864 File Offset: 0x00008A64
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		float num;
		float num2;
		bool flag;
		float num3;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num3)))
		{
			return false;
		}
		this.soundVolume = num.GetFinite();
		this.soundDuration = num2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = num3.GetFinite();
		return true;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000A8E0 File Offset: 0x00008AE0
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.soundVolume);
		stream.SendNext(this.soundDuration);
		stream.SendNext(this.soundEnabled);
		stream.SendNext(this.volumeFearAttractionMultiplier);
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000A938 File Offset: 0x00008B38
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.soundVolume);
		objList.Add(this.soundDuration);
		objList.Add(this.soundEnabled);
		objList.Add(this.volumeFearAttractionMultiplier);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000A99B File Offset: 0x00008B9B
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 4;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000A9A8 File Offset: 0x00008BA8
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		float num;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		float num2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out num2))
		{
			return this.TotalActorDataLength();
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex + 2], out flag))
		{
			return this.TotalActorDataLength();
		}
		float num3;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 3], out num3))
		{
			return this.TotalActorDataLength();
		}
		this.soundVolume = num.GetFinite();
		this.soundDuration = num2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = num3.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000AA44 File Offset: 0x00008C44
	public void PlayHandTapLocal(bool isLeft)
	{
		this.timeSoundEnabled = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		this.soundEnabled = true;
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000AA67 File Offset: 0x00008C67
	public void PlayHandTapRemote(double serverTime, bool isLeft)
	{
		this.timeSoundEnabled = serverTime;
		this.soundEnabled = true;
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000AA77 File Offset: 0x00008C77
	public void PlayVoiceSpeechLocal(double serverTime, float duration, float volume)
	{
		this.soundDuration = duration;
		this.timeSoundEnabled = serverTime;
		this.soundVolume = volume;
		this.soundEnabled = true;
	}

	// Token: 0x040001F9 RID: 505
	public float soundVolume;

	// Token: 0x040001FA RID: 506
	public float volumeFearAttractionMultiplier;

	// Token: 0x040001FB RID: 507
	public float soundDuration;

	// Token: 0x040001FC RID: 508
	public double timeSoundEnabled;

	// Token: 0x040001FD RID: 509
	public bool soundEnabled;

	// Token: 0x040001FE RID: 510
	private bool wasSoundEnabled;

	// Token: 0x040001FF RID: 511
	public bool disableWhenSoundDisabled;
}
