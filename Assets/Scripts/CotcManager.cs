using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;
using RSG;

public class CotcManager : MonoBehaviour
{

    private CotcSdk.Gamer currentGamer;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public IPromise<string> Login(string userName)
    {
        var promise = new RSG.Promise<string>();
        promise.Resolve("ok");
        var cotc = FindObjectOfType<CotcGameObject>();
        cotc.GetCloud().Done(cloud => {
            cloud.LoginAnonymously()
            .Done(gamer => {
                currentGamer = gamer;
                SetPlayerPrefsData();

                Bundle profileUpdates = Bundle.CreateObject();
                profileUpdates["displayName"] = new Bundle(userName);

                // currentGamer is an object retrieved after one of the different Login functions.
                currentGamer.Profile.Set(profileUpdates)
                .Done(profileRes => {
                    Debug.Log("Profile data set: " + profileRes.ToString());
                    promise.Resolve("ok");

                }, ex => {
                    // The exception should always be CotcException
                    CotcException error = (CotcException)ex;
                    promise.Reject(new System.Exception("Reject"));
                    Debug.LogError("Could not set profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
                });
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                promise.Reject(new System.Exception("Reject"));
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });

        return promise; // Return the promise so the caller can await resolution (or error).
    }
    public IPromise<string> ResumeSession(string gamerId, string gamerSecret)
    {
        var promise = new RSG.Promise<string>();
        var cotc = FindObjectOfType<CotcGameObject>();

        cotc.GetCloud().Done(cloud => {
            cloud.ResumeSession(
                gamerId: gamerId,
                gamerSecret: gamerSecret)
            .Done(gamer => {
                currentGamer = gamer;
                SetPlayerPrefsData();
                promise.Resolve("OK");
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                ResetPlayerPrefsData();
                promise.Reject(new System.Exception("Reject"));
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });  

        return promise; // Return the promise so the caller can await resolution (or error).
    }

    public IPromise<string> BestHighScores()
    {
        // currentGamer is an object retrieved after one of the different Login functions.
        var promise = new RSG.Promise<string>();
        currentGamer.Scores.Domain("private").BestHighScores("global", 10, 1)
        .Done(bestHighScoresRes => {
            var hightScoreTable = FindObjectOfType<HightScoreTable>();
            hightScoreTable.scores = bestHighScoresRes;
            hightScoreTable.ShowHightScores();
            promise.Resolve("OK");
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            promise.Reject(new System.Exception("Reject"));
        });
        return promise;
    }

    public IPromise<string> PostScore(int score)
    {
        // currentGamer is an object retrieved after one of the different Login functions.
        var promise = new RSG.Promise<string>();
        currentGamer.Scores.Domain("private").Post(score, "global", ScoreOrder.HighToLow,
        "", false)
        .Done(postScoreRes => {
            Debug.Log("Post score: " + postScoreRes.ToString());
            promise.Resolve("OK");
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            promise.Reject(new System.Exception("Reject"));
        });
        return promise;
    }


    private void SetPlayerPrefsData(){
        PlayerPrefs.SetString("gamerId", currentGamer.GamerId);
        PlayerPrefs.SetString("gamerSecret", currentGamer.GamerSecret);
    }
    private void ResetPlayerPrefsData(){
        PlayerPrefs.SetString("gamerId", "");
        PlayerPrefs.SetString("gamerSecret", "");
    }
}
