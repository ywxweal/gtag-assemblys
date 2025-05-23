using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005A1 RID: 1441
[Serializable]
public class GRAbilityStagger
{
	// Token: 0x0600232B RID: 9003 RVA: 0x000AFBB4 File Offset: 0x000ADDB4
	public void Setup(Vector3 staggerVel, GameAgent agent, Animation anim, Transform root, Rigidbody rb)
	{
		this.agent = agent;
		this.staggerVel = staggerVel;
		this.anim = anim;
		this.root = root;
		this.rb = rb;
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000AFBDC File Offset: 0x000ADDDC
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.endTime = Time.timeAsDouble + (double)this.duration;
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000AFC2C File Offset: 0x000ADE2C
	public void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000AFC47 File Offset: 0x000ADE47
	public bool IsDone()
	{
		return Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000AFC5C File Offset: 0x000ADE5C
	public void Update(float dt)
	{
		Vector3 position = this.root.position;
		Vector3 vector = position + this.staggerVel * dt;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
		{
			vector = navMeshHit.position;
		}
		if (NavMesh.Raycast(position, vector, out navMeshHit, -1))
		{
			vector = navMeshHit.position;
		}
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000AFCD8 File Offset: 0x000ADED8
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002761 RID: 10081
	public float duration;

	// Token: 0x04002762 RID: 10082
	public string animName;

	// Token: 0x04002763 RID: 10083
	public float animSpeed;

	// Token: 0x04002764 RID: 10084
	private GameAgent agent;

	// Token: 0x04002765 RID: 10085
	private Animation anim;

	// Token: 0x04002766 RID: 10086
	private Transform root;

	// Token: 0x04002767 RID: 10087
	private Rigidbody rb;

	// Token: 0x04002768 RID: 10088
	[ReadOnly]
	public Vector3 staggerVel;

	// Token: 0x04002769 RID: 10089
	[ReadOnly]
	public double endTime;
}
