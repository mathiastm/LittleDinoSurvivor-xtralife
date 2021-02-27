using System;
using UnityEngine;
using CotcSdk;

public class SaveLoadManager : MonoBehaviour
{

    #region Singleton

    public static SaveLoadManager Instance;
    private CotcSdk.Gamer currentGamer;
   
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of SaveLoadManager found!");
            return;
        }

        Instance = this;
       
    }

    #endregion

    public bool IsAlive;
    private AudioSource _audioSource;
    public int Score;
    public int BestScore;
    private Interface _interface;
    public bool NewBest;

    // Use this for initialization
    void Start()
    {
        NewBest = false;
        IsAlive = true;
        Score = 0;
        _interface = GameObject.Find("Interface").GetComponent<Interface>();
        _audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetString("Music").Equals("False"))
            {
                _audioSource.enabled = false;
            }
        }
        BestScore = PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetInt("BestScore") : 0;
        ResumeSession(PlayerPrefs.GetString("gamerId"), PlayerPrefs.GetString("gamerSecret"));
    }



    public void IncreaseScore()
    {
        if (IsAlive)
        {
            Score++;
            _interface.ScoreText.text = Convert.ToString(++_interface.ScoreValue);
        }

    }
    private void ResumeSession(string gamerId = null, string gamerSecret = null)
    {
        var cotcManager = FindObjectOfType<CotcManager>();
        cotcManager.ResumeSession(PlayerPrefs.GetString("gamerId"), PlayerPrefs.GetString("gamerSecret"));
    }

    public void SaveScore()
    {
        if (Score > BestScore)
        {
            NewBest = true;
            BestScore = Score;
            PlayerPrefs.SetInt("BestScore", Score);
        }
        PostScore(Score);
    }

    private void PostScore(int score)
    {
        var cotcManager = FindObjectOfType<CotcManager>();
        cotcManager.PostScore(score);
    }


}
