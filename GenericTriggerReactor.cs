using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000097 RID: 151
public class GenericTriggerReactor : MonoBehaviour, IBuildValidation
{
	// Token: 0x060003B7 RID: 951 RVA: 0x00016D0E File Offset: 0x00014F0E
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.ComponentName.Length == 0)
		{
			return true;
		}
		if (Type.GetType(this.ComponentName) == null)
		{
			Debug.LogError("GenericTriggerReactor :: ComponentName must specify a valid Component or be empty.");
			return false;
		}
		return true;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00016D3F File Offset: 0x00014F3F
	private void Awake()
	{
		this.componentType = Type.GetType(this.ComponentName);
		base.TryGetComponent<GorillaVelocityEstimator>(out this.gorillaVelocityEstimator);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00016D5F File Offset: 0x00014F5F
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeEnter, this.GTOnTriggerEnter, this.idealMotionPlayRangeEnter);
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00016D7A File Offset: 0x00014F7A
	private void OnTriggerExit(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeExit, this.GTOnTriggerExit, this.idealMotionPlayRangeExit);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x00016D98 File Offset: 0x00014F98
	private void OnTriggerTest(Collider other, Vector2 speedRange, UnityEvent unityEvent, Vector2 idealMotionPlay)
	{
		Component component;
		if (unityEvent != null && (this.componentType == null || other.TryGetComponent(this.componentType, out component)))
		{
			if (this.gorillaVelocityEstimator != null)
			{
				float magnitude = this.gorillaVelocityEstimator.linearVelocity.magnitude;
				if (magnitude < speedRange.x || magnitude > speedRange.y)
				{
					return;
				}
				if (this.idealMotion != null)
				{
					float num = Vector3.Dot(this.gorillaVelocityEstimator.linearVelocity.normalized, this.idealMotion.forward);
					if (num < idealMotionPlay.x || num > idealMotionPlay.y)
					{
						return;
					}
				}
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x04000435 RID: 1077
	[SerializeField]
	private string ComponentName = string.Empty;

	// Token: 0x04000436 RID: 1078
	[Space]
	[SerializeField]
	private Vector2 speedRangeEnter;

	// Token: 0x04000437 RID: 1079
	[SerializeField]
	private Vector2 speedRangeExit;

	// Token: 0x04000438 RID: 1080
	[Space]
	[SerializeField]
	private Transform idealMotion;

	// Token: 0x04000439 RID: 1081
	[SerializeField]
	private Vector2 idealMotionPlayRangeEnter;

	// Token: 0x0400043A RID: 1082
	[SerializeField]
	private Vector2 idealMotionPlayRangeExit;

	// Token: 0x0400043B RID: 1083
	[Space]
	[SerializeField]
	private UnityEvent GTOnTriggerEnter;

	// Token: 0x0400043C RID: 1084
	[SerializeField]
	private UnityEvent GTOnTriggerExit;

	// Token: 0x0400043D RID: 1085
	private Type componentType;

	// Token: 0x0400043E RID: 1086
	private GorillaVelocityEstimator gorillaVelocityEstimator;
}
