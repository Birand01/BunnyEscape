using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class GameUI : MonoBehaviour
{
    
    [SerializeField] GameObject gameLoseUI;
    [SerializeField] GameObject gameWinUI;
    [SerializeField] Button retryButton;
    [SerializeField] public Image scoreProgressImage;
    bool gameIsOver;
    #region Singleton Class: GameUI
    public static GameUI Instance;
    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
        }
    }
    #endregion
    void Start()
    {
        scoreProgressImage.fillAmount = 0f;
        Gardener.OnGardenerSpottedBunny +=ShowGameOverUI;
        FindObjectOfType<LivingEntity>().OnReachedEndOfLevel += ShowGameWinUI;
    }
    void Update()
    {
       
    }
    public void RetryButton()
    {
        if(gameIsOver)
        {
            SceneManager.LoadScene(0);
        }
    }
    void ShowGameOverUI()
    {
        OnGameOver(gameLoseUI);
    }
    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    public void UpdateScore()
    {
        float val = LittleBunny.Instance.carrotScore / 7f;
        scoreProgressImage.DOFillAmount(val, 0.4f);
    }



    void OnGameOver(GameObject gameUI)
    {
            gameUI.SetActive(true);    
            gameIsOver = true;
            Gardener.OnGardenerSpottedBunny -= ShowGameOverUI;
        FindObjectOfType<LivingEntity>().OnReachedEndOfLevel -= ShowGameWinUI;

    }
}
