using System;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B36 RID: 2870
	public class CMSTrigger : MonoBehaviour
	{
		// Token: 0x060046A8 RID: 18088 RVA: 0x001500FB File Offset: 0x0014E2FB
		public void OnEnable()
		{
			if (this.onEnableTriggerDelay > 0.0)
			{
				this.enabledTime = (double)Time.time;
			}
		}

		// Token: 0x060046A9 RID: 18089 RVA: 0x0015011A File Offset: 0x0014E31A
		public byte GetID()
		{
			return this.id;
		}

		// Token: 0x060046AA RID: 18090 RVA: 0x00150124 File Offset: 0x0014E324
		public virtual void CopyTriggerSettings(TriggerSettings settings)
		{
			this.id = settings.triggerId;
			this.triggeredBy = settings.triggeredBy;
			float num = Math.Max(settings.validationDistance, 2f);
			this.validationDistanceSquared = num * num;
			if (this.triggeredBy == TriggerSource.None)
			{
				if (settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = TriggerSource.Head;
				}
				else if (settings.triggeredByBody && !settings.triggeredByHead)
				{
					this.triggeredBy = TriggerSource.Body;
				}
				else if (settings.triggeredByHands && !settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = TriggerSource.Hands;
				}
				else
				{
					this.triggeredBy = TriggerSource.HeadOrBody;
				}
			}
			TriggerSource triggerSource = this.triggeredBy;
			if (triggerSource != TriggerSource.Hands)
			{
				if (triggerSource - TriggerSource.Head <= 2)
				{
					base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				}
			}
			else
			{
				base.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			}
			this.onEnableTriggerDelay = settings.onEnableTriggerDelay;
			this.generalRetriggerDelay = settings.generalRetriggerDelay;
			this.retriggerAfterDuration = settings.retriggerAfterDuration;
			if (Math.Abs(settings.retriggerDelay - 2f) > 0.001f && Math.Abs(settings.retriggerStayDuration - 2.0) < 0.001)
			{
				settings.retriggerStayDuration = (double)settings.retriggerDelay;
			}
			this.retriggerStayDuration = Math.Max(this.generalRetriggerDelay, settings.retriggerStayDuration);
			if (this.retriggerStayDuration <= 0.0)
			{
				this.retriggerAfterDuration = false;
			}
			this.numAllowedTriggers = settings.numAllowedTriggers;
			this.syncedToAllPlayers = settings.syncedToAllPlayers_private;
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RegisterTrigger(base.gameObject.scene.name, this);
			}
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x060046AB RID: 18091 RVA: 0x001502F5 File Offset: 0x0014E4F5
		public void OnTriggerEnter(Collider triggeringCollider)
		{
			if (this.ValidateCollider(triggeringCollider) && this.CanTrigger())
			{
				this.OnTriggerActivation(triggeringCollider);
			}
		}

		// Token: 0x060046AC RID: 18092 RVA: 0x00150310 File Offset: 0x0014E510
		private void OnTriggerStay(Collider other)
		{
			if (!this.retriggerAfterDuration)
			{
				return;
			}
			if (this.ValidateCollider(other))
			{
				if (NetworkSystem.Instance.InRoom)
				{
					if (PhotonNetwork.Time - this.lastTriggerTime > -3.0)
					{
						this.lastTriggerTime = -(4294967.295 - this.lastTriggerTime);
					}
					if (this.lastTriggerTime + this.retriggerStayDuration <= PhotonNetwork.Time)
					{
						this.OnTriggerActivation(other);
						return;
					}
				}
				else if (this.lastTriggerTime + this.retriggerStayDuration <= (double)Time.time)
				{
					this.OnTriggerActivation(other);
				}
			}
		}

		// Token: 0x060046AD RID: 18093 RVA: 0x001503A4 File Offset: 0x0014E5A4
		private bool ValidateCollider(Collider other)
		{
			GameObject gameObject = other.gameObject;
			bool flag = gameObject == GorillaTagger.Instance.headCollider.gameObject && (this.triggeredBy == TriggerSource.Head || this.triggeredBy == TriggerSource.HeadOrBody);
			bool flag2;
			if (GorillaTagger.Instance.bodyCollider.enabled)
			{
				flag2 = gameObject == GorillaTagger.Instance.bodyCollider.gameObject && (this.triggeredBy == TriggerSource.Body || this.triggeredBy == TriggerSource.HeadOrBody);
			}
			else
			{
				flag2 = gameObject == VRRig.LocalRig.gameObject && (this.triggeredBy == TriggerSource.Body || this.triggeredBy == TriggerSource.HeadOrBody);
			}
			bool flag3 = (gameObject == GorillaTagger.Instance.leftHandTriggerCollider.gameObject || gameObject == GorillaTagger.Instance.rightHandTriggerCollider.gameObject) && this.triggeredBy == TriggerSource.Hands;
			return flag || flag2 || flag3;
		}

		// Token: 0x060046AE RID: 18094 RVA: 0x00150496 File Offset: 0x0014E696
		private void OnTriggerActivation(Collider activatingCollider)
		{
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RequestTrigger(this.id);
				return;
			}
			this.Trigger(-1.0, true, false);
		}

		// Token: 0x060046AF RID: 18095 RVA: 0x001504C0 File Offset: 0x0014E6C0
		public bool CanTrigger()
		{
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				return false;
			}
			if (this.onEnableTriggerDelay > 0.0 && (double)Time.time - this.enabledTime < this.onEnableTriggerDelay)
			{
				return false;
			}
			if (this.generalRetriggerDelay <= 0.0)
			{
				return true;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (PhotonNetwork.Time - this.lastTriggerTime < -1.0)
				{
					this.lastTriggerTime = -(4294967.295 - this.lastTriggerTime);
				}
				if (this.lastTriggerTime + this.generalRetriggerDelay <= PhotonNetwork.Time)
				{
					return true;
				}
			}
			else if (this.lastTriggerTime + this.generalRetriggerDelay <= (double)Time.time)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060046B0 RID: 18096 RVA: 0x0015058C File Offset: 0x0014E78C
		public virtual void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			if (!ignoreTriggerCount)
			{
				if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
				{
					return;
				}
				this.numTimesTriggered += 1;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (triggerTime < 0.0)
				{
					triggerTime = PhotonNetwork.Time;
				}
			}
			else if (originatedLocally)
			{
				triggerTime = (double)Time.time;
			}
			this.lastTriggerTime = triggerTime;
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x060046B1 RID: 18097 RVA: 0x00150630 File Offset: 0x0014E830
		public void ResetTrigger(bool onlyResetTriggerCount = false)
		{
			if (!onlyResetTriggerCount)
			{
				this.lastTriggerTime = -1.0;
			}
			this.numTimesTriggered = 0;
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			CMSSerializer.ResetTrigger(this.id);
		}

		// Token: 0x060046B2 RID: 18098 RVA: 0x00150684 File Offset: 0x0014E884
		public void SetTriggerCount(byte value)
		{
			this.numTimesTriggered = Math.Min(value, this.numAllowedTriggers);
			if (this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x060046B3 RID: 18099 RVA: 0x001506D4 File Offset: 0x0014E8D4
		public void SetLastTriggerTime(double value)
		{
			this.lastTriggerTime = value;
		}

		// Token: 0x0400492B RID: 18731
		public const byte INVALID_TRIGGER_ID = 255;

		// Token: 0x0400492C RID: 18732
		public const double MAX_PHOTON_SERVER_TIME = 4294967.295;

		// Token: 0x0400492D RID: 18733
		public const float MINIMUM_VALIDATION_DISTANCE = 2f;

		// Token: 0x0400492E RID: 18734
		public bool syncedToAllPlayers;

		// Token: 0x0400492F RID: 18735
		public float validationDistanceSquared;

		// Token: 0x04004930 RID: 18736
		public TriggerSource triggeredBy = TriggerSource.HeadOrBody;

		// Token: 0x04004931 RID: 18737
		public double onEnableTriggerDelay;

		// Token: 0x04004932 RID: 18738
		public double generalRetriggerDelay;

		// Token: 0x04004933 RID: 18739
		public bool retriggerAfterDuration;

		// Token: 0x04004934 RID: 18740
		public double retriggerStayDuration = 2.0;

		// Token: 0x04004935 RID: 18741
		public byte numAllowedTriggers;

		// Token: 0x04004936 RID: 18742
		private byte numTimesTriggered;

		// Token: 0x04004937 RID: 18743
		private double lastTriggerTime = -1.0;

		// Token: 0x04004938 RID: 18744
		private double enabledTime = -1.0;

		// Token: 0x04004939 RID: 18745
		public byte id = byte.MaxValue;
	}
}
