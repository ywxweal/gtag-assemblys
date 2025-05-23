using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x02000E1B RID: 3611
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x06005A6F RID: 23151 RVA: 0x001B8F92 File Offset: 0x001B7192
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x06005A70 RID: 23152 RVA: 0x001B8FA1 File Offset: 0x001B71A1
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06005A71 RID: 23153 RVA: 0x001B8FBC File Offset: 0x001B71BC
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x06005A72 RID: 23154 RVA: 0x001B8FD1 File Offset: 0x001B71D1
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x06005A73 RID: 23155 RVA: 0x001B8FE7 File Offset: 0x001B71E7
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x04005E92 RID: 24210
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
