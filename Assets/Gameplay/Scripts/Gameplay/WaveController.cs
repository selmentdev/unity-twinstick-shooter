using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Bots;
using TestGame.Core;
using UnityEngine;

namespace TestGame.Gameplay
{
    /// <summary>
    /// Controls waves of enemies.
    /// </summary>
    public class WaveController : MonoBehaviour
    {
        public delegate void NotifyEnemyDiedDelegate(CharacterBase character);

        public event NotifyEnemyDiedDelegate EnemyDied;

        #region Component Propertiess
        public WaveSpawnArea[] SpawnAreas;
        public GameObject[] BotPrefabs;
        public Transform Bots;

        public int Wave;
        public int Enemies;
        public int EnemiesAlive;

        public int Score;
        public int TotalEnemiesDown;
        #endregion

        public WaveController()
        {
            this.EnemyDied += WaveController_EnemyDied;
        }

        private void WaveController_EnemyDied(CharacterBase character)
        {
            //
            // Update stats.
            //
            --this.EnemiesAlive;
            ++this.TotalEnemiesDown;
            this.Score += (int)character.HealthMax;

            //
            // Notify HUD to pull that info back.
            //
            HudController.Instance.NotifyUpdateWave();

            //
            // Check if we should start next wave.
            //
            if (this.EnemiesAlive == 0)
            {
                ++this.Wave;
                this.StartWave();
            }
        }

        public void RaiseEnemyDied(CharacterBase character)
        {
            var handler = this.EnemyDied;
            if (handler != null)
            {
                handler(character);
            }
        }

        #region Gametone
        private static WaveController s_Instance = null;
        public static WaveController Instance
        {
            get
            {
                return WaveController.s_Instance;
            }
        }

        private void Awake()
        {
            Debug.Assert(WaveController.s_Instance == null);
            WaveController.s_Instance = this;
        }

        private void OnDestroy()
        {
            Debug.Assert(WaveController.s_Instance == this);
            WaveController.s_Instance = null;
        }
        #endregion

        private GameObject GetRandomBot()
        {
            Debug.Assert(this.BotPrefabs.Length > 0);

            var randomIndex = UnityEngine.Random.Range(0, this.BotPrefabs.Length);
            var randomPrefab = this.BotPrefabs[randomIndex];

            return randomPrefab;
        }

        private void Start()
        {
            this.StartWave();
        }

        private void StartWave()
        {
            this.EnemiesAlive = this.Enemies = NumberOfEnemies(this.Wave);
            StartCoroutine(SpawnEnemies());
        }

        public static int NumberOfEnemies(int wave)
        {
            //
            // Nice approximation function.
            //
            // Wave 50 =~ 450 enemies.
            //
            var a = (16.0F * wave * wave) / 117.0F;
            var b = (214.0F * wave) / 117.0F;
            var c = 940.0F / 117.0F;
            return (int)(a + b + c);
        }

        private IEnumerator SpawnEnemies()
        {
            //
            // Show wave number.
            //
            HudController.Instance.UpdateWaveCountdown(true, waveNumber: true, value: this.Wave);
            yield return new WaitForSeconds(1.0F);

            //
            // Perform countdown
            //
            for (var i = 5; i >= 1; --i)
            {
                HudController.Instance.UpdateWaveCountdown(true, waveNumber: false, value: i);
                yield return new WaitForSeconds(1.0F);
            }

            //
            // Hide message panel.
            //
            HudController.Instance.UpdateWaveCountdown(false, false, 0);

            //
            // Get player position.
            //
            var playerPosition = GameController.Instance.Player.transform.position;

            //
            // Get areas that are away from player.
            //
            var availableAreas = this.SpawnAreas.Where(x =>
            {
                var distanceToPlayer = Vector3.Distance(x.transform.position, playerPosition);
                return distanceToPlayer >= (x.Radius * 2.5F);
            }).ToArray();

            //
            // Create enemies at spawn points.
            //
            for (var i = 0; i < this.Enemies; ++i)
            {
                var randomBot = this.GetRandomBot();
                var randomArea = availableAreas[UnityEngine.Random.Range(0, availableAreas.Length)];

                //
                // Spawn new enemy.
                //
                randomArea.SpawnEnemy(randomBot);

                yield return new WaitForSeconds(0.05F);
            }
        }
    }
}
