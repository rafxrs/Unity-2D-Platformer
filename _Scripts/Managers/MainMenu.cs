using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool isMuted = false;
    // public Text volumeText;

    void Start()
    {
        // volumeText.text = PlayerPrefs.GetString("Sound", "SOUND ON");
        // int muted = PlayerPrefs.GetInt("Muted", 0);
        // if (muted==0) {
        //     isMuted=false;
        // } else {
        //     isMuted = true;
        // }

    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape)) 
        // {
        //     Application.Quit();
        //     // SceneManager.LoadScene(0  );
        // }
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level); 
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0); 
    }
    // public void LoadSettings()
    // {
    //     SceneManager.LoadScene(); 
    // }
    // public void LoadCredits()
    // {
    //     SceneManager.LoadScene();
    // }
    public void MuteAllSound()
    {
        AudioListener.volume = 0;
        
    }
    public void UnMuteAllSound()
    {
        AudioListener.volume = 1;
        
    }
    public void ChangeVolume()
    {
        // if (isMuted)
        // {
        //     volumeText.text = "SOUND ON";
        //     UnMuteAllSound();
        //     PlayerPrefs.SetString("Sound", "SOUND ON"); 
        //     PlayerPrefs.SetInt("Muted", 0);
        //     isMuted = false;
            
        // } else {
        //     volumeText.text = "SOUND OFF";
        //     MuteAllSound();
        //     PlayerPrefs.SetString("Sound", "SOUND OFF"); 
        //     PlayerPrefs.SetInt("Muted", 1);
        //     isMuted = true;
        // }
    }
}
