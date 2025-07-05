using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void skelotonScene()
    {
        print("Skeleton");
        SceneManager.LoadScene("SampleScene");
    }

    public void layersScene()
    {
        print("Layers");
        SceneManager.LoadScene("LayerScene");
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public GameObject settingPanel;

    public void openSetting()
    {
        settingPanel.SetActive(true);
    }

    public void backFromSetting()
    {
        settingPanel.SetActive(false);
    }
}
