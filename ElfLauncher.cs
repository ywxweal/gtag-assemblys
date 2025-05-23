using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class ElfLauncher : MonoBehaviour
{
	// Token: 0x06000949 RID: 2377 RVA: 0x0003223C File Offset: 0x0003043C
	private void OnEnable()
	{
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((this.parentHoldable.myOnlineRig != null) ? this.parentHoldable.myOnlineRig.creator : ((this.parentHoldable.myRig != null) ? ((this.parentHoldable.myRig.creator != null) ? this.parentHoldable.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
			if (netPlayer != null)
			{
				this.m_player = netPlayer;
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.ShootShared;
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00032328 File Offset: 0x00030528
	private void OnDisable()
	{
		if (this._events != null)
		{
			this._events.Activate -= this.ShootShared;
			this._events.Dispose();
			this._events = null;
			this.m_player = null;
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00032380 File Offset: 0x00030580
	private void Awake()
	{
		this._events = base.GetComponent<RubberDuckEvents>();
		this.elfProjectileHash = PoolUtils.GameObjHashCode(this.elfProjectilePrefab);
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCranked));
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x000323D4 File Offset: 0x000305D4
	private void OnCranked(float deltaAngle)
	{
		this.currentShootCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentShootCrankAmount) > this.crankShootThreshold)
		{
			this.currentShootCrankAmount = 0f;
			this.Shoot();
		}
		this.currentClickCrankAmount += deltaAngle;
		if (Mathf.Abs(this.currentClickCrankAmount) > this.crankClickThreshold)
		{
			this.currentClickCrankAmount = 0f;
			this.crankClickAudio.Play();
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0003244C File Offset: 0x0003064C
	private void Shoot()
	{
		if (this.parentHoldable.IsLocalObject())
		{
			GorillaTagger.Instance.StartVibration(true, this.shootHapticStrength, this.shootHapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.shootHapticStrength, this.shootHapticDuration);
			if (PhotonNetwork.InRoom)
			{
				this._events.Activate.RaiseAll(new object[]
				{
					this.muzzle.transform.position,
					this.muzzle.transform.forward
				});
				return;
			}
			this.ShootShared(this.muzzle.transform.position, this.muzzle.transform.forward);
		}
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0003250C File Offset: 0x0003070C
	private void ShootShared(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (args.Length != 2)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		VRRig ownerRig = this.parentHoldable.ownerRig;
		if (info.senderID != ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length == 2)
		{
			object obj = args[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				obj = args[1];
				if (obj is Vector3)
				{
					Vector3 vector2 = (Vector3)obj;
					float num = 10000f;
					if ((in vector).IsValid(in num))
					{
						float num2 = 10000f;
						if ((in vector2).IsValid(in num2))
						{
							if (!FXSystem.CheckCallSpam(ownerRig.fxSettings, 4, info.SentServerTime) || !ownerRig.IsPositionInRange(vector, 6f))
							{
								return;
							}
							this.ShootShared(vector, vector2);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000325C4 File Offset: 0x000307C4
	private void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().velocity = direction * this.muzzleVelocity * lossyScale.x;
	}

	// Token: 0x04000B25 RID: 2853
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04000B26 RID: 2854
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x04000B27 RID: 2855
	[SerializeField]
	private float crankShootThreshold = 360f;

	// Token: 0x04000B28 RID: 2856
	[SerializeField]
	private float crankClickThreshold = 30f;

	// Token: 0x04000B29 RID: 2857
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04000B2A RID: 2858
	[SerializeField]
	private GameObject elfProjectilePrefab;

	// Token: 0x04000B2B RID: 2859
	private int elfProjectileHash;

	// Token: 0x04000B2C RID: 2860
	[SerializeField]
	private float muzzleVelocity = 10f;

	// Token: 0x04000B2D RID: 2861
	[SerializeField]
	private SoundBankPlayer crankClickAudio;

	// Token: 0x04000B2E RID: 2862
	[SerializeField]
	private SoundBankPlayer shootAudio;

	// Token: 0x04000B2F RID: 2863
	[SerializeField]
	private float shootHapticStrength;

	// Token: 0x04000B30 RID: 2864
	[SerializeField]
	private float shootHapticDuration;

	// Token: 0x04000B31 RID: 2865
	private RubberDuckEvents _events;

	// Token: 0x04000B32 RID: 2866
	private float currentShootCrankAmount;

	// Token: 0x04000B33 RID: 2867
	private float currentClickCrankAmount;

	// Token: 0x04000B34 RID: 2868
	private NetPlayer m_player;
}
