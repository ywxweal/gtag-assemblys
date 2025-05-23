using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class GhostPal : MonoBehaviour
{
	// Token: 0x0600044D RID: 1101 RVA: 0x00018F21 File Offset: 0x00017121
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00018F5D File Offset: 0x0001715D
	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00018F6C File Offset: 0x0001716C
	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float num2 = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * num2;
		vector.y = num + this.bounceHeight;
		double num3 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : (-1))) : Time.timeAsDouble);
		Vector3 vector2 = new Vector3(this.orbitRadius * (float)Math.Cos(num3), 0f, this.orbitRadius * (float)Math.Sin(num3));
		Vector3 vector3 = position + vector + vector2;
		Vector3 vector4 = vector3 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, vector4.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.Play();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector3 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float num4 = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector3, num4);
		}
		Quaternion quaternion = Quaternion.Slerp(Quaternion.LookRotation(vector3 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-vector4, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector3, quaternion);
	}

	// Token: 0x040004E6 RID: 1254
	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	// Token: 0x040004E7 RID: 1255
	[SerializeField]
	private float orbitRadius = 1f;

	// Token: 0x040004E8 RID: 1256
	[SerializeField]
	private float orbitHeight = 1f;

	// Token: 0x040004E9 RID: 1257
	[SerializeField]
	private float orbitSpeed = 0.1f;

	// Token: 0x040004EA RID: 1258
	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	// Token: 0x040004EB RID: 1259
	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	// Token: 0x040004EC RID: 1260
	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	// Token: 0x040004ED RID: 1261
	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	// Token: 0x040004EE RID: 1262
	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	// Token: 0x040004EF RID: 1263
	[SerializeField]
	private AudioSource triggerAudioSource;

	// Token: 0x040004F0 RID: 1264
	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	// Token: 0x040004F1 RID: 1265
	[SerializeField]
	private AudioClip[] triggerAudioClips;

	// Token: 0x040004F2 RID: 1266
	private VRRig rig;

	// Token: 0x040004F3 RID: 1267
	private Animator animator;

	// Token: 0x040004F4 RID: 1268
	private float lookAtTime;

	// Token: 0x040004F5 RID: 1269
	private bool hasTriggered;

	// Token: 0x040004F6 RID: 1270
	private Coroutine bounceCoroutine;

	// Token: 0x040004F7 RID: 1271
	private float bounceHeight;

	// Token: 0x040004F8 RID: 1272
	private Vector3 trailingPosition;

	// Token: 0x040004F9 RID: 1273
	private int triggerAudioClipIndex;

	// Token: 0x040004FA RID: 1274
	private int neutralAnimID = Animator.StringToHash("Neutral");

	// Token: 0x040004FB RID: 1275
	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
