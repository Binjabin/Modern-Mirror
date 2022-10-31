using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static List<Color> scoredColors = new List<Color>();
    public GameObject painting;
    public GameObject basketball;

    public TMP_Text timerText;
    public float basketballTime;
    float basketballTimeRemaining;
    bool inBasketball;


    public void ToPainting()
    {
        inBasketball = false;
        painting.SetActive(true);
        basketball.SetActive(false);
        FindObjectOfType<CanSpawner>().SpawnCans();
    }

    public void Start()
    {
        inBasketball = true;
        painting.SetActive(false);
        basketball.SetActive(true);
        basketballTimeRemaining = basketballTime;
    }

    private void Update()
    {
        if(inBasketball)
        {
            basketballTimeRemaining -= Time.deltaTime;
            float secs = Mathf.Round(basketballTimeRemaining % 60);
            float mins = Mathf.Floor(basketballTimeRemaining / 60);
            timerText.text = mins + ":" + secs;
            if (basketballTimeRemaining < 0f)
            {
                ToPainting();

            }
        }
        
    }

}
