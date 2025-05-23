using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8A RID: 3210
	public class StoreController : MonoBehaviour
	{
		// Token: 0x06004F99 RID: 20377 RVA: 0x0017B745 File Offset: 0x00179945
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
				return;
			}
			if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x06004F9A RID: 20378 RVA: 0x0017B77C File Offset: 0x0017997C
		public void CreateDynamicCosmeticStandsDictionatary()
		{
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									if (!this.CosmeticStandsDict.ContainsKey(string.Concat(new string[] { storeDepartment.departmentName, "|", storeDisplay.displayName, "|", dynamicCosmeticStand.StandName })))
									{
										this.CosmeticStandsDict.Add(string.Concat(new string[] { storeDepartment.departmentName, "|", storeDisplay.displayName, "|", dynamicCosmeticStand.StandName }), dynamicCosmeticStand);
									}
									else
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											storeDepartment.departmentName,
											"|",
											storeDisplay.displayName,
											"|",
											dynamicCosmeticStand.StandName,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06004F9B RID: 20379 RVA: 0x0017B958 File Offset: 0x00179B58
		private void Create_StandsByPlayfabIDDictionary()
		{
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (!dynamicCosmeticStand.StandName.IsNullOrEmpty() && !dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
					{
						this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					}
					else
					{
						this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand> { dynamicCosmeticStand });
					}
				}
			}
		}

		// Token: 0x06004F9C RID: 20380 RVA: 0x000023F4 File Offset: 0x000005F4
		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		// Token: 0x06004F9D RID: 20381 RVA: 0x000023F4 File Offset: 0x000005F4
		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x000023F4 File Offset: 0x000005F4
		public void ImportCosmeticStandLayout()
		{
		}

		// Token: 0x06004F9F RID: 20383 RVA: 0x0017BA18 File Offset: 0x00179C18
		public void InitalizeCosmeticStands()
		{
			this.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.InitializeCosmetic();
			}
			this.Create_StandsByPlayfabIDDictionary();
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x0017BA7C File Offset: 0x00179C7C
		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		// Token: 0x06004FA1 RID: 20385 RVA: 0x0017BAB4 File Offset: 0x00179CB4
		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		// Token: 0x06004FA2 RID: 20386 RVA: 0x0017BB38 File Offset: 0x00179D38
		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindObjectOfType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		// Token: 0x06004FA3 RID: 20387 RVA: 0x0017BB68 File Offset: 0x00179D68
		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		// Token: 0x06004FA4 RID: 20388 RVA: 0x0017BBD0 File Offset: 0x00179DD0
		public void FindAllDepartments()
		{
			this.Departments = Object.FindObjectsOfType<StoreDepartment>().ToList<StoreDepartment>();
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x0017BBE4 File Offset: 0x00179DE4
		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x0017BD04 File Offset: 0x00179F04
		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindObjectOfType<StoreController>();
			}
			StoreController.instance.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		// Token: 0x040052AF RID: 21167
		public static volatile StoreController instance;

		// Token: 0x040052B0 RID: 21168
		public List<StoreDepartment> Departments;

		// Token: 0x040052B1 RID: 21169
		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		// Token: 0x040052B2 RID: 21170
		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		// Token: 0x040052B3 RID: 21171
		public AllCosmeticsArraySO AllCosmeticsArraySO;

		// Token: 0x040052B4 RID: 21172
		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";
	}
}
