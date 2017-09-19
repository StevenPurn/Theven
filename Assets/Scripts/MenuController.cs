using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    GameBoard gBoard;
    GameBoard.GameType gameType;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Use this for initialization
    void Start () {
        gBoard = FindObjectOfType<GameBoard>();
        gameType = GameBoard.GameType.standard;
	}

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        gBoard.CreateBoard(gameType);
    }
	
    public void SetFirstPlayer(Dropdown dropDown)
    {
        switch (dropDown.value)
        {
            case 0:
                GameBoard.CurPlayer = Player.white;
                break;
            case 1:
                GameBoard.CurPlayer = Player.black;
                break;
            default:
                GameBoard.CurPlayer = Player.white;
                break;
        }
    }

    public void SetGameSetup(Dropdown dropDown)
    {
        switch (dropDown.value)
        {
            case 0:
                gameType = GameBoard.GameType.standard;
                break;
            case 1:
                gameType = GameBoard.GameType.centered;
                break;
            case 2:
                gameType = GameBoard.GameType.corners;
                break;
            default:
                gameType = GameBoard.GameType.standard;
                break;
        }
    }
}
