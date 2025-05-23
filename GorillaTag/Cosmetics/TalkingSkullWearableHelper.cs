using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DAE RID: 3502
	public class TalkingSkullWearableHelper : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060056C2 RID: 22210 RVA: 0x001A712C File Offset: 0x001A532C
		public void Awake()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x060056C3 RID: 22211 RVA: 0x001A713C File Offset: 0x001A533C
		private void Start()
		{
			this._helpers = new List<TalkingSkullHelper>();
			base.transform.root.GetComponentsInChildren<TalkingSkullHelper>(this._helpers);
			VRRig componentInParent = base.GetComponentInParent<VRRig>();
			this._speakerHeadCollider = ((componentInParent != null) ? componentInParent.rigContainer.HeadCollider : null);
			this._headDistanceSqr = this.HeadDistance * this.HeadDistance;
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x001A719A File Offset: 0x001A539A
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.SetJewelColor(this.JewelColorOff);
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x00010F34 File Offset: 0x0000F134
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x001A71B0 File Offset: 0x001A53B0
		public void SliceUpdate()
		{
			this._deltaTime = Time.time - this._timeLastUpdated;
			this._timeLastUpdated = Time.time;
			if (this._speakerHeadCollider == null)
			{
				return;
			}
			this._toggleCooldown -= Time.deltaTime;
			if (this._toggleCooldown <= 0f)
			{
				if ((base.transform.position - this._speakerHeadCollider.transform.position).sqrMagnitude < this._headDistanceSqr)
				{
					this.ToggleBand(true);
					return;
				}
				this.ToggleBand(false);
			}
		}

		// Token: 0x060056C7 RID: 22215 RVA: 0x001A7248 File Offset: 0x001A5448
		private void ToggleBand(bool toggle)
		{
			if (toggle)
			{
				for (int i = 0; i < this._helpers.Count; i++)
				{
					this._helpers[i].CanTalk(true);
				}
				this.SetJewelColor(this.JewelColorOn);
			}
			else
			{
				for (int j = 0; j < this._helpers.Count; j++)
				{
					this._helpers[j].CanTalk(false);
				}
				this.SetJewelColor(this.JewelColorOff);
			}
			this._toggleCooldown = this.ToggleCooldown;
		}

		// Token: 0x060056C8 RID: 22216 RVA: 0x001A72CE File Offset: 0x001A54CE
		private void SetJewelColor(Color jewelColor)
		{
			this._materialPropertyBlock.SetColor("_BaseColor", jewelColor);
			this._meshRenderer.SetPropertyBlock(this._materialPropertyBlock, 0);
		}

		// Token: 0x060056CA RID: 22218 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04005AB6 RID: 23222
		public float HeadDistance = 0.5f;

		// Token: 0x04005AB7 RID: 23223
		public float ToggleCooldown = 0.5f;

		// Token: 0x04005AB8 RID: 23224
		public Color JewelColorOff = Color.black;

		// Token: 0x04005AB9 RID: 23225
		public Color JewelColorOn = Color.white;

		// Token: 0x04005ABA RID: 23226
		[SerializeField]
		private List<TalkingSkullHelper> _helpers;

		// Token: 0x04005ABB RID: 23227
		[SerializeField]
		private Collider _speakerHeadCollider;

		// Token: 0x04005ABC RID: 23228
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x04005ABD RID: 23229
		private float _deltaTime;

		// Token: 0x04005ABE RID: 23230
		private float _timeLastUpdated;

		// Token: 0x04005ABF RID: 23231
		private float _headDistanceSqr = 1f;

		// Token: 0x04005AC0 RID: 23232
		private float _toggleCooldown;

		// Token: 0x04005AC1 RID: 23233
		private MaterialPropertyBlock _materialPropertyBlock;
	}
}
