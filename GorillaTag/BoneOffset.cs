using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D04 RID: 3332
	[Serializable]
	public struct BoneOffset
	{
		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060053A1 RID: 21409 RVA: 0x00196230 File Offset: 0x00194430
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x060053A2 RID: 21410 RVA: 0x0019623D File Offset: 0x0019443D
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x060053A3 RID: 21411 RVA: 0x0019624A File Offset: 0x0019444A
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x060053A4 RID: 21412 RVA: 0x00196257 File Offset: 0x00194457
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x060053A5 RID: 21413 RVA: 0x00196270 File Offset: 0x00194470
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x060053A6 RID: 21414 RVA: 0x00196285 File Offset: 0x00194485
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x060053A7 RID: 21415 RVA: 0x001962A0 File Offset: 0x001944A0
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x060053A8 RID: 21416 RVA: 0x001962BB File Offset: 0x001944BB
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x001962D8 File Offset: 0x001944D8
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles, scale);
		}

		// Token: 0x040056C4 RID: 22212
		public GTHardCodedBones.SturdyEBone bone;

		// Token: 0x040056C5 RID: 22213
		public XformOffset offset;

		// Token: 0x040056C6 RID: 22214
		public static readonly BoneOffset Identity = new BoneOffset
		{
			bone = GTHardCodedBones.EBone.None,
			offset = XformOffset.Identity
		};
	}
}
