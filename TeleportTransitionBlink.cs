using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class TeleportTransitionBlink : TeleportTransition
{
	// Token: 0x0600129F RID: 4767 RVA: 0x00057B88 File Offset: 0x00055D88
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.BlinkCoroutine());
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x00057B97 File Offset: 0x00055D97
	protected IEnumerator BlinkCoroutine()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		float elapsedTime = 0f;
		float teleportTime = this.TransitionDuration * this.TeleportDelay;
		bool teleported = false;
		while (elapsedTime < this.TransitionDuration)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			if (!teleported && elapsedTime >= teleportTime)
			{
				teleported = true;
				base.LocomotionTeleport.DoTeleport();
			}
		}
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	// Token: 0x040014A5 RID: 5285
	[Tooltip("How long the transition takes. Usually this is greater than Teleport Delay.")]
	[Range(0.01f, 2f)]
	public float TransitionDuration = 0.5f;

	// Token: 0x040014A6 RID: 5286
	[Tooltip("At what percentage of the elapsed transition time does the teleport occur?")]
	[Range(0f, 1f)]
	public float TeleportDelay = 0.5f;

	// Token: 0x040014A7 RID: 5287
	[Tooltip("Fade to black over the duration of the transition")]
	public AnimationCurve FadeLevels = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});
}
