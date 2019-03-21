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
                Player.Instance.ChangeDirection( FloorManager.Instance.GetChangedRandomDirection(Player.Instance.Direction));
                break;
            case PlayerAction.Back:
            case PlayerAction.Forward:
            case PlayerAction.Left:
            case PlayerAction.Right:
                Player.Instance.ChangeDirection( VectorInt.fromPlayerAction(pendingAction));
                break;
            case PlayerAction.Jump:
                Player.Instance.Jump();
                break;
        }
    }

    PlayerAction GetPendingAction()
    {
        var playerTile = Player.Instance.CurrentTilePosition;
        var playerDirection = Player.Instance.Direction;

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
