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
            Collider2D col = tile.GetComponent<Collider2D>();

            if (col != null)
            {
                if (col.gameObject.layer != 10)
                {
                    PolyNavObstacle ob = tile.gameObject.AddComponent<PolyNavObstacle>();
                    ob.extraOffset = extraOffset;
                }
            }
        }
    }
}
