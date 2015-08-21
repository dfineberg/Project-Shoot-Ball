using UnityEngine;
using System.Collections;

public class TileMapNavGenerator : MonoBehaviour
{
    public float extraOffset;

    void Start()
    {
        //Turns each tile with a collider attached to it into an obstacle on the nav mesh
        foreach (TileEditor.Tile tile in FindObjectsOfType<TileEditor.Tile>())
        {
            if (tile.GetComponent<Collider2D>() != null)
            {
                PolyNavObstacle ob = tile.gameObject.AddComponent<PolyNavObstacle>();
                ob.extraOffset = extraOffset;
            }
        }
    }
}
