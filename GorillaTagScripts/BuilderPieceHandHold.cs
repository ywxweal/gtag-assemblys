using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD2 RID: 2770
	[RequireComponent(typeof(Collider))]
	public class BuilderPieceHandHold : MonoBehaviour, IGorillaGrabable, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x060042F3 RID: 17139 RVA: 0x00135735 File Offset: 0x00133935
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.myCollider = base.GetComponent<Collider>();
			this.initialized = true;
		}

		// Token: 0x060042F4 RID: 17140 RVA: 0x00135753 File Offset: 0x00133953
		public bool IsHandHoldMoving()
		{
			return this.myPiece.IsPieceMoving();
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x00135760 File Offset: 0x00133960
		public bool MomentaryGrabOnly()
		{
			return this.forceMomentary;
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x00135768 File Offset: 0x00133968
		public virtual bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && (!this.myPiece.GetTable().isTableMutable || grabber.Player.scale < 0.5f);
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x001357A0 File Offset: 0x001339A0
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Initialize();
			grabbedTransform = base.transform;
			Vector3 position = grabber.transform.position;
			localGrabbedPosition = base.transform.InverseTransformPoint(position);
			this.activeGrabbers.Add(grabber);
			this.isGrabbed = true;
			Vector3 vector;
			grabber.Player.AddHandHold(base.transform, localGrabbedPosition, grabber, grabber.IsRightHand, false, out vector);
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x0013580D File Offset: 0x00133A0D
		public void OnGrabReleased(GorillaGrabber grabber)
		{
			this.Initialize();
			this.activeGrabbers.Remove(grabber);
			this.isGrabbed = this.activeGrabbers.Count < 1;
			grabber.Player.RemoveHandHold(grabber, grabber.IsRightHand);
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x060042F9 RID: 17145 RVA: 0x00135848 File Offset: 0x00133A48
		// (set) Token: 0x060042FA RID: 17146 RVA: 0x00135850 File Offset: 0x00133A50
		public bool TickRunning { get; set; }

		// Token: 0x060042FB RID: 17147 RVA: 0x0013585C File Offset: 0x00133A5C
		public void Tick()
		{
			if (!this.isGrabbed)
			{
				return;
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				if (gorillaGrabber != null && gorillaGrabber.Player.scale > 0.5f)
				{
					this.OnGrabReleased(gorillaGrabber);
				}
			}
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060042FE RID: 17150 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x001358D4 File Offset: 0x00133AD4
		public void OnPieceActivate()
		{
			if (!this.TickRunning && this.myPiece.GetTable().isTableMutable)
			{
				TickSystem<object>.AddCallbackTarget(this);
			}
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x001358F8 File Offset: 0x00133AF8
		public void OnPieceDeactivate()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				this.OnGrabReleased(gorillaGrabber);
			}
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x0001396B File Offset: 0x00011B6B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x0400457D RID: 17789
		private bool initialized;

		// Token: 0x0400457E RID: 17790
		private Collider myCollider;

		// Token: 0x0400457F RID: 17791
		[SerializeField]
		private bool forceMomentary = true;

		// Token: 0x04004580 RID: 17792
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004581 RID: 17793
		private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>(2);

		// Token: 0x04004582 RID: 17794
		private bool isGrabbed;
	}
}
