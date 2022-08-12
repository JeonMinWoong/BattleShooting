using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    public GameObject introPanel;
    public TextMeshProUGUI introText;
    public GameObject[] introOj;
    public int clickCount;

    public void TrainingStart()
    {
        if (AuthManager.instance.userTraining == false)
        {
            introPanel.SetActive(true);
            Click();
        }
    }

    public void Click()
    {
        if (clickCount == 0)
            introText.text = "Hello!";
        else if (clickCount == 1)
            introText.text = "Welcome to the battle shooting!";
        else if (clickCount == 2)
            introText.text = "Let me briefly introduce you to this game.";
        else if (clickCount == 3)
        {
            introText.text = "This part represents the player's level and experience, and it goes up every time you play the game, so please work hard.";
            introOj[0].transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
        }
        else if (clickCount == 4)
        {
            introOj[0].transform.SetAsFirstSibling();
            introOj[1].transform.SetAsLastSibling();
            introOj[2].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is a button to view user information.\nYou can press it.";
        }
        else if (clickCount == 5)
        {
            introOj[1].transform.SetAsFirstSibling();
            introOj[2].transform.SetAsFirstSibling();
            introOj[3].transform.SetAsLastSibling();
            introOj[4].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is a button to select a skill. \nYou can choose a skill that is useful for you in the lobby.";
        }
        else if (clickCount == 6)
        {
            introOj[3].transform.SetAsFirstSibling();
            introOj[4].transform.SetAsFirstSibling();
            introOj[5].transform.SetAsLastSibling();
            introOj[6].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is a characteristic button. \nYou can level up and choose your own unique abilities to become stronger. \nBut keep in mind that you can only take one thing.";
        }
        else if (clickCount == 7)
        {
            introOj[5].transform.SetAsFirstSibling();
            introOj[6].transform.SetAsFirstSibling();
            introOj[7].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is the store button.\nIt's still unimplemented, so we'll see you soon.";
        }
        else if (clickCount == 8)
        {
            introOj[7].transform.SetAsFirstSibling();
            introOj[8].transform.SetAsLastSibling();
            introOj[9].transform.SetAsLastSibling();
            introOj[10].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is your nickname, money and medals you have. \nMoney and medals are earned according to your ranking each time you play. \nIt is useful, \nso please put it together.";
        }
        else if (clickCount == 9)
        {
            introOj[8].transform.SetAsFirstSibling();
            introOj[9].transform.SetAsFirstSibling();
            introOj[10].transform.SetAsFirstSibling();
            introOj[11].transform.SetAsLastSibling();
            introOj[12].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "This is the setting button. \nYou can set up the game.";
        }
        else if (clickCount == 10)
        {
            introOj[11].transform.SetAsFirstSibling();
            introOj[12].transform.SetAsFirstSibling();
            introOj[13].transform.SetAsLastSibling();
            introOj[14].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "The game start button. With this, you can enjoy the game with other users. \nHowever, always be aware of the server status.";
        }
        else if (clickCount == 11)
        {
            introOj[13].transform.SetAsFirstSibling();
            introOj[14].transform.SetAsFirstSibling();
            introOj[15].transform.SetAsLastSibling();
            introText.transform.SetAsLastSibling();
            introOj[16].transform.SetAsLastSibling();
            introText.text = "Finally, shall we train? \nPlease press it.";
        }
        clickCount++;
    }

    public void Skip()
    {
        introPanel.SetActive(false);
        AuthManager.instance.userTraining = true;
        AuthManager.instance.OnClickUpdateChildren();

    }
}
