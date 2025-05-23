using System;
using emotitron.Compression;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D26 RID: 3366
	public class DrinkableHoldable : TransferrableObject
	{
		// Token: 0x06005419 RID: 21529 RVA: 0x001977B0 File Offset: 0x001959B0
		internal override void OnEnable()
		{
			base.OnEnable();
			base.enabled = this.containerLiquid != null;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.sipSoundCooldown, this.containerLiquid.fillAmount, this.coolingDown);
			this.myByteArray = new byte[32];
		}

		// Token: 0x0600541A RID: 21530 RVA: 0x00197804 File Offset: 0x00195A04
		protected override void LateUpdateLocal()
		{
			if (!this.containerLiquid.isActiveAndEnabled || !GorillaParent.hasInstance || !GorillaComputer.hasInstance)
			{
				base.LateUpdateLocal();
				return;
			}
			float num = (float)((GorillaComputer.instance.startupMillis + (long)Time.realtimeSinceStartup * 1000L) % 259200000L) / 1000f;
			if (Mathf.Abs(num - this.lastTimeSipSoundPlayed) > 129600f)
			{
				this.lastTimeSipSoundPlayed = num;
			}
			float num2 = this.sipRadius * this.sipRadius;
			bool flag = (GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
			if (!flag)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!vrrig.isOfflineVRRig)
					{
						if (flag || vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						flag = (vrrig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
					}
				}
			}
			if (flag)
			{
				this.containerLiquid.fillAmount = Mathf.Clamp01(this.containerLiquid.fillAmount - this.sipRate * Time.deltaTime);
				if (num > this.lastTimeSipSoundPlayed + this.sipSoundCooldown)
				{
					if (!this.wasSipping)
					{
						this.lastTimeSipSoundPlayed = num;
						this.coolingDown = true;
					}
				}
				else
				{
					this.coolingDown = false;
				}
			}
			this.wasSipping = flag;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.lastTimeSipSoundPlayed, this.containerLiquid.fillAmount, this.coolingDown);
			base.LateUpdateLocal();
		}

		// Token: 0x0600541B RID: 21531 RVA: 0x00197A00 File Offset: 0x00195C00
		protected override void LateUpdateReplicated()
		{
			base.LateUpdateReplicated();
			int itemState = (int)this.itemState;
			this.UnpackValuesNonstatic(in itemState, out this.lastTimeSipSoundPlayed, out this.containerLiquid.fillAmount, out this.coolingDown);
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x00197A39 File Offset: 0x00195C39
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (this.coolingDown && !this.wasCoolingDown)
			{
				this.sipSoundBankPlayer.Play();
			}
			this.wasCoolingDown = this.coolingDown;
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x00197A68 File Offset: 0x00195C68
		private static int PackValues(float cooldownStartTime, float fillAmount, bool coolingDown)
		{
			byte[] array = new byte[32];
			int num = 0;
			array.WriteBool(coolingDown, ref num);
			array.Write((ulong)((double)cooldownStartTime * 100.0), ref num, 25);
			array.Write((ulong)((double)fillAmount * 63.0), ref num, 6);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x0600541E RID: 21534 RVA: 0x00197ABC File Offset: 0x00195CBC
		private void UnpackValuesNonstatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			DrinkableHoldable.GetBytes(packed, ref this.myByteArray);
			int num = 0;
			coolingDown = this.myByteArray.ReadBool(ref num);
			cooldownStartTime = (float)(this.myByteArray.Read(ref num, 25) / 100.0);
			fillAmount = this.myByteArray.Read(ref num, 6) / 63f;
		}

		// Token: 0x0600541F RID: 21535 RVA: 0x00197B20 File Offset: 0x00195D20
		public static void GetBytes(int value, ref byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)((value >> 8 * i) & 255);
			}
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x00197B50 File Offset: 0x00195D50
		private static void UnpackValuesStatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			byte[] bytes = BitConverter.GetBytes(packed);
			int num = 0;
			coolingDown = bytes.ReadBool(ref num);
			cooldownStartTime = (float)(bytes.Read(ref num, 25) / 100.0);
			fillAmount = bytes.Read(ref num, 6) / 63f;
		}

		// Token: 0x04005714 RID: 22292
		[AssignInCorePrefab]
		public ContainerLiquid containerLiquid;

		// Token: 0x04005715 RID: 22293
		[AssignInCorePrefab]
		[SoundBankInfo]
		public SoundBankPlayer sipSoundBankPlayer;

		// Token: 0x04005716 RID: 22294
		[AssignInCorePrefab]
		public float sipRate = 0.1f;

		// Token: 0x04005717 RID: 22295
		[AssignInCorePrefab]
		public float sipSoundCooldown = 0.5f;

		// Token: 0x04005718 RID: 22296
		[AssignInCorePrefab]
		public Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04005719 RID: 22297
		[AssignInCorePrefab]
		public float sipRadius = 0.15f;

		// Token: 0x0400571A RID: 22298
		private float lastTimeSipSoundPlayed;

		// Token: 0x0400571B RID: 22299
		private bool wasSipping;

		// Token: 0x0400571C RID: 22300
		private bool coolingDown;

		// Token: 0x0400571D RID: 22301
		private bool wasCoolingDown;

		// Token: 0x0400571E RID: 22302
		private byte[] myByteArray;
	}
}
