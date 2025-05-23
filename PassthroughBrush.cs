using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class PassthroughBrush : MonoBehaviour
{
	// Token: 0x06001397 RID: 5015 RVA: 0x0005F320 File Offset: 0x0005D520
	private void OnDisable()
	{
		this.brushStatus = PassthroughBrush.BrushState.Idle;
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0005F32C File Offset: 0x0005D52C
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - Camera.main.transform.position);
		if (this.controllerHand != OVRInput.Controller.LTouch && this.controllerHand != OVRInput.Controller.RTouch)
		{
			return;
		}
		Vector3 position = base.transform.position;
		PassthroughBrush.BrushState brushState = this.brushStatus;
		if (brushState != PassthroughBrush.BrushState.Idle)
		{
			if (brushState != PassthroughBrush.BrushState.Inking)
			{
				return;
			}
			this.UpdateLine(position);
			if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.brushStatus = PassthroughBrush.BrushState.Idle;
			}
		}
		else
		{
			if (OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
			{
				this.UndoInkLine();
			}
			if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.StartLine(position);
				this.brushStatus = PassthroughBrush.BrushState.Inking;
				return;
			}
		}
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x0005F3EC File Offset: 0x0005D5EC
	private void StartLine(Vector3 inkPos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.lineSegmentPrefab, inkPos, Quaternion.identity);
		this.currentLineSegment = gameObject.GetComponent<LineRenderer>();
		this.currentLineSegment.positionCount = 1;
		this.currentLineSegment.SetPosition(0, inkPos);
		this.strokeWidth = this.currentLineSegment.startWidth;
		this.strokeLength = 0f;
		this.inkPositions.Clear();
		this.inkPositions.Add(inkPos);
		gameObject.transform.parent = this.lineContainer.transform;
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0005F47C File Offset: 0x0005D67C
	private void UpdateLine(Vector3 inkPos)
	{
		float magnitude = (inkPos - this.inkPositions[this.inkPositions.Count - 1]).magnitude;
		if (magnitude >= this.minInkDist)
		{
			this.inkPositions.Add(inkPos);
			this.currentLineSegment.positionCount = this.inkPositions.Count;
			this.currentLineSegment.SetPositions(this.inkPositions.ToArray());
			this.strokeLength += magnitude;
			this.currentLineSegment.material.SetFloat("_LineLength", this.strokeLength / this.strokeWidth);
		}
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0005F524 File Offset: 0x0005D724
	public void ClearLines()
	{
		for (int i = 0; i < this.lineContainer.transform.childCount; i++)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0005F568 File Offset: 0x0005D768
	public void UndoInkLine()
	{
		if (this.lineContainer.transform.childCount >= 1)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(this.lineContainer.transform.childCount - 1).gameObject);
		}
	}

	// Token: 0x040015C7 RID: 5575
	public OVRInput.Controller controllerHand;

	// Token: 0x040015C8 RID: 5576
	public GameObject lineSegmentPrefab;

	// Token: 0x040015C9 RID: 5577
	public GameObject lineContainer;

	// Token: 0x040015CA RID: 5578
	public bool forceActive = true;

	// Token: 0x040015CB RID: 5579
	private LineRenderer currentLineSegment;

	// Token: 0x040015CC RID: 5580
	private List<Vector3> inkPositions = new List<Vector3>();

	// Token: 0x040015CD RID: 5581
	private float minInkDist = 0.01f;

	// Token: 0x040015CE RID: 5582
	private float strokeWidth = 0.1f;

	// Token: 0x040015CF RID: 5583
	private float strokeLength;

	// Token: 0x040015D0 RID: 5584
	private PassthroughBrush.BrushState brushStatus;

	// Token: 0x0200033E RID: 830
	public enum BrushState
	{
		// Token: 0x040015D2 RID: 5586
		Idle,
		// Token: 0x040015D3 RID: 5587
		Inking
	}
}
