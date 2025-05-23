using System;
using UnityEngine;

// Token: 0x020003A0 RID: 928
internal interface ITetheredObjectBehavior
{
	// Token: 0x060015B3 RID: 5555
	void DbgClear();

	// Token: 0x060015B4 RID: 5556
	void EnableDistanceConstraints(bool v, float playerScale);

	// Token: 0x060015B5 RID: 5557
	void EnableDynamics(bool enable, bool collider, bool kinematic);

	// Token: 0x060015B6 RID: 5558
	bool IsEnabled();

	// Token: 0x060015B7 RID: 5559
	void ReParent();

	// Token: 0x060015B8 RID: 5560
	bool ReturnStep();

	// Token: 0x060015B9 RID: 5561
	void TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership);
}
