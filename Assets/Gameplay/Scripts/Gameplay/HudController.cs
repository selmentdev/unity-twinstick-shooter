using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Core;
using TestGame.Player;
using TestGame.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame.Gameplay
{
    /// <summary>
    /// Provides way of updating gameplay HUD.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        #region Component Properties
        [Header("Player")]
        public PlayerCharacter Player;
        public PlayerController PlayerController;

        [Header("GameController")]
        public GameController GameController;

        [Header("Health Panel")]
        public Image HealthBar;
        public Text HealthValue;

        [Header("Armor Panel")]
        public Image ArmorBar;
        public Text ArmorValue;

        [Header("Current Weapon Panel")]
        public Image WeaponImage;
        public Text WeaponClipAmmo;
        public Text WeaponTotalAmmo;

        [Header("Wave Panel")]
        public Text WaveCurrent;
        public Text WaveBotsCount;

        [Header("Wave Countdown")]
        public GameObject WaveCountdownBackground;
        public Text WaveCountdownText;

        [Header("Score Panel")]
        public Text ScoreValue;
        public Text ScoreTotalDown;

        #endregion

        #region Gametone
        private static HudController s_Instance = null;
        public static HudController Instance
        {
            get
            {
                return HudController.s_Instance;
            }
        }

        private void Awake()
        {
            Debug.Assert(HudController.s_Instance == null);
            HudController.s_Instance = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(HudController.s_Instance == this);
            HudController.s_Instance = null;
        }
        #endregion

        private bool m_UpdateHealth = true;
        private bool m_UpdateWave = true;

        public void UpdateHealth(CharacterBase character)
        {
            //
            // Update health progress bar and values.
            //
            {
                var progress = character.Health / character.HealthMax;
                var fillAmount = Mathf.Clamp01(progress);

                this.HealthBar.fillAmount = fillAmount;

                //
                // Update health value.
                //

                this.HealthValue.text = String.Format("{0}/{1}", (int)character.Health, (int)character.HealthMax);
            }

            {
                var progress = character.Armor / character.ArmorMax;
                var fillAmount = Mathf.Clamp01(progress);

                this.ArmorBar.fillAmount = fillAmount;

                //
                // Update health value.
                //

                this.ArmorValue.text = String.Format("{0}/{1}", (int)character.Armor, (int)character.ArmorMax);
            }
        }

        public void UpdateWave(int currentWave, int enemiesLeft, int enemiesTotal)
        {
            var waveController = WaveController.Instance;

            this.WaveCurrent.text = String.Format("Wave {0}", waveController.Wave);
            this.WaveBotsCount.text = String.Format("{0}/{1}", waveController.EnemiesAlive, waveController.Enemies);
            this.ScoreValue.text = String.Format("Score: {0}", waveController.Score);
            this.ScoreTotalDown.text = String.Format("Total: {0}", waveController.TotalEnemiesDown);
        }

        public void UpdateWeapon(Weapon weapon)
        {
            if (weapon.IsReloading)
            {
                this.WeaponClipAmmo.text = "Reloading...";
            }
            else
            {
                this.WeaponClipAmmo.text = String.Format("{0}/{1}", weapon.CurrentClipAmmo, weapon.MaxClipAmmo);
            }

            if (weapon.InfiniteAmmo)
            {
                this.WeaponTotalAmmo.text = "\u221e";
            }
            else
            {
                this.WeaponTotalAmmo.text = String.Format("{0}/{1}", weapon.CurrentBagAmmo, weapon.MaxBagAmmo);
            }

            this.WeaponImage.sprite = weapon.AvatarImage;
        }

        public void UpdateWaveCountdown(bool show, bool waveNumber, int value)
        {
            this.WaveCountdownBackground.SetActive(show);
            if (show)
            {
                this.WaveCountdownText.text = String.Format(
                    waveNumber
                    ? "Wave {0}"
                    : "{0}", value);
            }
        }

        public void NotifyUpdateHealth()
        {
            this.m_UpdateHealth = true;
        }

        public void NotifyUpdateWave()
        {
            this.m_UpdateWave = true;
        }

        private void LateUpdate()
        {
            if (this.m_UpdateHealth)
            {
                this.UpdateHealth(this.Player);
            }

            this.UpdateWeapon(this.PlayerController.CurrentWeapon);

            if (this.m_UpdateWave)
            {
                this.m_UpdateWave = false;

                var waveController = WaveController.Instance;

                this.UpdateWave(
                    waveController.Wave,
                    waveController.EnemiesAlive,
                    waveController.Enemies);
            }
        }
    }
}
