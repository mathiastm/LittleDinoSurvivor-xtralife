using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class CotcManager : MonoBehaviour
{
	
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("CotcManager Start");
        LoginAnonymous();
        // test.LoginAnonymously();
    }

    void LoginAnonymous()
    {
        var cotc = FindObjectOfType<CotcGameObject>();
        
        cotc.GetCloud().Done(cloud => {
            cloud.LoginAnonymously()
            .Done(gamer => {
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
