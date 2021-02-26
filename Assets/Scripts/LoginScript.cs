using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using CotcSdk;

public class LoginScript : MonoBehaviour
{
	public UnityEngine.UI.Button loginButton;
    public UnityEngine.UI.Button logoutButton;
	public TMP_InputField userNameTextField;
	public TMP_Text errorMessage;
    public HightScoreTable hightScoreTable;
    public MainInterface playInterface;
    public LoginInterface loginInterface;

	private CotcSdk.Gamer currentGamer;

    // Start is called before the first frame update
    void Start()
    {
        // login button click
        loginButton.onClick.AddListener(Login);
        logoutButton.onClick.AddListener(NotReadyToPlay);
        
        // Resume Xtralife Session if needed
        if(!string.IsNullOrEmpty(PlayerPrefs.GetString("gamerId")) & !string.IsNullOrEmpty(PlayerPrefs.GetString("gamerSecret"))){
        	ResumeSession(PlayerPrefs.GetString("gamerId"), PlayerPrefs.GetString("gamerSecret"));
        }else{
            NotReadyToPlay();
        }

    }
    private void PostScore()
    {
        // currentGamer is an object retrieved after one of the different Login functions.

        currentGamer.Scores.Domain("private").Post(53, "global", ScoreOrder.HighToLow,
        "", false)
        .Done(postScoreRes => {
            Debug.Log("Post score: " + postScoreRes.ToString());
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    private void Login()
    {
        var cotc = FindObjectOfType<CotcGameObject>();
        cotc.GetCloud().Done(cloud => {
            cloud.LoginAnonymously()
            .Done(gamer => {
                currentGamer = gamer;
                SetPlayerPrefsData();

                Bundle profileUpdates = Bundle.CreateObject();
                profileUpdates["displayName"] = new Bundle(userNameTextField.text);

                // currentGamer is an object retrieved after one of the different Login functions.
                currentGamer.Profile.Set(profileUpdates)
                .Done(profileRes => {
                    Debug.Log("Profile data set: " + profileRes.ToString());
                    ReadyToPlay();

                }, ex => {
                    // The exception should always be CotcException
                    CotcException error = (CotcException)ex;
                    NotReadyToPlay();
                    Debug.LogError("Could not set profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
                });
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                NotReadyToPlay();
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });  
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
                ReadyToPlay();
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                NotReadyToPlay();
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });  
    }

    private void SetPlayerPrefsData(){
    	PlayerPrefs.SetString("gamerId", currentGamer.GamerId);
        PlayerPrefs.SetString("gamerSecret", currentGamer.GamerSecret);
    }
    private void ResetPlayerPrefsData(){
        PlayerPrefs.SetString("gamerId", "");
        PlayerPrefs.SetString("gamerSecret", "");
    }

    private void BestHighScores()
    {
        // currentGamer is an object retrieved after one of the different Login functions.

        currentGamer.Scores.Domain("private").BestHighScores("global", 10, 1)
        .Done(bestHighScoresRes => {
            hightScoreTable.scores = bestHighScoresRes;
            hightScoreTable.ShowHightScores();
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    private void ReadyToPlay(){
        SetPlayerPrefsData();
        BestHighScores();
        playInterface.gameObject.SetActive(true);
        hightScoreTable.gameObject.SetActive(true);
        loginInterface.gameObject.SetActive(false);
    }
    private void NotReadyToPlay(){
        ResetPlayerPrefsData();
        playInterface.gameObject.SetActive(false);
        hightScoreTable.gameObject.SetActive(false);
        loginInterface.gameObject.SetActive(true);
    }
}
