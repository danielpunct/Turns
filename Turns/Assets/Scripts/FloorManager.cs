using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public Transform tilesHolder;
    public float initialSpeed;
    public int initalTiles = 6;

    Stack<FloorTile> tiles;
    Vector3 nextTilePosition = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        InitialSetup();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void InitialSetup()
    {
        tiles = new Stack<FloorTile>();

        for (int i = 0; i < initalTiles; i++)
        {
            var tile = PoolManager.Instance.TilesPool.Spawn(nextTilePosition, Quaternion.identity, tilesHolder)
                .GetComponent<FloorTile>();
            tiles.Append(tile);
            
            SetNextPosition(Vector3.forward);
        }
    }

    public void SetNextPosition(Vector3 direction)
    {
        nextTilePosition += direction;
    }
}