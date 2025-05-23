using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E4D RID: 3661
	public class BoingBehavior : BoingBase
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06005BA3 RID: 23459 RVA: 0x001C24A7 File Offset: 0x001C06A7
		// (set) Token: 0x06005BA4 RID: 23460 RVA: 0x001C24B9 File Offset: 0x001C06B9
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06005BA5 RID: 23461 RVA: 0x001C24D3 File Offset: 0x001C06D3
		// (set) Token: 0x06005BA6 RID: 23462 RVA: 0x001C24E5 File Offset: 0x001C06E5
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06005BA7 RID: 23463 RVA: 0x001C24FF File Offset: 0x001C06FF
		// (set) Token: 0x06005BA8 RID: 23464 RVA: 0x001C2511 File Offset: 0x001C0711
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x06005BA9 RID: 23465 RVA: 0x001C252B File Offset: 0x001C072B
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x06005BAA RID: 23466 RVA: 0x001C2554 File Offset: 0x001C0754
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x06005BAB RID: 23467 RVA: 0x001C261D File Offset: 0x001C081D
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x06005BAC RID: 23468 RVA: 0x001C2633 File Offset: 0x001C0833
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x06005BAD RID: 23469 RVA: 0x001C263C File Offset: 0x001C083C
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x06005BAE RID: 23470 RVA: 0x001C2644 File Offset: 0x001C0844
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x001C264C File Offset: 0x001C084C
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005BB0 RID: 23472 RVA: 0x001C2654 File Offset: 0x001C0854
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x06005BB1 RID: 23473 RVA: 0x001C2753 File Offset: 0x001C0953
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x06005BB2 RID: 23474 RVA: 0x001C275C File Offset: 0x001C095C
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x06005BB3 RID: 23475 RVA: 0x001C27D2 File Offset: 0x001C09D2
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x06005BB4 RID: 23476 RVA: 0x001C27E0 File Offset: 0x001C09E0
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x001C27F0 File Offset: 0x001C09F0
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x06005BB6 RID: 23478 RVA: 0x001C28BC File Offset: 0x001C0ABC
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x06005BB7 RID: 23479 RVA: 0x001C29B4 File Offset: 0x001C0BB4
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x04005F82 RID: 24450
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04005F83 RID: 24451
		public bool TwoDDistanceCheck;

		// Token: 0x04005F84 RID: 24452
		public bool TwoDPositionInfluence;

		// Token: 0x04005F85 RID: 24453
		public bool TwoDRotationInfluence;

		// Token: 0x04005F86 RID: 24454
		public bool EnablePositionEffect = true;

		// Token: 0x04005F87 RID: 24455
		public bool EnableRotationEffect = true;

		// Token: 0x04005F88 RID: 24456
		public bool EnableScaleEffect;

		// Token: 0x04005F89 RID: 24457
		public bool GlobalReactionUpVector;

		// Token: 0x04005F8A RID: 24458
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x04005F8B RID: 24459
		public bool LockTranslationX;

		// Token: 0x04005F8C RID: 24460
		public bool LockTranslationY;

		// Token: 0x04005F8D RID: 24461
		public bool LockTranslationZ;

		// Token: 0x04005F8E RID: 24462
		public BoingWork.Params Params;

		// Token: 0x04005F8F RID: 24463
		public SharedBoingParams SharedParams;

		// Token: 0x04005F90 RID: 24464
		internal bool PositionSpringDirty;

		// Token: 0x04005F91 RID: 24465
		internal bool RotationSpringDirty;

		// Token: 0x04005F92 RID: 24466
		internal bool ScaleSpringDirty;

		// Token: 0x04005F93 RID: 24467
		internal bool CachedTransformValid;

		// Token: 0x04005F94 RID: 24468
		internal Vector3 CachedPositionLs;

		// Token: 0x04005F95 RID: 24469
		internal Vector3 CachedPositionWs;

		// Token: 0x04005F96 RID: 24470
		internal Vector3 RenderPositionWs;

		// Token: 0x04005F97 RID: 24471
		internal Quaternion CachedRotationLs;

		// Token: 0x04005F98 RID: 24472
		internal Quaternion CachedRotationWs;

		// Token: 0x04005F99 RID: 24473
		internal Quaternion RenderRotationWs;

		// Token: 0x04005F9A RID: 24474
		internal Vector3 CachedScaleLs;

		// Token: 0x04005F9B RID: 24475
		internal Vector3 RenderScaleLs;

		// Token: 0x04005F9C RID: 24476
		internal bool InitRebooted;
	}
}
