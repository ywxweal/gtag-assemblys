using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D81 RID: 3457
	public static class GTHardCodedBones
	{
		// Token: 0x060055EE RID: 21998 RVA: 0x001A236A File Offset: 0x001A056A
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void HandleRuntimeInitialize_OnBeforeSceneLoad()
		{
			VRRigCache.OnPostInitialize += GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x001A237D File Offset: 0x001A057D
		private static void HandleVRRigCache_OnPostInitialize()
		{
			VRRigCache.OnPostInitialize -= GTHardCodedBones.HandleVRRigCache_OnPostInitialize;
			GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig();
			VRRigCache.OnPostSpawnRig += GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig;
		}

		// Token: 0x060055F0 RID: 22000 RVA: 0x001A23A8 File Offset: 0x001A05A8
		private static void HandleVRRigCache_OnPostSpawnRig()
		{
			if (!VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			Transform[] array;
			string text;
			if (!GTHardCodedBones.TryGetBoneXforms(VRRig.LocalRig, out array, out text))
			{
				Debug.LogError("GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig: Error getting bone Transforms: " + text);
			}
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			for (int i = 0; i < allRigs.Length; i++)
			{
				Transform[] array2;
				string text2;
				if (!GTHardCodedBones.TryGetBoneXforms(allRigs[i], out array2, out text2))
				{
					Debug.LogError("GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig: Error getting bone Transforms: " + text, allRigs[i]);
					return;
				}
			}
		}

		// Token: 0x060055F1 RID: 22001 RVA: 0x000430AE File Offset: 0x000412AE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(GTHardCodedBones.EBone bone)
		{
			return (int)bone;
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x001A2420 File Offset: 0x001A0620
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBoneIndex(string name)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x060055F3 RID: 22003 RVA: 0x001A2454 File Offset: 0x001A0654
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneIndexByName(string name, out int out_index)
		{
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					out_index = i;
					return true;
				}
			}
			out_index = 0;
			return false;
		}

		// Token: 0x060055F4 RID: 22004 RVA: 0x001A248B File Offset: 0x001A068B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBone(string name)
		{
			return (GTHardCodedBones.EBone)GTHardCodedBones.GetBoneIndex(name);
		}

		// Token: 0x060055F5 RID: 22005 RVA: 0x001A2494 File Offset: 0x001A0694
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneByName(string name, out GTHardCodedBones.EBone out_eBone)
		{
			int num;
			if (GTHardCodedBones.TryGetBoneIndexByName(name, out num))
			{
				out_eBone = (GTHardCodedBones.EBone)num;
				return true;
			}
			out_eBone = GTHardCodedBones.EBone.None;
			return false;
		}

		// Token: 0x060055F6 RID: 22006 RVA: 0x001A24B4 File Offset: 0x001A06B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(int boneIndex)
		{
			return GTHardCodedBones.kBoneNames[boneIndex];
		}

		// Token: 0x060055F7 RID: 22007 RVA: 0x001A24BD File Offset: 0x001A06BD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(int boneIndex, out string out_name)
		{
			if (boneIndex >= 0 && boneIndex < GTHardCodedBones.kBoneNames.Length)
			{
				out_name = GTHardCodedBones.kBoneNames[boneIndex];
				return true;
			}
			out_name = "None";
			return false;
		}

		// Token: 0x060055F8 RID: 22008 RVA: 0x001A24E0 File Offset: 0x001A06E0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetBoneName(GTHardCodedBones.EBone bone)
		{
			return GTHardCodedBones.GetBoneName((int)bone);
		}

		// Token: 0x060055F9 RID: 22009 RVA: 0x001A24E8 File Offset: 0x001A06E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneName(GTHardCodedBones.EBone bone, out string out_name)
		{
			return GTHardCodedBones.TryGetBoneName((int)bone, out out_name);
		}

		// Token: 0x060055FA RID: 22010 RVA: 0x001A24F4 File Offset: 0x001A06F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(string name)
		{
			if (name == "None")
			{
				return 0L;
			}
			for (int i = 0; i < GTHardCodedBones.kBoneNames.Length; i++)
			{
				if (GTHardCodedBones.kBoneNames[i] == name)
				{
					return 1L << i - 1;
				}
			}
			return 0L;
		}

		// Token: 0x060055FB RID: 22011 RVA: 0x001A253E File Offset: 0x001A073E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetBoneBitFlag(GTHardCodedBones.EBone bone)
		{
			if (bone == GTHardCodedBones.EBone.None)
			{
				return 0L;
			}
			return 1L << bone - GTHardCodedBones.EBone.rig;
		}

		// Token: 0x060055FC RID: 22012 RVA: 0x001A254F File Offset: 0x001A074F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static EHandedness GetHandednessFromBone(GTHardCodedBones.EBone bone)
		{
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1728432283058160L) != 0L)
			{
				return EHandedness.Left;
			}
			if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1769114204897280L) == 0L)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}

		// Token: 0x060055FD RID: 22013 RVA: 0x001A257C File Offset: 0x001A077C
		public static bool TryGetBoneXforms(VRRig vrRig, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
			{
				return false;
			}
			VRRigAnchorOverrides componentInChildren = vrRig.GetComponentInChildren<VRRigAnchorOverrides>(true);
			BodyDockPositions componentInChildren2 = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outBoneXforms[46] = componentInChildren2.leftBackTransform;
			outBoneXforms[47] = componentInChildren2.rightBackTransform;
			outBoneXforms[42] = componentInChildren2.chestTransform;
			outBoneXforms[43] = componentInChildren.CurrentBadgeTransform;
			outBoneXforms[44] = componentInChildren.nameTransform;
			outBoneXforms[52] = componentInChildren.huntComputer;
			outBoneXforms[50] = componentInChildren.friendshipBraceletLeftAnchor;
			outBoneXforms[51] = componentInChildren.friendshipBraceletRightAnchor;
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			return true;
		}

		// Token: 0x060055FE RID: 22014 RVA: 0x001A2648 File Offset: 0x001A0848
		public static bool TryGetSlotAnchorXforms(VRRig vrRig, out Transform[] outSlotXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (vrRig == null)
			{
				outErrorMsg = "The VRRig is null.";
				outSlotXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = vrRig.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_slotXforms.TryGetValue(instanceID, out outSlotXforms))
			{
				return true;
			}
			Transform[] array;
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out array, out outErrorMsg))
			{
				return false;
			}
			outSlotXforms = new Transform[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				outSlotXforms[i] = array[i];
			}
			BodyDockPositions componentInChildren = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			outSlotXforms[7] = componentInChildren.leftArmTransform;
			outSlotXforms[25] = componentInChildren.rightArmTransform;
			outSlotXforms[8] = componentInChildren.leftHandTransform;
			outSlotXforms[26] = componentInChildren.rightHandTransform;
			GTHardCodedBones._gInstIds_To_slotXforms[instanceID] = outSlotXforms;
			return true;
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x001A2700 File Offset: 0x001A0900
		public static bool TryGetBoneXforms(SkinnedMeshRenderer skinnedMeshRenderer, out Transform[] outBoneXforms, out string outErrorMsg)
		{
			outErrorMsg = string.Empty;
			if (skinnedMeshRenderer == null)
			{
				outErrorMsg = "The SkinnedMeshRenderer was null.";
				outBoneXforms = Array.Empty<Transform>();
				return false;
			}
			int instanceID = skinnedMeshRenderer.GetInstanceID();
			if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceID, out outBoneXforms))
			{
				return true;
			}
			GTHardCodedBones._gMissingBonesReport.Clear();
			Transform[] bones = skinnedMeshRenderer.bones;
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i] == null)
				{
					Debug.LogError(string.Format("this should never happen -- skinned mesh bone index {0} is null in component: ", i) + "\"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else if (bones[i].parent == null)
				{
					Debug.LogError(string.Format("unexpected and unhandled scenario -- skinned mesh bone at index {0} has no parent in ", i) + "component: \"" + skinnedMeshRenderer.GetComponentPath(int.MaxValue) + "\"", skinnedMeshRenderer);
				}
				else
				{
					bones[i] = (bones[i].name.EndsWith("_new") ? bones[i].parent : bones[i]);
				}
			}
			outBoneXforms = new Transform[GTHardCodedBones.kBoneNames.Length];
			for (int j = 1; j < GTHardCodedBones.kBoneNames.Length; j++)
			{
				string text = GTHardCodedBones.kBoneNames[j];
				if (!(text == "None") && !text.EndsWith("_end") && !text.Contains("Anchor") && j != 1)
				{
					bool flag = false;
					foreach (Transform transform in bones)
					{
						if (!(transform == null) && !(transform.name != text))
						{
							outBoneXforms[j] = transform;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						GTHardCodedBones._gMissingBonesReport.Add(j);
					}
				}
			}
			for (int l = 1; l < GTHardCodedBones.kBoneNames.Length; l++)
			{
				string text2 = GTHardCodedBones.kBoneNames[l];
				if (text2.EndsWith("_end"))
				{
					string text3 = text2;
					int boneIndex = GTHardCodedBones.GetBoneIndex(text3.Substring(0, text3.Length - 4));
					if (boneIndex < 0)
					{
						GTHardCodedBones._gMissingBonesReport.Add(l);
					}
					else
					{
						Transform transform2 = outBoneXforms[boneIndex];
						if (transform2 == null)
						{
							GTHardCodedBones._gMissingBonesReport.Add(l);
						}
						else
						{
							Transform transform3 = transform2.Find(text2);
							if (transform3 == null)
							{
								GTHardCodedBones._gMissingBonesReport.Add(l);
							}
							else
							{
								outBoneXforms[l] = transform3;
							}
						}
					}
				}
			}
			Transform transform4 = outBoneXforms[2];
			if (transform4 != null && transform4.parent != null)
			{
				outBoneXforms[1] = transform4.parent;
			}
			else
			{
				GTHardCodedBones._gMissingBonesReport.Add(1);
			}
			for (int m = 1; m < GTHardCodedBones.kBoneNames.Length; m++)
			{
				string text4 = GTHardCodedBones.kBoneNames[m];
				if (text4.Contains("Anchor"))
				{
					Transform transform5;
					if (transform4.TryFindByPath("/**/" + text4, out transform5, false))
					{
						outBoneXforms[m] = transform5;
					}
					else
					{
						GameObject gameObject = new GameObject(text4);
						gameObject.transform.SetParent(transform4, false);
						outBoneXforms[m] = gameObject.transform;
					}
				}
			}
			GTHardCodedBones._gInstIds_To_boneXforms[instanceID] = outBoneXforms;
			if (GTHardCodedBones._gMissingBonesReport.Count == 0)
			{
				return true;
			}
			string text5 = "The SkinnedMeshRenderer on \"" + skinnedMeshRenderer.name + "\" did not have these expected bones: ";
			foreach (int num in GTHardCodedBones._gMissingBonesReport)
			{
				text5 = text5 + "\n- " + GTHardCodedBones.kBoneNames[num];
			}
			outErrorMsg = text5;
			return true;
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x001A2AA0 File Offset: 0x001A0CA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, string boneName, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(boneName)];
			return boneXform != null;
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x001A2AB4 File Offset: 0x001A0CB4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetBoneXform(Transform[] boneXforms, GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(eBone)];
			return boneXform != null;
		}

		// Token: 0x06005602 RID: 22018 RVA: 0x001A2AC8 File Offset: 0x001A0CC8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetFirstBoneInParents(Transform transform, out GTHardCodedBones.EBone eBone, out Transform boneXform)
		{
			while (transform != null)
			{
				string name = transform.name;
				if (name == "DropZoneAnchor" && transform.parent != null)
				{
					string name2 = transform.parent.name;
					if (name2 == "Slingshot Chest Snap")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftArm")
					{
						eBone = GTHardCodedBones.EBone.forearm_L;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemLeftShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
						boneXform = transform;
						return true;
					}
					if (name2 == "TransferrableItemRightShoulder")
					{
						eBone = GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
						boneXform = transform;
						return true;
					}
				}
				else
				{
					if (name == "TransferrableItemLeftHand")
					{
						eBone = GTHardCodedBones.EBone.hand_L;
						boneXform = transform;
						return true;
					}
					if (name == "TransferrableItemRightHand")
					{
						eBone = GTHardCodedBones.EBone.hand_R;
						boneXform = transform;
						return true;
					}
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(transform.name);
				if (bone != GTHardCodedBones.EBone.None)
				{
					eBone = bone;
					boneXform = transform;
					return true;
				}
				transform = transform.parent;
			}
			eBone = GTHardCodedBones.EBone.None;
			boneXform = null;
			return false;
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x001A2BB8 File Offset: 0x001A0DB8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GTHardCodedBones.EBone GetBoneEnumOfCosmeticPosStateFlag(TransferrableObject.PositionState positionState)
		{
			if (positionState <= TransferrableObject.PositionState.OnChest)
			{
				switch (positionState)
				{
				case TransferrableObject.PositionState.None:
					break;
				case TransferrableObject.PositionState.OnLeftArm:
					return GTHardCodedBones.EBone.forearm_L;
				case TransferrableObject.PositionState.OnRightArm:
					return GTHardCodedBones.EBone.forearm_R;
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
				case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm | TransferrableObject.PositionState.InLeftHand:
					goto IL_005F;
				case TransferrableObject.PositionState.InLeftHand:
					return GTHardCodedBones.EBone.hand_L;
				case TransferrableObject.PositionState.InRightHand:
					return GTHardCodedBones.EBone.hand_R;
				default:
					if (positionState != TransferrableObject.PositionState.OnChest)
					{
						goto IL_005F;
					}
					return GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
				}
			}
			else
			{
				if (positionState == TransferrableObject.PositionState.OnLeftShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
				}
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					return GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
				}
				if (positionState != TransferrableObject.PositionState.Dropped)
				{
					goto IL_005F;
				}
			}
			return GTHardCodedBones.EBone.None;
			IL_005F:
			throw new ArgumentOutOfRangeException(positionState.ToString());
		}

		// Token: 0x06005604 RID: 22020 RVA: 0x001A2C38 File Offset: 0x001A0E38
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticBodyDockDropPosFlags(BodyDockPositions.DropPositions enumFlags)
		{
			BodyDockPositions.DropPositions[] values = EnumData<BodyDockPositions.DropPositions>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (BodyDockPositions.DropPositions dropPositions in values)
			{
				if (dropPositions != BodyDockPositions.DropPositions.All && dropPositions != BodyDockPositions.DropPositions.None && dropPositions != BodyDockPositions.DropPositions.MaxDropPostions && (enumFlags & dropPositions) != BodyDockPositions.DropPositions.None)
				{
					list.Add(GTHardCodedBones._k_bodyDockDropPosition_to_eBone[dropPositions]);
				}
			}
			return list;
		}

		// Token: 0x06005605 RID: 22021 RVA: 0x001A2C90 File Offset: 0x001A0E90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticTransferrablePosStateFlags(TransferrableObject.PositionState enumFlags)
		{
			TransferrableObject.PositionState[] values = EnumData<TransferrableObject.PositionState>.Shared.Values;
			List<GTHardCodedBones.EBone> list = new List<GTHardCodedBones.EBone>(32);
			foreach (TransferrableObject.PositionState positionState in values)
			{
				if (positionState != TransferrableObject.PositionState.None && positionState != TransferrableObject.PositionState.Dropped && (enumFlags & positionState) != TransferrableObject.PositionState.None)
				{
					list.Add(GTHardCodedBones._k_transferrablePosState_to_eBone[positionState]);
				}
			}
			return list;
		}

		// Token: 0x06005606 RID: 22022 RVA: 0x001A2CE4 File Offset: 0x001A0EE4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetTransferrablePosStateFromBoneEnum(GTHardCodedBones.EBone eBone, out TransferrableObject.PositionState outPosState)
		{
			return GTHardCodedBones._k_eBone_to_transferrablePosState.TryGetValue(eBone, out outPosState);
		}

		// Token: 0x06005607 RID: 22023 RVA: 0x001A2CF4 File Offset: 0x001A0EF4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Transform GetBoneXformOfCosmeticPosStateFlag(TransferrableObject.PositionState anchorPosState, Transform[] bones)
		{
			if (bones.Length != 53)
			{
				throw new Exception(string.Format("{0}: Supplied bones array length is {1} but requires ", "GTHardCodedBones", bones.Length) + string.Format("{0}.", 53));
			}
			int boneIndex = GTHardCodedBones.GetBoneIndex(GTHardCodedBones.GetBoneEnumOfCosmeticPosStateFlag(anchorPosState));
			if (boneIndex != -1)
			{
				return bones[boneIndex];
			}
			return null;
		}

		// Token: 0x04005969 RID: 22889
		public const int kBoneCount = 53;

		// Token: 0x0400596A RID: 22890
		public static readonly string[] kBoneNames = new string[]
		{
			"None", "rig", "body", "head", "head_end", "shoulder.L", "upper_arm.L", "forearm.L", "hand.L", "palm.01.L",
			"palm.02.L", "thumb.01.L", "thumb.02.L", "thumb.03.L", "thumb.03.L_end", "f_index.01.L", "f_index.02.L", "f_index.03.L", "f_index.03.L_end", "f_middle.01.L",
			"f_middle.02.L", "f_middle.03.L", "f_middle.03.L_end", "shoulder.R", "upper_arm.R", "forearm.R", "hand.R", "palm.01.R", "palm.02.R", "thumb.01.R",
			"thumb.02.R", "thumb.03.R", "thumb.03.R_end", "f_index.01.R", "f_index.02.R", "f_index.03.R", "f_index.03.R_end", "f_middle.01.R", "f_middle.02.R", "f_middle.03.R",
			"f_middle.03.R_end", "body_AnchorTop_Neck", "body_AnchorFront_StowSlot", "body_AnchorFrontLeft_Badge", "body_AnchorFrontRight_NameTag", "body_AnchorBack", "body_AnchorBackLeft_StowSlot", "body_AnchorBackRight_StowSlot", "body_AnchorBottom", "body_AnchorBackBottom_Tail",
			"hand_L_AnchorBack", "hand_R_AnchorBack", "hand_L_AnchorFront_GameModeItemSlot"
		};

		// Token: 0x0400596B RID: 22891
		private const long kLeftSideMask = 1728432283058160L;

		// Token: 0x0400596C RID: 22892
		private const long kRightSideMask = 1769114204897280L;

		// Token: 0x0400596D RID: 22893
		private static readonly Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> _k_bodyDockDropPosition_to_eBone = new Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone>
		{
			{
				BodyDockPositions.DropPositions.None,
				GTHardCodedBones.EBone.None
			},
			{
				BodyDockPositions.DropPositions.LeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				BodyDockPositions.DropPositions.RightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				BodyDockPositions.DropPositions.Chest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				BodyDockPositions.DropPositions.LeftBack,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				BodyDockPositions.DropPositions.RightBack,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			}
		};

		// Token: 0x0400596E RID: 22894
		private static readonly Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> _k_transferrablePosState_to_eBone = new Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone>
		{
			{
				TransferrableObject.PositionState.None,
				GTHardCodedBones.EBone.None
			},
			{
				TransferrableObject.PositionState.OnLeftArm,
				GTHardCodedBones.EBone.forearm_L
			},
			{
				TransferrableObject.PositionState.OnRightArm,
				GTHardCodedBones.EBone.forearm_R
			},
			{
				TransferrableObject.PositionState.InLeftHand,
				GTHardCodedBones.EBone.hand_L
			},
			{
				TransferrableObject.PositionState.InRightHand,
				GTHardCodedBones.EBone.hand_R
			},
			{
				TransferrableObject.PositionState.OnChest,
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot
			},
			{
				TransferrableObject.PositionState.OnLeftShoulder,
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
			},
			{
				TransferrableObject.PositionState.OnRightShoulder,
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
			},
			{
				TransferrableObject.PositionState.Dropped,
				GTHardCodedBones.EBone.None
			}
		};

		// Token: 0x0400596F RID: 22895
		private static readonly Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> _k_eBone_to_transferrablePosState = new Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState>
		{
			{
				GTHardCodedBones.EBone.None,
				TransferrableObject.PositionState.None
			},
			{
				GTHardCodedBones.EBone.forearm_L,
				TransferrableObject.PositionState.OnLeftArm
			},
			{
				GTHardCodedBones.EBone.forearm_R,
				TransferrableObject.PositionState.OnRightArm
			},
			{
				GTHardCodedBones.EBone.hand_L,
				TransferrableObject.PositionState.InLeftHand
			},
			{
				GTHardCodedBones.EBone.hand_R,
				TransferrableObject.PositionState.InRightHand
			},
			{
				GTHardCodedBones.EBone.body_AnchorFront_StowSlot,
				TransferrableObject.PositionState.OnChest
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot,
				TransferrableObject.PositionState.OnLeftShoulder
			},
			{
				GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot,
				TransferrableObject.PositionState.OnRightShoulder
			}
		};

		// Token: 0x04005970 RID: 22896
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly List<int> _gMissingBonesReport = new List<int>(53);

		// Token: 0x04005971 RID: 22897
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_boneXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x04005972 RID: 22898
		[OnEnterPlay_Clear]
		[OnExitPlay_Clear]
		private static readonly Dictionary<int, Transform[]> _gInstIds_To_slotXforms = new Dictionary<int, Transform[]>(20);

		// Token: 0x02000D82 RID: 3458
		public enum EBone
		{
			// Token: 0x04005974 RID: 22900
			None,
			// Token: 0x04005975 RID: 22901
			rig,
			// Token: 0x04005976 RID: 22902
			body,
			// Token: 0x04005977 RID: 22903
			head,
			// Token: 0x04005978 RID: 22904
			head_end,
			// Token: 0x04005979 RID: 22905
			shoulder_L,
			// Token: 0x0400597A RID: 22906
			upper_arm_L,
			// Token: 0x0400597B RID: 22907
			forearm_L,
			// Token: 0x0400597C RID: 22908
			hand_L,
			// Token: 0x0400597D RID: 22909
			palm_01_L,
			// Token: 0x0400597E RID: 22910
			palm_02_L,
			// Token: 0x0400597F RID: 22911
			thumb_01_L,
			// Token: 0x04005980 RID: 22912
			thumb_02_L,
			// Token: 0x04005981 RID: 22913
			thumb_03_L,
			// Token: 0x04005982 RID: 22914
			thumb_03_L_end,
			// Token: 0x04005983 RID: 22915
			f_index_01_L,
			// Token: 0x04005984 RID: 22916
			f_index_02_L,
			// Token: 0x04005985 RID: 22917
			f_index_03_L,
			// Token: 0x04005986 RID: 22918
			f_index_03_L_end,
			// Token: 0x04005987 RID: 22919
			f_middle_01_L,
			// Token: 0x04005988 RID: 22920
			f_middle_02_L,
			// Token: 0x04005989 RID: 22921
			f_middle_03_L,
			// Token: 0x0400598A RID: 22922
			f_middle_03_L_end,
			// Token: 0x0400598B RID: 22923
			shoulder_R,
			// Token: 0x0400598C RID: 22924
			upper_arm_R,
			// Token: 0x0400598D RID: 22925
			forearm_R,
			// Token: 0x0400598E RID: 22926
			hand_R,
			// Token: 0x0400598F RID: 22927
			palm_01_R,
			// Token: 0x04005990 RID: 22928
			palm_02_R,
			// Token: 0x04005991 RID: 22929
			thumb_01_R,
			// Token: 0x04005992 RID: 22930
			thumb_02_R,
			// Token: 0x04005993 RID: 22931
			thumb_03_R,
			// Token: 0x04005994 RID: 22932
			thumb_03_R_end,
			// Token: 0x04005995 RID: 22933
			f_index_01_R,
			// Token: 0x04005996 RID: 22934
			f_index_02_R,
			// Token: 0x04005997 RID: 22935
			f_index_03_R,
			// Token: 0x04005998 RID: 22936
			f_index_03_R_end,
			// Token: 0x04005999 RID: 22937
			f_middle_01_R,
			// Token: 0x0400599A RID: 22938
			f_middle_02_R,
			// Token: 0x0400599B RID: 22939
			f_middle_03_R,
			// Token: 0x0400599C RID: 22940
			f_middle_03_R_end,
			// Token: 0x0400599D RID: 22941
			body_AnchorTop_Neck,
			// Token: 0x0400599E RID: 22942
			body_AnchorFront_StowSlot,
			// Token: 0x0400599F RID: 22943
			body_AnchorFrontLeft_Badge,
			// Token: 0x040059A0 RID: 22944
			body_AnchorFrontRight_NameTag,
			// Token: 0x040059A1 RID: 22945
			body_AnchorBack,
			// Token: 0x040059A2 RID: 22946
			body_AnchorBackLeft_StowSlot,
			// Token: 0x040059A3 RID: 22947
			body_AnchorBackRight_StowSlot,
			// Token: 0x040059A4 RID: 22948
			body_AnchorBottom,
			// Token: 0x040059A5 RID: 22949
			body_AnchorBackBottom_Tail,
			// Token: 0x040059A6 RID: 22950
			hand_L_AnchorBack,
			// Token: 0x040059A7 RID: 22951
			hand_R_AnchorBack,
			// Token: 0x040059A8 RID: 22952
			hand_L_AnchorFront_GameModeItemSlot
		}

		// Token: 0x02000D83 RID: 3459
		public enum EStowSlots
		{
			// Token: 0x040059AA RID: 22954
			None,
			// Token: 0x040059AB RID: 22955
			forearm_L = 7,
			// Token: 0x040059AC RID: 22956
			forearm_R = 25,
			// Token: 0x040059AD RID: 22957
			body_AnchorFront_Chest = 42,
			// Token: 0x040059AE RID: 22958
			body_AnchorBackLeft = 46,
			// Token: 0x040059AF RID: 22959
			body_AnchorBackRight
		}

		// Token: 0x02000D84 RID: 3460
		public enum EHandAndStowSlots
		{
			// Token: 0x040059B1 RID: 22961
			None,
			// Token: 0x040059B2 RID: 22962
			forearm_L = 7,
			// Token: 0x040059B3 RID: 22963
			hand_L,
			// Token: 0x040059B4 RID: 22964
			forearm_R = 25,
			// Token: 0x040059B5 RID: 22965
			hand_R,
			// Token: 0x040059B6 RID: 22966
			body_AnchorFront_Chest = 42,
			// Token: 0x040059B7 RID: 22967
			body_AnchorBackLeft = 46,
			// Token: 0x040059B8 RID: 22968
			body_AnchorBackRight
		}

		// Token: 0x02000D85 RID: 3461
		public enum ECosmeticSlots
		{
			// Token: 0x040059BA RID: 22970
			Hat = 4,
			// Token: 0x040059BB RID: 22971
			Badge = 43,
			// Token: 0x040059BC RID: 22972
			Face = 3,
			// Token: 0x040059BD RID: 22973
			ArmLeft = 6,
			// Token: 0x040059BE RID: 22974
			ArmRight = 24,
			// Token: 0x040059BF RID: 22975
			BackLeft = 46,
			// Token: 0x040059C0 RID: 22976
			BackRight,
			// Token: 0x040059C1 RID: 22977
			HandLeft = 8,
			// Token: 0x040059C2 RID: 22978
			HandRight = 26,
			// Token: 0x040059C3 RID: 22979
			Chest = 42,
			// Token: 0x040059C4 RID: 22980
			Fur = 1,
			// Token: 0x040059C5 RID: 22981
			Shirt,
			// Token: 0x040059C6 RID: 22982
			Pants = 48,
			// Token: 0x040059C7 RID: 22983
			Back = 45,
			// Token: 0x040059C8 RID: 22984
			Arms = 2,
			// Token: 0x040059C9 RID: 22985
			TagEffect = 0
		}

		// Token: 0x02000D86 RID: 3462
		[Serializable]
		public struct SturdyEBone : ISerializationCallbackReceiver
		{
			// Token: 0x170008A0 RID: 2208
			// (get) Token: 0x06005609 RID: 22025 RVA: 0x001A3050 File Offset: 0x001A1250
			// (set) Token: 0x0600560A RID: 22026 RVA: 0x001A3058 File Offset: 0x001A1258
			public GTHardCodedBones.EBone Bone
			{
				get
				{
					return this._bone;
				}
				set
				{
					this._bone = value;
					this._boneName = GTHardCodedBones.GetBoneName(this._bone);
				}
			}

			// Token: 0x0600560B RID: 22027 RVA: 0x001A3072 File Offset: 0x001A1272
			public SturdyEBone(GTHardCodedBones.EBone bone)
			{
				this._bone = bone;
				this._boneName = null;
			}

			// Token: 0x0600560C RID: 22028 RVA: 0x001A3082 File Offset: 0x001A1282
			public SturdyEBone(string boneName)
			{
				this._bone = GTHardCodedBones.GetBone(boneName);
				this._boneName = null;
			}

			// Token: 0x0600560D RID: 22029 RVA: 0x001A3097 File Offset: 0x001A1297
			public static implicit operator GTHardCodedBones.EBone(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return sturdyBone.Bone;
			}

			// Token: 0x0600560E RID: 22030 RVA: 0x001A30A0 File Offset: 0x001A12A0
			public static implicit operator GTHardCodedBones.SturdyEBone(GTHardCodedBones.EBone bone)
			{
				return new GTHardCodedBones.SturdyEBone(bone);
			}

			// Token: 0x0600560F RID: 22031 RVA: 0x001A3097 File Offset: 0x001A1297
			public static explicit operator int(GTHardCodedBones.SturdyEBone sturdyBone)
			{
				return (int)sturdyBone.Bone;
			}

			// Token: 0x06005610 RID: 22032 RVA: 0x001A30A8 File Offset: 0x001A12A8
			public override string ToString()
			{
				return this._boneName;
			}

			// Token: 0x06005611 RID: 22033 RVA: 0x000023F4 File Offset: 0x000005F4
			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
			}

			// Token: 0x06005612 RID: 22034 RVA: 0x001A30B0 File Offset: 0x001A12B0
			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				if (string.IsNullOrEmpty(this._boneName))
				{
					this._bone = GTHardCodedBones.EBone.None;
					this._boneName = "None";
					return;
				}
				GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(this._boneName);
				if (bone != GTHardCodedBones.EBone.None)
				{
					this._bone = bone;
				}
			}

			// Token: 0x040059CA RID: 22986
			[SerializeField]
			private GTHardCodedBones.EBone _bone;

			// Token: 0x040059CB RID: 22987
			[SerializeField]
			private string _boneName;
		}
	}
}
