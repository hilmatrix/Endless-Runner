using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCurrentScore() {
        return currentScore;
    }

    public void IncreaseCurrentScore(int increment) {
        currentScore += increment;
    }

    public void FinishScoring() {
        // set high score
        if (currentScore > ScoreData.highScore) {
            ScoreData.highScore = currentScore;
        }
    }
}
