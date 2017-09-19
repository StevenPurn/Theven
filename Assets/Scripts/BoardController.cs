using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    public GameBoard currentBoard;

    public void PositionChildren()
    {
        foreach (Transform child in GetComponent<Transform>())
        {
            if (child.GetComponent<TileController>() != null)
            {
                Vector3 loc = new Vector3(child.GetComponent<TileController>().tileLoc.x, 0, child.GetComponent<TileController>().tileLoc.y);
                child.position = loc;
            }
        }
    }
}
