using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class GRMetalEnergyGate : MonoBehaviour
{
	// Token: 0x06002392 RID: 9106 RVA: 0x000B304B File Offset: 0x000B124B
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000B307C File Offset: 0x000B127C
	private void OnDisable()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange -= this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000B30D4 File Offset: 0x000B12D4
	private void OnEnergyChange(GRTool tool, int energyChange)
	{
		if (this.state == GRMetalEnergyGate.State.Closed && tool.energy >= tool.maxEnergy)
		{
			this.SetState(GRMetalEnergyGate.State.Open);
			if (GameEntityManager.instance.IsAuthority())
			{
				GameEntityManager.instance.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000B3125 File Offset: 0x000B1325
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!GameEntityManager.instance.IsAuthority())
		{
			this.SetState((GRMetalEnergyGate.State)nextState);
		}
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000B3140 File Offset: 0x000B1340
	public void SetState(GRMetalEnergyGate.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.audioSource.PlayOneShot(this.doorOpenClip);
					for (int i = 0; i < this.enableObjectsOnOpen.Count; i++)
					{
						this.enableObjectsOnOpen[i].gameObject.SetActive(true);
					}
					for (int j = 0; j < this.disableObjectsOnOpen.Count; j++)
					{
						this.disableObjectsOnOpen[j].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.audioSource.PlayOneShot(this.doorCloseClip);
				for (int k = 0; k < this.enableObjectsOnOpen.Count; k++)
				{
					this.enableObjectsOnOpen[k].gameObject.SetActive(false);
				}
				for (int l = 0; l < this.disableObjectsOnOpen.Count; l++)
				{
					this.disableObjectsOnOpen[l].gameObject.SetActive(true);
				}
			}
			if (this.doorAnimationCoroutine == null)
			{
				this.doorAnimationCoroutine = base.StartCoroutine(this.UpdateDoorAnimation());
			}
		}
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000B3268 File Offset: 0x000B1468
	public void OpenGate()
	{
		this.SetState(GRMetalEnergyGate.State.Open);
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000B3271 File Offset: 0x000B1471
	public void CloseGate()
	{
		this.SetState(GRMetalEnergyGate.State.Closed);
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000B327A File Offset: 0x000B147A
	private IEnumerator UpdateDoorAnimation()
	{
		while ((this.state == GRMetalEnergyGate.State.Open && this.openProgress < 1f) || (this.state == GRMetalEnergyGate.State.Closed && this.openProgress > 0f))
		{
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.openProgress = Mathf.MoveTowards(this.openProgress, 1f, Time.deltaTime / this.doorOpenTime);
					float num = this.doorOpenCurve.Evaluate(this.openProgress);
					this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num);
					this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num);
				}
			}
			else
			{
				this.openProgress = Mathf.MoveTowards(this.openProgress, 0f, Time.deltaTime / this.doorOpenTime);
				float num2 = this.doorCloseCurve.Evaluate(this.openProgress);
				this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, num2);
				this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, num2);
			}
			yield return null;
		}
		this.doorAnimationCoroutine = null;
		yield break;
	}

	// Token: 0x04002841 RID: 10305
	[SerializeField]
	public GRMetalEnergyGate.DoorParams upperDoor;

	// Token: 0x04002842 RID: 10306
	[SerializeField]
	public GRMetalEnergyGate.DoorParams lowerDoor;

	// Token: 0x04002843 RID: 10307
	[SerializeField]
	private float doorOpenTime = 1.5f;

	// Token: 0x04002844 RID: 10308
	[SerializeField]
	private float doorCloseTime = 1.5f;

	// Token: 0x04002845 RID: 10309
	[SerializeField]
	private AnimationCurve doorOpenCurve;

	// Token: 0x04002846 RID: 10310
	[SerializeField]
	private AnimationCurve doorCloseCurve;

	// Token: 0x04002847 RID: 10311
	[SerializeField]
	private AudioClip doorOpenClip;

	// Token: 0x04002848 RID: 10312
	[SerializeField]
	private AudioClip doorCloseClip;

	// Token: 0x04002849 RID: 10313
	[SerializeField]
	private List<Transform> enableObjectsOnOpen = new List<Transform>();

	// Token: 0x0400284A RID: 10314
	[SerializeField]
	private List<Transform> disableObjectsOnOpen = new List<Transform>();

	// Token: 0x0400284B RID: 10315
	[SerializeField]
	private GRTool tool;

	// Token: 0x0400284C RID: 10316
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x0400284D RID: 10317
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400284E RID: 10318
	public GRMetalEnergyGate.State state;

	// Token: 0x0400284F RID: 10319
	private float openProgress;

	// Token: 0x04002850 RID: 10320
	private Coroutine doorAnimationCoroutine;

	// Token: 0x020005B1 RID: 1457
	public enum State
	{
		// Token: 0x04002852 RID: 10322
		Closed,
		// Token: 0x04002853 RID: 10323
		Open
	}

	// Token: 0x020005B2 RID: 1458
	[Serializable]
	public struct DoorParams
	{
		// Token: 0x04002854 RID: 10324
		public Transform doorTransform;

		// Token: 0x04002855 RID: 10325
		public Transform doorClosedPosition;

		// Token: 0x04002856 RID: 10326
		public Transform doorOpenPosition;
	}
}
