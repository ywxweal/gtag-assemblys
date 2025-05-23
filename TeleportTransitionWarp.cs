using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class TeleportTransitionWarp : TeleportTransition
{
	// Token: 0x060012AA RID: 4778 RVA: 0x00057D1B File Offset: 0x00055F1B
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.DoWarp());
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00057D2A File Offset: 0x00055F2A
	private IEnumerator DoWarp()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		Vector3 startPosition = base.LocomotionTeleport.GetCharacterPosition();
		float elapsedTime = 0f;
		while (elapsedTime < this.TransitionDuration)
		{
			elapsedTime += Time.deltaTime;
			float num = elapsedTime / this.TransitionDuration;
			float num2 = this.PositionLerp.Evaluate(num);
			base.LocomotionTeleport.DoWarp(startPosition, num2);
			yield return null;
		}
		base.LocomotionTeleport.DoWarp(startPosition, 1f);
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	// Token: 0x040014AE RID: 5294
	[Tooltip("How much time the warp transition takes to complete.")]
	[Range(0.01f, 1f)]
	public float TransitionDuration = 0.5f;

	// Token: 0x040014AF RID: 5295
	[HideInInspector]
	public AnimationCurve PositionLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
