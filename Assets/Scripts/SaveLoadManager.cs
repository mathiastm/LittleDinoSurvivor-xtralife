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

    private void ResumeSession(string gamerId = null, string gamerSecret = null)
    {
        var cotc = FindObjectOfType<CotcGameObject>();

        cotc.GetCloud().Done(cloud => {
            cloud.ResumeSession(
                gamerId: gamerId,
                gamerSecret: gamerSecret)
            .Done(gamer => {
                currentGamer = gamer;
                SetPlayerPrefsData();
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });  
    }

    private void PostScore(int score)
    {
        // currentGamer is an object retrieved after one of the different Login functions.

        currentGamer.Scores.Domain("private").Post(score, "global", ScoreOrder.HighToLow,
        "", false)
        .Done(postScoreRes => {
            Debug.Log("Post score: " + postScoreRes.ToString());
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    private void SetPlayerPrefsData(){
        PlayerPrefs.SetString("gamerId", currentGamer.GamerId);
        PlayerPrefs.SetString("gamerSecret", currentGamer.GamerSecret);
        Debug.Log("########## gamer.Profile.DisplayName ############");
        Debug.Log(currentGamer["profile"]["displayName"]);
        PlayerPrefs.SetString("gamerDisplayName", currentGamer["profile"]["displayName"]);
    }

}
