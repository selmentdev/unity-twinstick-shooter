using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TestGame.Menu
{
    public class MainMenuHandler : MonoBehaviour
    {
        public GameObject LoadingPanel;
        public Image LoadingProgress;

        public Text HighScoreContent;

        private void Start()
        {
            this.HighScoreContent.text = String.Format(
                "Top Score: {0}\nTop Kills: {1}",
                Career.HighScore.Instance.TopScore,
                Career.HighScore.Instance.TopKills
                );
        }

        public void PlayGame()
        {
            LoadingPanel.SetActive(true);

            StartCoroutine(LoadGameplay());
        }

        public IEnumerator LoadGameplay()
        {
            var result = SceneManager.LoadSceneAsync("Gameplay/Scenes/World",  LoadSceneMode.Single);

            while (!result.isDone)
            {
                this.LoadingProgress.fillAmount = result.progress;
                yield return null;
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}