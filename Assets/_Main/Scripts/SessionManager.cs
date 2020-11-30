using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SessionManager : MonoBehaviour
{
    public bool gameIsRunning;

    [Header("Time and Score")]
    public int maxTime;
    public int currentTime;
    public int matchScore;
    public int currentScore;


    [Header("UI")]
    public GameObject gamePlayCanvas;
    public GameObject preGameCanvas;
    public GameObject endGameCanvas;
    public GameObject sceneBackground;
    public Text timeText;
    public Text scoreText;
    public Text endScoreText;


    // Components
    private GridManager gridManager;

    #region defaultMethods

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }
  

    #endregion

    #region SessionEngine

    public void BeginGame()
    {
        gamePlayCanvas.SetActive(true);
        preGameCanvas.SetActive(false);
        sceneBackground.SetActive(true);
        
        StartCoroutine(StartSession());

        currentTime = maxTime;
        currentScore = 0;

        UpdateUI();

        InvokeRepeating("TimeTick", 1f, 1f);
    }

    private IEnumerator StartSession()
    {
        gridManager.CreateGrid();
        gridManager.GeneratePieces();
        yield return StartCoroutine(gridManager.NeighbourhoodCheck());

        StartCoroutine(SessionValidation());

        yield return null;
    }

    private IEnumerator SessionValidation()
    {
        yield return StartCoroutine(gridManager.CheckMatches());

        if (gridManager.matchedSpaces.Count > 0)
        {
            yield return StartCoroutine(SessionReformulation());
        }
        else
        {
            gameIsRunning = true;
        }

        yield return StartCoroutine(gridManager.CheckSoftLock());

        if (gridManager.softLocked)
        {
            Debug.Log("Softlocked!");
        }


    }

    private IEnumerator SessionReformulation()
    {
      
        yield return StartCoroutine(gridManager.ClearPrematureMatches());

        yield return new WaitForEndOfFrame();

        gridManager.EraseMatches();
        StartCoroutine(SessionValidation());
        
    }


    private void GameFinished()
    {
        CancelInvoke("TimeTick");
        gameIsRunning = false;
        endGameCanvas.SetActive(true);
        gamePlayCanvas.SetActive(false);
        sceneBackground.SetActive(false);
        gridManager.gameObject.SetActive(false);
        endScoreText.text = currentScore.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region TimeAndScore

    private void TimeTick()
    {
        currentTime--;
        timeText.text = currentTime.ToString();

        UpdateUI();

        if (currentTime <= 0)
            GameFinished();

    }

    public void AddScore(int count)
    {
        if(gameIsRunning)
        currentScore += matchScore * count;
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        timeText.text = currentTime.ToString();
        scoreText.text = currentScore.ToString();
    }

    #endregion


}
