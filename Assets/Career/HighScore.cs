using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Career
{
    public class HighScore
    {
        #region Singleton :(
        private HighScore()
        {
        }

        private static HighScore s_Instance = new HighScore();

        public static HighScore Instance
        {
            get
            {
                return HighScore.s_Instance;
            }
        }
        #endregion

        private static readonly string Key_TopKills = "TopKills";

        public int TopKills
        {
            get
            {
                return PlayerPrefs.GetInt(HighScore.Key_TopKills, 0);
            }
            set
            {
                var current = this.TopKills;
                if (current < value)
                {
                    PlayerPrefs.SetInt(HighScore.Key_TopKills, value);
                }
            }
        }

        private static readonly string Key_TopScore = "TopScore";

        public int TopScore
        {
            get
            {
                return PlayerPrefs.GetInt(HighScore.Key_TopScore, 0);
            }
            set
            {
                var current = this.TopScore;
                if (current < value)
                {
                    PlayerPrefs.SetInt(HighScore.Key_TopScore, value);
                }
            }
        }
    }
}
