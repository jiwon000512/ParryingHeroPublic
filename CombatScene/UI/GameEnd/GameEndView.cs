using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameEndView : MonoBehaviour
{
    public GameObject gameEndPopup;
    public GameObject returnButton;
    public GameObject showADReturnButton;

    public GameObject stageClearPopup;
    public GameObject clearButton;
    public GameObject showADClearButton;

    public void GameOver()
    {
        StartCoroutine(ShowGameOverPopup());
    }

    public void StageClear()
    {
        StartCoroutine(ShowStageClearPopup());
    }

    IEnumerator ShowGameOverPopup()
    {
        gameEndPopup.SetActive(false);
        gameEndPopup.SetActive(true);
        gameEndPopup.GetComponent<Image>().DOFade(1, 3f);

        yield return new WaitForSeconds(3f);

        returnButton.gameObject.SetActive(true);
        showADReturnButton.gameObject.SetActive(true);
        returnButton.GetComponent<Image>().DOFade(1, 1f);
        showADReturnButton.GetComponent<Image>().DOFade(1, 1f);
    }

    IEnumerator ShowStageClearPopup()
    {
        stageClearPopup.SetActive(false);
        stageClearPopup.SetActive(true);
        stageClearPopup.GetComponent<Image>().DOFade(0.4f, 3f);

        yield return new WaitForSeconds(3f);

        clearButton.SetActive(true);
        showADClearButton.SetActive(true);
        clearButton.GetComponent<Image>().DOFade(1, 1f);
        showADClearButton.GetComponent<Image>().DOFade(1, 1f);


    }

    public void ReturnToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
