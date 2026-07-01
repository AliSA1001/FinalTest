using UnityEngine;

public class OptionController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionPanel;

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionPanel.SetActive(false);
        mainMenu.SetActive(true);
    }
}