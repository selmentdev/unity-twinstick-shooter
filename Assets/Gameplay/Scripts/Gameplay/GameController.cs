using System;
using System.Collections;
using System.Collections.Generic;
using TestGame.Player;
using UnityEngine;

namespace TestGame.Gameplay
{
    /// <summary>
    /// Controls game.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        #region Component Properties
        public PlayerController Player;

        #endregion

        #region `Gametone`
        private static GameController s_Instance = null;
        public static GameController Instance
        {
            get
            {
                return GameController.s_Instance;
            }
        }

        private void Awake()
        {
            //
            // This is not singleton per se. Just handy way of accessing it.
            //
            Debug.Assert(GameController.s_Instance == null);
            GameController.s_Instance = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(GameController.s_Instance == this);
            GameController.s_Instance = null;
        } 
        #endregion

        public GameObject DeadMessagePanel;
        
        private void Update()
        {
            if (Input.GetButtonDown("Start"))
            {
                Time.timeScale = 0.0F;

                Cursor.visible = true;

                this.DeadMessagePanel.SetActive(true);
            }
        }

        public void PlayerPassedAway()
        {
            //
            // Pause gameplay...
            //
            Time.timeScale = 0.0F;

            Cursor.visible = true;

            this.DeadMessagePanel.SetActive(true);
        }
    }

}