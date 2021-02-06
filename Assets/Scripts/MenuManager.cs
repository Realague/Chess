using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{

    public TMP_Text endText;

    public GameObject checkText;

    public GameObject endMenu;

    public GameObject chooseMenu;

    public void DisplayEndMenu() {
        if (GameMaster.instance.playerColor == Color.None) {
            endText.text = GameMaster.instance.turn == Color.Light ? "Dark won !" : "Light won!";
        } else {
            endText.text = GameMaster.instance.turn == GameMaster.instance.playerColor ? "You lost !" : "You won !";
        }
        endMenu.SetActive(true);
    }

    public void PlayAgain() {
        SceneManager.LoadScene("chess");
    }

    public void DisplayCheck(bool state) {
        checkText.SetActive(state);
    }

    public void ChooseSide(int color) {
        GameMaster.instance.playerColor = (Color)color;
        GameMaster.instance.start = true;
        chooseMenu.SetActive(false);
    }
}
