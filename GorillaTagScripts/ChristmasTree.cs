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
		// Token: 0x060044CF RID: 17615 RVA: 0x00146060 File Offset: 0x00144260
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

		// Token: 0x060044D0 RID: 17616 RVA: 0x00146101 File Offset: 0x00144301
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, Space.World);
			}
		}

		// Token: 0x060044D1 RID: 17617 RVA: 0x00146140 File Offset: 0x00144340
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x060044D2 RID: 17618 RVA: 0x001461C0 File Offset: 0x001443C0
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

		// Token: 0x060044D3 RID: 17619 RVA: 0x00146240 File Offset: 0x00144440
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
		// (get) Token: 0x060044D4 RID: 17620 RVA: 0x00146297 File Offset: 0x00144497
		// (set) Token: 0x060044D5 RID: 17621 RVA: 0x001462C1 File Offset: 0x001444C1
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

		// Token: 0x060044D6 RID: 17622 RVA: 0x001462EC File Offset: 0x001444EC
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x060044D7 RID: 17623 RVA: 0x001462FF File Offset: 0x001444FF
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x060044D8 RID: 17624 RVA: 0x00146338 File Offset: 0x00144538
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x060044D9 RID: 17625 RVA: 0x0014635C File Offset: 0x0014455C
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

		// Token: 0x060044DB RID: 17627 RVA: 0x001463CC File Offset: 0x001445CC
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060044DC RID: 17628 RVA: 0x001463E4 File Offset: 0x001445E4
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400478D RID: 18317
		public GameObject hangers;

		// Token: 0x0400478E RID: 18318
		public GameObject lights;

		// Token: 0x0400478F RID: 18319
		public GameObject topOrnament;

		// Token: 0x04004790 RID: 18320
		public float spinSpeed = 60f;

		// Token: 0x04004791 RID: 18321
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04004792 RID: 18322
		private MeshRenderer[] lightRenderers;

		// Token: 0x04004793 RID: 18323
		private bool wasActive;

		// Token: 0x04004794 RID: 18324
		private bool isActive;

		// Token: 0x04004795 RID: 18325
		private bool spinTheTop;

		// Token: 0x04004796 RID: 18326
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x04004797 RID: 18327
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x04004798 RID: 18328
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkBool _Data;
	}
}
