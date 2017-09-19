using UnityEngine;

public class TileController : MonoBehaviour {

    public Material blackMat, whiteMat;
    public TileLocation tileLoc;
    public TileColor tileColor;
    public GameObject possibleMoveEffect;

    public void SetMaterial(TileColor tc)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material = tc == TileColor.black ? blackMat : whiteMat;
    }

    public void SetTileLocation(TileLocation tileLocation)
    {
        tileLoc = tileLocation;
    }

    public void SetAsPossibleMove()
    {
        possibleMoveEffect.SetActive(true);
    }

    public void RemoveAsPossibleMove()
    {
        possibleMoveEffect.SetActive(false);
    }
}
