using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/* TO USE
 * paste FindObjectOfType<AudioManager>().Play("audio name");
 * where you want sound to play. change "audio name" to name 
 * of audio in the Audio Manager */

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        bool themePaused;

        Play("Theme");
        themePaused = false;

        Scene currentScene = SceneManager.GetActiveScene();

        string sceneName = currentScene.name;

        if (sceneName != "MainMenu" && sceneName != "Lobby")
        {
            Play("Theme");
            themePaused = true;
        }
        else
        {
            if(themePaused)
            {
                Play("Theme");
                themePaused = false;
            }
        }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Uhhh did you mean to say " + name + "???? I'm not seeing that in my inspector. Also make sure you name the title theme 'Theme'");
            return;
        }

        if (name == "Theme")
        {
            Scene currentScene = SceneManager.GetActiveScene();

            string sceneName = currentScene.name;

            if (sceneName != "MainMenu" && sceneName != "Lobby")
            {
                Debug.Log("Paused theme");
                return;
            }  
        }
        s.source.Play();
    }
}
