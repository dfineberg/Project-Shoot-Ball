using UnityEngine;
using System.Collections;

public class TileMapNavGenerator : MonoBehaviour {

    public float extraOffset;

	// Use this for initialization
	void Start () {
	
        foreach(TileEditor.Tile tile in FindObjectsOfType<TileEditor.Tile>())
        {
            if(tile.GetComponent<Collider2D>() != null)
            {
                PolyNavObstacle ob = tile.gameObject.AddComponent<PolyNavObstacle>();
                ob.extraOffset = extraOffset;
            }
        }

	}
}
