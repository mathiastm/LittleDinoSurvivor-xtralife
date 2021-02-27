using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using CotcSdk;
using RSG;

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

        // logout button click
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
        var cotcManager = FindObjectOfType<CotcManager>();
        cotcManager.Login(userNameTextField.text)
		    .Then(result => {
		        Debug.Log(result);
		        ReadyToPlay();
		    })
		    .Catch(exception => {
		        Debug.LogError("An exception occured while CotcManager Login");
		        NotReadyToPlay();
		    });

    }

    private void ResumeSession(string gamerId = null, string gamerSecret = null)
    {
    	var cotcManager = FindObjectOfType<CotcManager>();
        cotcManager.ResumeSession(PlayerPrefs.GetString("gamerId"), PlayerPrefs.GetString("gamerSecret"))
		    .Then(result => {
		        Debug.Log(result);
		        ReadyToPlay();
		    })
		    .Catch(exception => {
		        Debug.LogError("An exception occured while CotcManager ResumeSession");
		    	NotReadyToPlay();
		    });
    }

    private void BestHighScores()
    {
        var cotcManager = FindObjectOfType<CotcManager>();
        cotcManager.BestHighScores();
    }
    private void ReadyToPlay(){
        BestHighScores();
        playInterface.gameObject.SetActive(true);
        hightScoreTable.gameObject.SetActive(true);
        loginInterface.gameObject.SetActive(false);
    }
    private void NotReadyToPlay(){
        playInterface.gameObject.SetActive(false);
        hightScoreTable.gameObject.SetActive(false);
        loginInterface.gameObject.SetActive(true);
    }
}
