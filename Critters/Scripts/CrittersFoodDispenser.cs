using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x02000E1B RID: 3611
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x06005A70 RID: 23152 RVA: 0x001B906A File Offset: 0x001B726A
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x06005A71 RID: 23153 RVA: 0x001B9079 File Offset: 0x001B7279
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06005A72 RID: 23154 RVA: 0x001B9094 File Offset: 0x001B7294
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06005A73 RID: 23155 RVA: 0x001B90A9 File Offset: 0x001B72A9
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x06005A74 RID: 23156 RVA: 0x001B90BF File Offset: 0x001B72BF
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x04005E93 RID: 24211
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
