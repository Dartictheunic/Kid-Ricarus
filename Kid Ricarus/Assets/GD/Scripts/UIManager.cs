using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager uimanager;
    public GameObject[] menus;
    public GameObject baseMenu;

    bool isGamePaused;

    private void Awake()
    {
        uimanager = this;
    }

    public void ActivateMenuItem(GameObject menuToActivate)
    {
        foreach(GameObject go in menus)
        {
            if(go == menuToActivate)
            {
                if (go.activeInHierarchy)
                {
                    go.SetActive(false);
                }

                else
                {
                    go.SetActive(true);
                }
            }
        }
    }

    public void SettingsButtonPushed()
    {
        if(!isGamePaused)
        {
            isGamePaused = true;
            baseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        else
        {
            isGamePaused = false;
            baseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
