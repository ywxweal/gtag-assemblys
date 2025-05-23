using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000225 RID: 549
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x06000CC8 RID: 3272 RVA: 0x00042C28 File Offset: 0x00040E28
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = this.ownerRig != null;
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = this.audioSource != null;
		this.assignedSlotBitIndex = (int)this.assignedSlot;
		if (this.oneShot)
		{
			this.toggleCooldownRange.x = this.toggleCooldownRange.x + this.animationTransitionDuration;
			this.toggleCooldownRange.y = this.toggleCooldownRange.y + this.animationTransitionDuration;
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00042D30 File Offset: 0x00040F30
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.localScale.x, this.colliders, this.layerMask) > 0 && this.toggleCooldownTimer < 0f)
			{
				XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					this.LocalToggle(componentInParent.controllerNode == XRNode.LeftHand, true);
				}
				this.toggleCooldownTimer = Random.Range(this.toggleCooldownRange.x, this.toggleCooldownRange.y);
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & (1 << this.assignedSlotBitIndex)) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag, true);
			}
		}
		if (this.oneShot)
		{
			if (this.isOn)
			{
				this.progress = Mathf.MoveTowards(this.progress, 1f, Time.deltaTime / this.animationTransitionDuration);
				if (this.progress == 1f)
				{
					if (this.ownerIsLocal)
					{
						this.LocalToggle(false, false);
					}
					else
					{
						this.SharedSetState(false, false);
					}
					this.progress = 0f;
				}
			}
		}
		else
		{
			this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		}
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00042EE8 File Offset: 0x000410E8
	private void LocalToggle(bool isLeftHand, bool playAudio)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & (1 << this.assignedSlotBitIndex)) != 0, playAudio);
		if (playAudio && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00042F7C File Offset: 0x0004117C
	private void SharedSetState(bool state, bool playAudio)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (!playAudio || !this.hasAudioSource)
		{
			return;
		}
		AudioClip audioClip = (this.isOn ? this.toggleOnSound : this.toggleOffSound);
		if (audioClip == null)
		{
			return;
		}
		if (this.oneShot)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.GTPlay();
			return;
		}
		this.audioSource.GTPlayOneShot(audioClip, 1f);
	}

	// Token: 0x04000F4A RID: 3914
	public Renderer[] renderers;

	// Token: 0x04000F4B RID: 3915
	public Animator[] animators;

	// Token: 0x04000F4C RID: 3916
	public float animationTransitionDuration = 1f;

	// Token: 0x04000F4D RID: 3917
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x04000F4E RID: 3918
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x04000F4F RID: 3919
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x04000F50 RID: 3920
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x04000F51 RID: 3921
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x04000F52 RID: 3922
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x04000F53 RID: 3923
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x04000F54 RID: 3924
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x04000F55 RID: 3925
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x04000F56 RID: 3926
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x04000F57 RID: 3927
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x04000F58 RID: 3928
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x04000F59 RID: 3929
	private VRRig ownerRig;

	// Token: 0x04000F5A RID: 3930
	private bool ownerIsLocal;

	// Token: 0x04000F5B RID: 3931
	private bool isOn;

	// Token: 0x04000F5C RID: 3932
	[SerializeField]
	private Vector2 toggleCooldownRange = new Vector2(0.2f, 0.2f);

	// Token: 0x04000F5D RID: 3933
	private bool hasAudioSource;

	// Token: 0x04000F5E RID: 3934
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x04000F5F RID: 3935
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x04000F60 RID: 3936
	private float toggleCooldownTimer;

	// Token: 0x04000F61 RID: 3937
	private int assignedSlotBitIndex;

	// Token: 0x04000F62 RID: 3938
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x04000F63 RID: 3939
	private float progress;

	// Token: 0x04000F64 RID: 3940
	[SerializeField]
	private bool oneShot;
}
