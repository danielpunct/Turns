using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

public class OperationsManager : Singleton<OperationsManager>
{
    public enum PlayerAction
    {
        Invalid,
        None,
        Left,
        Back,
        Right,
        Forward,
        Jump
    };
    
    public void DoNextAction( )
    {
        var pendingAction = GetPendingAction();
        
        switch (pendingAction)
        {
            case PlayerAction.Invalid:
                break;
            case PlayerAction.None:
                Runner.Instance.ChangeDirection( FloorManager.Instance.GetChangedRandomDirection(Runner.Instance.Direction));
                break;
            case PlayerAction.Back:
            case PlayerAction.Forward:
            case PlayerAction.Left:
            case PlayerAction.Right:
                Runner.Instance.ChangeDirection( VectorInt.fromPlayerAction(pendingAction));
                break;
            case PlayerAction.Jump:
                Runner.Instance.Jump();
                break;
        }
    }

    public PlayerAction GetPendingAction()
    {
        var pendingAction = PlayerAction.Invalid;

        var playerTilePos = Runner.Instance.TilePosition;
        var playerDirection = Runner.Instance.Direction;


        var tile = FloorManager.Instance.PeekTile(playerTilePos);

        if (tile != null && tile.NextPositionKey != null)
        {
            pendingAction = FloorManager.Instance.GetNextPendingOperation(playerTilePos, playerDirection);
        }

        return pendingAction;
    }
}
