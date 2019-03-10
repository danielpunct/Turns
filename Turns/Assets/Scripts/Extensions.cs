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

        public static Vector3Int[] Directions = {Vector3Int.right, VectorInt.forward, Vector3Int.left, VectorInt.back};


    }
