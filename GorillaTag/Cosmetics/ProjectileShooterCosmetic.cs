using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF2 RID: 3570
	[RequireComponent(typeof(TransferrableObject))]
	public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06005860 RID: 22624 RVA: 0x001B2D65 File Offset: 0x001B0F65
		// (set) Token: 0x06005861 RID: 22625 RVA: 0x001B2D6D File Offset: 0x001B0F6D
		public bool TickRunning { get; set; }

		// Token: 0x06005862 RID: 22626 RVA: 0x001B2D78 File Offset: 0x001B0F78
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
			this.launchedTime = 0f;
			this.canShoot = true;
			this.pressCounter = 0f;
			this.chargeShader = base.GetComponent<ProjectileChargeShader>();
		}

		// Token: 0x06005863 RID: 22627 RVA: 0x001B2DCB File Offset: 0x001B0FCB
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			if (this.chargeShader)
			{
				this.chargeShader.UpdateChargeProgress(0f);
			}
		}

		// Token: 0x06005864 RID: 22628 RVA: 0x001B2DF0 File Offset: 0x001B0FF0
		public void Tick()
		{
			if (!this.canShoot && Time.time - this.launchedTime >= this.cooldown)
			{
				this.canShoot = true;
				if (this.audioSource && this.readyToShootSoundBank != null)
				{
					this.readyToShootSoundBank.Play();
				}
				this.pressCounter = 0f;
			}
			if (this.pressStarted && this.canShoot)
			{
				float num = this.pressCounter + 1f;
				this.pressCounter = num;
				if (num <= this.maxButtonPressDuration)
				{
					this.pressCounter += 1f;
					if (this.audioSource && this.chargingSoundBank != null && !this.chargingSoundBank.isPlaying)
					{
						this.chargingSoundBank.Play();
					}
				}
				if (this.chargeShader)
				{
					this.chargeShader.UpdateChargeProgress(this.GetChargeRate());
				}
			}
		}

		// Token: 0x06005865 RID: 22629 RVA: 0x001B2EE8 File Offset: 0x001B10E8
		private void Shoot()
		{
			if (!this.canShoot)
			{
				return;
			}
			Vector3 vector = this.launchPosition.forward * this.GetLaunchSpeed() * this.transferrableObject.ownerRig.scaleFactor;
			this.LaunchProjectileLocal(this.launchPosition.position, this.launchPosition.rotation, vector, this.transferrableObject.ownerRig.scaleFactor);
			this.launchedTime = Time.time;
			this.canShoot = false;
			if (this.transferrableObject.IsMyItem())
			{
				UnityEvent<bool, float> unityEvent = this.onOwnerLaunchProjectile;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.transferrableObject.InLeftHand(), this.GetLaunchSpeed());
			}
		}

		// Token: 0x06005866 RID: 22630 RVA: 0x001B2F98 File Offset: 0x001B1198
		private void LaunchProjectileLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, float playerScale)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.projectileHash, true);
			gameObject.transform.localScale = Vector3.one * playerScale;
			IProjectile component = gameObject.GetComponent<IProjectile>();
			if (component != null)
			{
				component.Launch(startPos, rotation, velocity, playerScale);
			}
			if (this.audioSource && this.shootSoundBank != null)
			{
				this.shootSoundBank.audioSource.Stop();
				this.shootSoundBank.Play();
			}
			if (this.launchParticles)
			{
				this.launchParticles.Play();
			}
		}

		// Token: 0x06005867 RID: 22631 RVA: 0x001B3030 File Offset: 0x001B1230
		private float GetLaunchSpeed()
		{
			if (!this.useButtonPressDurationAsVelocityModifier)
			{
				return 1f;
			}
			return Mathf.Lerp(this.launchMinSpeed, this.launchMaxSpeed, Mathf.InverseLerp(0f, this.maxButtonPressDuration, Mathf.Clamp(this.pressCounter, 0f, this.maxButtonPressDuration)));
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x001B3084 File Offset: 0x001B1284
		private float GetChargeRate()
		{
			if (!this.useButtonPressDurationAsVelocityModifier)
			{
				return (float)this.chargeShader.shaderAnimSteps;
			}
			return Mathf.Lerp(0f, (float)this.chargeShader.shaderAnimSteps, Mathf.InverseLerp(0f, this.maxButtonPressDuration, Mathf.Clamp(this.pressCounter, 0f, this.maxButtonPressDuration)));
		}

		// Token: 0x06005869 RID: 22633 RVA: 0x001B30E2 File Offset: 0x001B12E2
		private void TriggerShoot()
		{
			this.Shoot();
			this.pressCounter = 0f;
		}

		// Token: 0x0600586A RID: 22634 RVA: 0x001B30F5 File Offset: 0x001B12F5
		public void OnButtonPressed()
		{
			this.pressStarted = true;
			if (this.launchActivatorType == ProjectileShooterCosmetic.LaunchActivator.ButtonPressed)
			{
				this.TriggerShoot();
			}
		}

		// Token: 0x0600586B RID: 22635 RVA: 0x001B310D File Offset: 0x001B130D
		public void OnButtonReleased()
		{
			this.pressStarted = false;
			if (this.chargeShader)
			{
				this.chargeShader.UpdateChargeProgress(0f);
			}
			if (this.launchActivatorType == ProjectileShooterCosmetic.LaunchActivator.ButtonReleased)
			{
				this.TriggerShoot();
			}
		}

		// Token: 0x04005D94 RID: 23956
		[SerializeField]
		private float cooldown;

		// Token: 0x04005D95 RID: 23957
		[SerializeField]
		private GameObject projectilePrefab;

		// Token: 0x04005D96 RID: 23958
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x04005D97 RID: 23959
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005D98 RID: 23960
		[SerializeField]
		private SoundBankPlayer shootSoundBank;

		// Token: 0x04005D99 RID: 23961
		[SerializeField]
		private SoundBankPlayer readyToShootSoundBank;

		// Token: 0x04005D9A RID: 23962
		[SerializeField]
		private SoundBankPlayer chargingSoundBank;

		// Token: 0x04005D9B RID: 23963
		[SerializeField]
		private float launchMinSpeed;

		// Token: 0x04005D9C RID: 23964
		[SerializeField]
		private float launchMaxSpeed;

		// Token: 0x04005D9D RID: 23965
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x04005D9E RID: 23966
		[SerializeField]
		private ProjectileShooterCosmetic.LaunchActivator launchActivatorType;

		// Token: 0x04005D9F RID: 23967
		[SerializeField]
		private bool useButtonPressDurationAsVelocityModifier;

		// Token: 0x04005DA0 RID: 23968
		[SerializeField]
		private float maxButtonPressDuration = 200f;

		// Token: 0x04005DA1 RID: 23969
		public UnityEvent<bool, float> onOwnerLaunchProjectile;

		// Token: 0x04005DA2 RID: 23970
		private int projectileHash;

		// Token: 0x04005DA3 RID: 23971
		private float launchedTime;

		// Token: 0x04005DA4 RID: 23972
		private bool canShoot;

		// Token: 0x04005DA5 RID: 23973
		private float pressStartedTime;

		// Token: 0x04005DA6 RID: 23974
		private bool pressStarted;

		// Token: 0x04005DA7 RID: 23975
		private float pressCounter;

		// Token: 0x04005DA8 RID: 23976
		private TransferrableObject transferrableObject;

		// Token: 0x04005DA9 RID: 23977
		private ProjectileChargeShader chargeShader;

		// Token: 0x02000DF3 RID: 3571
		private enum LaunchActivator
		{
			// Token: 0x04005DAC RID: 23980
			ButtonReleased,
			// Token: 0x04005DAD RID: 23981
			ButtonPressed,
			// Token: 0x04005DAE RID: 23982
			ButtonStayed
		}
	}
}
