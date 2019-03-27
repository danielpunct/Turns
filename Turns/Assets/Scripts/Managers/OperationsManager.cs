using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

public class OperationsManager : Singleton<OperationsManager>
{
    public enum PlayerAction
    {
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

    PlayerAction GetPendingAction()
    {
        var playerTile = Runner.Instance.LastTilePosition;
        var playerDirection = Runner.Instance.Direction;

        var pendingAction = PlayerAction.None;
        
        if (playerTile != null)
        {
            var tile = FloorManager.Instance.PeekTile(playerTile.Value);

            if (tile != null && tile.NextPositionKey != null)
            {
                pendingAction = FloorManager.Instance.GetNextPendingOperation(playerTile.Value, playerDirection);
            }
        }

        return pendingAction;
    }
}
