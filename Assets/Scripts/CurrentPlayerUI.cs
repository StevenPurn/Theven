using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerUI : MonoBehaviour {

    public string curPlayerText;

    public void SetUIText()
    {
        curPlayerText = GameBoard.CurPlayer.ToString();
        GetComponent<Text>().text = curPlayerText;
    }
}
