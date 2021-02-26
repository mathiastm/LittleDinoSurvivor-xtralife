using UnityEngine.UI;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public Text Score;
    private void Start()
    {
        Score.text = SaveLoadManager.Instance.Score.ToString();
    }

}
