using UnityEngine;

    public static class Extensions
    {
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
        
        public static Vector3Int GetChangedRandomDirection(this Vector3Int dir)
        {
            if (dir == Vector3.forward || dir == Vector3.back)
            {
                return Random.Range(0, 2) > 0 ? Vector3Int.left : Vector3Int.right;
            }
            else
            {
                return Random.Range(0, 2) > 0 ? VectorInt.forward : VectorInt.back;
            }
        }
    }
