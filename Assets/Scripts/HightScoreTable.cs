using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CotcSdk;

public class HightScoreTable : MonoBehaviour
{

	public Transform entriesContainer;
	private Transform entryTemplate;
	public PagedList<Score> scores;

    public void ShowHightScores(){

		entryTemplate = entriesContainer.Find("HightScoreEntryTemplate");

		entryTemplate.gameObject.SetActive(false);

		Debug.Log("############# HIGHT SCORE TEMPLATING ##############");
		Debug.Log(entryTemplate);


		float templateHeight = 30f;
		int index = 0;
		foreach(var score in scores){
			Transform entryTransform = Instantiate(entryTemplate, entriesContainer);
			RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
			entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight *index);
			entryTransform.gameObject.SetActive(true);

			int rank = index+1;
			string rankString;
			switch(rank){
				default:
					rankString = rank + "TH"; break;
				case 1: rankString = "1ST"; break;
				case 2: rankString = "2ND"; break;
				case 3: rankString = "3RD"; break;
			}

			entryTransform.Find("PositionText").GetComponent<TMP_Text>().text = rankString;
			entryTransform.Find("ScoreText").GetComponent<TMP_Text>().text = score.Value.ToString();
			entryTransform.Find("NameText").GetComponent<TMP_Text>().text = score.GamerInfo["profile"]["displayName"];
			index++;
		}
    }
}
