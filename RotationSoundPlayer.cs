using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class RotationSoundPlayer : MonoBehaviour
{
	// Token: 0x0600062B RID: 1579 RVA: 0x000237E4 File Offset: 0x000219E4
	private void Awake()
	{
		List<Transform> list = new List<Transform>(this.transforms);
		list.RemoveAll((Transform xform) => xform == null);
		this.transforms = list.ToArray();
		this.initialUpAxis = new Vector3[this.transforms.Length];
		this.lastUpAxis = new Vector3[this.transforms.Length];
		this.lastRotationSpeeds = new float[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.initialUpAxis[i] = this.transforms[i].localRotation * Vector3.up;
			this.lastUpAxis[i] = this.initialUpAxis[i];
			this.lastRotationSpeeds[i] = 0f;
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000238C4 File Offset: 0x00021AC4
	private void Update()
	{
		this.cooldownTimer -= Time.deltaTime;
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Vector3 vector = this.transforms[i].localRotation * Vector3.up;
			float num = Vector3.Angle(vector, this.initialUpAxis[i]);
			float num2 = Vector3.Angle(vector, this.lastUpAxis[i]);
			float deltaTime = Time.deltaTime;
			float num3 = num2 / deltaTime;
			if (this.cooldownTimer <= 0f && num > this.rotationAmountThreshold && num3 > this.rotationSpeedThreshold && !this.soundBankPlayer.isPlaying)
			{
				this.cooldownTimer = this.cooldown;
				this.soundBankPlayer.Play();
			}
			this.lastUpAxis[i] = vector;
			this.lastRotationSpeeds[i] = num3;
		}
	}

	// Token: 0x0400074E RID: 1870
	[Tooltip("Transforms that will make a noise when they rotate.")]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x0400074F RID: 1871
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000750 RID: 1872
	[Tooltip("How much the transform must rotate from it's initial rotation before a sound is played.")]
	private float rotationAmountThreshold = 30f;

	// Token: 0x04000751 RID: 1873
	[Tooltip("How fast the transform must rotate before a sound is played.")]
	private float rotationSpeedThreshold = 45f;

	// Token: 0x04000752 RID: 1874
	private float cooldown = 0.6f;

	// Token: 0x04000753 RID: 1875
	private float cooldownTimer;

	// Token: 0x04000754 RID: 1876
	private Vector3[] initialUpAxis;

	// Token: 0x04000755 RID: 1877
	private Vector3[] lastUpAxis;

	// Token: 0x04000756 RID: 1878
	private float[] lastRotationSpeeds;
}
