using UnityEngine;

    public static class Extensions
    {
        public static OperationsManager.PlayerAction ToAction(this Vector3Int v)
        {
            if (v == VectorInt.back)
            {
                return OperationsManager.PlayerAction.Back;
            }
            if (v == VectorInt.forward)
            {
                return OperationsManager.PlayerAction.Forward;
            }
            if (v == Vector3Int.left)
            {
                return OperationsManager.PlayerAction.Left;
            }
            if (v == Vector3Int.right)
            {
                return OperationsManager.PlayerAction.Right;
            }

            return OperationsManager.PlayerAction.None;
        }

        public static Vector3 ToVector3(this Vector3Int v)
        {
            return  new Vector3(v.x, v.y, v.z);
        }
    }

    public static class VectorInt
    {
        public static Vector3Int forward
        {
            get
            {
                return new Vector3Int(0,0,1);
            }
        }
        
        public static Vector3Int back
        {
            get
            {
                return new Vector3Int(0,0,-1);
            }
        }

        public static Vector3Int HorizontalDif(this Vector3Int a, Vector3Int b)
        {
            return  new Vector3Int(a.x - b.x, 0, a.z - b.z);
        }

        public static Vector3Int fromPlayerAction(OperationsManager.PlayerAction action)
        {
            switch (action)
            {
                case OperationsManager.PlayerAction.Left:
                    return Vector3Int.left;
                case OperationsManager.PlayerAction.Right:
                    return Vector3Int.right;
                case OperationsManager.PlayerAction.Back:
                    return back;
                default:
                case OperationsManager.PlayerAction.Forward:
                    return forward;
            }
        }

        public static Vector3Int[] Directions = {Vector3Int.right, VectorInt.forward, Vector3Int.left, VectorInt.back};


    }
