using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class BuilderActions
{
	// Token: 0x06001E0B RID: 7691 RVA: 0x000920D8 File Offset: 0x000902D8
	public static BuilderAction CreateAttachToPlayer(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, int actorNumber, bool leftHand)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber,
			localPosition = localPosition,
			localRotation = localRotation,
			isLeftHand = leftHand
		};
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x00092128 File Offset: 0x00090328
	public static BuilderAction CreateAttachToPlayerRollback(int cmdId, BuilderPiece piece)
	{
		return BuilderActions.CreateAttachToPlayer(cmdId, piece.pieceId, piece.transform.localPosition, piece.transform.localRotation, piece.heldByPlayerActorNumber, piece.heldInLeftHand);
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x00092158 File Offset: 0x00090358
	public static BuilderAction CreateDetachFromPlayer(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPlayer,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x00092190 File Offset: 0x00090390
	public static BuilderAction CreateAttachToPiece(int cmdId, int pieceId, int parentPieceId, int attachIndex, int parentAttachIndex, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int actorNumber, int timeStamp)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			parentPieceId = parentPieceId,
			attachIndex = attachIndex,
			parentAttachIndex = parentAttachIndex,
			bumpOffsetx = bumpOffsetX,
			bumpOffsetz = bumpOffsetZ,
			twist = twist,
			playerActorNumber = actorNumber,
			timeStamp = timeStamp
		};
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x00092204 File Offset: 0x00090404
	public static BuilderAction CreateAttachToPieceRollback(int cmdId, BuilderPiece piece, int actorNumber)
	{
		byte pieceTwist = piece.GetPieceTwist();
		sbyte b;
		sbyte b2;
		piece.GetPieceBumpOffset(pieceTwist, out b, out b2);
		return BuilderActions.CreateAttachToPiece(cmdId, piece.pieceId, piece.parentPiece.pieceId, piece.attachIndex, piece.parentAttachIndex, b, b2, pieceTwist, actorNumber, piece.activatedTimeStamp);
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x00092250 File Offset: 0x00090450
	public static BuilderAction CreateDetachFromPiece(int cmdId, int pieceId, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DetachFromPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x00092288 File Offset: 0x00090488
	public static BuilderAction CreateMakeRoot(int cmdId, int pieceId)
	{
		return new BuilderAction
		{
			type = BuilderActionType.MakePieceRoot,
			localCommandId = cmdId,
			pieceId = pieceId
		};
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000922B8 File Offset: 0x000904B8
	public static BuilderAction CreateDropPiece(int cmdId, int pieceId, Vector3 localPosition, Quaternion localRotation, Vector3 velocity, Vector3 angVelocity, int actorNumber)
	{
		return new BuilderAction
		{
			type = BuilderActionType.DropPiece,
			localCommandId = cmdId,
			pieceId = pieceId,
			localPosition = localPosition,
			localRotation = localRotation,
			velocity = velocity,
			angVelocity = angVelocity,
			playerActorNumber = actorNumber
		};
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x00092314 File Offset: 0x00090514
	public static BuilderAction CreateDropPieceRollback(int cmdId, BuilderPiece rootPiece, int actorNumber)
	{
		Vector3 vector = rootPiece.transform.position;
		Quaternion quaternion = rootPiece.transform.rotation;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		Rigidbody component = rootPiece.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.position;
			quaternion = component.rotation;
			vector2 = component.velocity;
			vector3 = component.angularVelocity;
		}
		return BuilderActions.CreateDropPiece(cmdId, rootPiece.pieceId, vector, quaternion, vector2, vector3, actorNumber);
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x00092388 File Offset: 0x00090588
	public static BuilderAction CreateAttachToShelfRollback(int cmdId, BuilderPiece piece, int shelfID, bool isConveyor, int timestamp = 0, float splineTime = 0f)
	{
		return new BuilderAction
		{
			type = BuilderActionType.AttachToShelf,
			localCommandId = cmdId,
			pieceId = piece.pieceId,
			attachIndex = shelfID,
			parentAttachIndex = timestamp,
			isLeftHand = isConveyor,
			velocity = new Vector3(splineTime, 0f, 0f),
			localPosition = piece.transform.localPosition,
			localRotation = piece.transform.localRotation
		};
	}
}
