using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000AFD RID: 2813
	[NetworkBehaviourWeaved(1)]
	public class ChristmasTree : NetworkComponent
	{
		// Token: 0x060044D0 RID: 17616 RVA: 0x00146138 File Offset: 0x00144338
		protected override void Awake()
		{
			base.Awake();
			foreach (AttachPoint attachPoint in this.hangers.GetComponentsInChildren<AttachPoint>())
			{
				this.attachPointsList.Add(attachPoint);
				AttachPoint attachPoint2 = attachPoint;
				attachPoint2.onHookedChanged = (UnityAction)Delegate.Combine(attachPoint2.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.lightRenderers = this.lights.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = this.lightRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = this.lightsOffMaterial;
			}
			this.wasActive = false;
			this.isActive = false;
		}

		// Token: 0x060044D1 RID: 17617 RVA: 0x001461D9 File Offset: 0x001443D9
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, Space.World);
			}
		}

		// Token: 0x060044D2 RID: 17618 RVA: 0x00146218 File Offset: 0x00144418
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x060044D3 RID: 17619 RVA: 0x00146298 File Offset: 0x00144498
		private void UpdateHangers()
		{
			if (this.attachPointsList.Count == 0)
			{
				return;
			}
			using (List<AttachPoint>.Enumerator enumerator = this.attachPointsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHooked())
					{
						if (base.IsMine)
						{
							this.updateLight(true);
						}
						return;
					}
				}
			}
			if (base.IsMine)
			{
				this.updateLight(false);
			}
		}

		// Token: 0x060044D4 RID: 17620 RVA: 0x00146318 File Offset: 0x00144518
		private void updateLight(bool enable)
		{
			this.isActive = enable;
			for (int i = 0; i < this.lightRenderers.Length; i++)
			{
				this.lightRenderers[i].material = (enable ? this.lightsOnMaterials[i % this.lightsOnMaterials.Length] : this.lightsOffMaterial);
			}
			this.spinTheTop = enable;
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x060044D5 RID: 17621 RVA: 0x0014636F File Offset: 0x0014456F
		// (set) Token: 0x060044D6 RID: 17622 RVA: 0x00146399 File Offset: 0x00144599
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe NetworkBool Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkBool*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkBool*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x060044D7 RID: 17623 RVA: 0x001463C4 File Offset: 0x001445C4
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x060044D8 RID: 17624 RVA: 0x001463D7 File Offset: 0x001445D7
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x060044D9 RID: 17625 RVA: 0x00146410 File Offset: 0x00144610
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x060044DA RID: 17626 RVA: 0x00146434 File Offset: 0x00144634
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			this.wasActive = this.isActive;
			this.isActive = (bool)stream.ReceiveNext();
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x060044DC RID: 17628 RVA: 0x001464A4 File Offset: 0x001446A4
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060044DD RID: 17629 RVA: 0x001464BC File Offset: 0x001446BC
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400478E RID: 18318
		public GameObject hangers;

		// Token: 0x0400478F RID: 18319
		public GameObject lights;

		// Token: 0x04004790 RID: 18320
		public GameObject topOrnament;

		// Token: 0x04004791 RID: 18321
		public float spinSpeed = 60f;

		// Token: 0x04004792 RID: 18322
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04004793 RID: 18323
		private MeshRenderer[] lightRenderers;

		// Token: 0x04004794 RID: 18324
		private bool wasActive;

		// Token: 0x04004795 RID: 18325
		private bool isActive;

		// Token: 0x04004796 RID: 18326
		private bool spinTheTop;

		// Token: 0x04004797 RID: 18327
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x04004798 RID: 18328
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x04004799 RID: 18329
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkBool _Data;
	}
}
