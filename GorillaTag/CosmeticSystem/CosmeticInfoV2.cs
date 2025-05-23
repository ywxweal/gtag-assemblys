using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7A RID: 3450
	[Serializable]
	public struct CosmeticInfoV2 : ISerializationCallbackReceiver
	{
		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x060055E2 RID: 21986 RVA: 0x001A2064 File Offset: 0x001A0264
		public bool hasHoldableParts
		{
			get
			{
				CosmeticPart[] array = this.holdableParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060055E3 RID: 21987 RVA: 0x001A2084 File Offset: 0x001A0284
		public bool hasWardrobeParts
		{
			get
			{
				CosmeticPart[] array = this.wardrobeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060055E4 RID: 21988 RVA: 0x001A20A4 File Offset: 0x001A02A4
		public bool hasStoreParts
		{
			get
			{
				CosmeticPart[] array = this.storeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060055E5 RID: 21989 RVA: 0x001A20C4 File Offset: 0x001A02C4
		public bool hasFunctionalParts
		{
			get
			{
				CosmeticPart[] array = this.functionalParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060055E6 RID: 21990 RVA: 0x001A20E4 File Offset: 0x001A02E4
		public bool hasFirstPersonViewParts
		{
			get
			{
				CosmeticPart[] array = this.firstPersonViewParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x060055E7 RID: 21991 RVA: 0x001A2104 File Offset: 0x001A0304
		public CosmeticInfoV2(string displayName)
		{
			this.enabled = true;
			this.season = null;
			this.displayName = displayName;
			this.playFabID = "";
			this.category = CosmeticsController.CosmeticCategory.None;
			this.icon = null;
			this.isHoldable = false;
			this.isThrowable = false;
			this.usesBothHandSlots = false;
			this.hideWardrobeMannequin = false;
			this.holdableParts = new CosmeticPart[0];
			this.functionalParts = new CosmeticPart[0];
			this.wardrobeParts = new CosmeticPart[0];
			this.storeParts = new CosmeticPart[0];
			this.firstPersonViewParts = new CosmeticPart[0];
			this.setCosmetics = new CosmeticSO[0];
			this.anchorAntiIntersectOffsets = default(CosmeticAnchorAntiIntersectOffsets);
			this.debugCosmeticSOName = "__UNINITIALIZED__";
		}

		// Token: 0x060055E8 RID: 21992 RVA: 0x000023F4 File Offset: 0x000005F4
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x060055E9 RID: 21993 RVA: 0x001A21C0 File Offset: 0x001A03C0
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
			this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
			this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
			this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
			this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
			if (this.setCosmetics == null)
			{
				this.setCosmetics = Array.Empty<CosmeticSO>();
			}
		}

		// Token: 0x060055EA RID: 21994 RVA: 0x001A2224 File Offset: 0x001A0424
		private void _OnAfterDeserialize_InitializePartsArray(ref CosmeticPart[] parts, ECosmeticPartType partType)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i].partType = partType;
				ref CosmeticAttachInfo[] ptr = ref parts[i].attachAnchors;
				if (ptr == null)
				{
					ptr = Array.Empty<CosmeticAttachInfo>();
				}
			}
		}

		// Token: 0x0400593D RID: 22845
		public bool enabled;

		// Token: 0x0400593E RID: 22846
		[Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
		[Delayed]
		public SeasonSO season;

		// Token: 0x0400593F RID: 22847
		[Tooltip("Name that is displayed in the store during purchasing.")]
		[Delayed]
		public string displayName;

		// Token: 0x04005940 RID: 22848
		[Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
		[Delayed]
		public string playFabID;

		// Token: 0x04005941 RID: 22849
		public Sprite icon;

		// Token: 0x04005942 RID: 22850
		[Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
		public StringEnum<CosmeticsController.CosmeticCategory> category;

		// Token: 0x04005943 RID: 22851
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
		[HideInInspector]
		public bool isHoldable;

		// Token: 0x04005944 RID: 22852
		public bool isThrowable;

		// Token: 0x04005945 RID: 22853
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented.")]
		[HideInInspector]
		public bool usesBothHandSlots;

		// Token: 0x04005946 RID: 22854
		public bool hideWardrobeMannequin;

		// Token: 0x04005947 RID: 22855
		public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";

		// Token: 0x04005948 RID: 22856
		public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components which are parented to slots in \"Gorilla Player Networked.prefab\". Which slots can be used by the \n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x04005949 RID: 22857
		[Space]
		[Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components which are parented to slots in \"Gorilla Player Networked.prefab\". Which slots can be used by the \n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] holdableParts;

		// Token: 0x0400594A RID: 22858
		public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";

		// Token: 0x0400594B RID: 22859
		public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nAny parts attached to the head (like hats) are hidden from local camera view. To make it visible from first person: create a first person version of the prefab and assign it to the \"First Person\" array below.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x0400594C RID: 22860
		[Space]
		[Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nAny parts attached to the head (like hats) are hidden from local camera view. To make it visible from first person: create a first person version of the prefab and assign it to the \"First Person\" array below.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] functionalParts;

		// Token: 0x0400594D RID: 22861
		public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x0400594E RID: 22862
		public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x0400594F RID: 22863
		[Space]
		[Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] wardrobeParts;

		// Token: 0x04005950 RID: 22864
		[Space]
		[Tooltip("TODO")]
		public CosmeticPart[] storeParts;

		// Token: 0x04005951 RID: 22865
		public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x04005952 RID: 22866
		public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)";

		// Token: 0x04005953 RID: 22867
		[Space]
		[Tooltip("\"First Person View Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\n(Last Updated 2024-08-07 MattO)")]
		public CosmeticPart[] firstPersonViewParts;

		// Token: 0x04005954 RID: 22868
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets;

		// Token: 0x04005955 RID: 22869
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticSO[] setCosmetics;

		// Token: 0x04005956 RID: 22870
		[NonSerialized]
		public string debugCosmeticSOName;
	}
}
