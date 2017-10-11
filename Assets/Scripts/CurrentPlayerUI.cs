using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerUI : MonoBehaviour {

    public string curPlayerText;

    private void Start()
    {
        SetUIText();
    }

    public void SetUIText()
    {
        curPlayerText = GameBoard.CurPlayer.ToString();
        GetComponent<Text>().text = "Current player: " + char.ToUpper(curPlayerText[0]) + curPlayerText.Substring(1);
    }
}
