using TestGame.Equipment;
using TestGame.Player;
using UnityEngine;

namespace TestGame.Triggers
{
    /// <summary>
    /// Item spawn area.
    /// </summary>
    /// <remarks>
    /// Spawns one of items from tray and allows to collect it by player.
    /// </remarks>
    [RequireComponent(typeof(BoxCollider))]
    public class ItemSpawnArea : MonoBehaviour
    {
        //
        // A spawn point socket object.
        //
        public GameObject SpawnPoint;

        //
        // "Force field" prefab object. Just for fun and indication to user.
        //
        public GameObject ForcefieldPrefab;

        //
        // Item respawn interval.
        //
        public float RespawnInterval;

        //
        // Socket rotation speed (deg/s)
        //
        public float RotationAngleSpeed = 30.0F;

        //
        // Tray with objects to spawn.
        //
        public GameObject[] Tray;
        
        //
        // An instantiated force field.
        //
        private GameObject m_ForceField;

        //
        // A spawned object.
        //
        private GameObject m_SpawnedObject;

        //
        // Current timeout value.
        //
        private float m_Timeout;

        //
        // An index of currently selected item
        //
        private int m_CurrentItemIndex = -1;

        private void Start()
        {
            //
            // Reset timeout.
            //
            this.m_Timeout = 0.0F;

            //
            // Instantiate forcefield.
            //
            this.m_ForceField = GameObject.Instantiate(this.ForcefieldPrefab, this.SpawnPoint.transform, false);
        }

        private void Update()
        {
            //
            // Item spawn area must have anything to spawn.
            //
            if (this.Tray.Length > 0)
            {
                //
                // Update timeout.
                //
                this.m_Timeout -= Time.deltaTime;

                if (this.m_Timeout <= 0.0F)
                {
                    //
                    // Reset timeout timer.
                    //
                    this.m_Timeout = this.RespawnInterval;

                    //
                    // Destroy current object.
                    //
                    this.DestroyStoredItem();

                    //
                    // And create new item.
                    //
                    this.CreateNewItem();
                }

                //
                // Perform any effect
                //
                this.DoRotationEffect();
            }
        }

        private void DoRotationEffect()
        {
            //
            // Rotates effect.
            //
            var scaledAngle = RotationAngleSpeed * Time.deltaTime;
            this.SpawnPoint.transform.Rotate(Vector3.up, scaledAngle);
        }

        private int GetDifferentRandomIndex()
        {
            while (this.Tray.Length > 1)
            {
                //
                // Generate random indices.
                //
                var index = UnityEngine.Random.Range(0, this.Tray.Length);

                if (index != this.m_CurrentItemIndex)
                {
                    //
                    // This index is different than previous one.
                    //
                    return index;
                }
            }

            //
            // Spawn point has only one element in tray.
            //
            return 0;
        }

        private void CreateNewItem()
        {
            //
            // Choose new random object from tray.
            //
            this.m_CurrentItemIndex = GetDifferentRandomIndex();
            var prefab = this.Tray[this.m_CurrentItemIndex];

            //
            // Instantiate that prefab at spawn point.
            //
            this.m_SpawnedObject = GameObject.Instantiate(
                prefab,
                this.SpawnPoint.transform,
                false
                );

            //
            // And show forcefield.
            //
            this.m_ForceField.SetActive(true);
        }

        private void DestroyStoredItem()
        {
            if (this.m_SpawnedObject != null)
            {
                //
                // Destroy spawned object.
                //
                GameObject.Destroy(this.m_SpawnedObject);

                //
                // And hide force field.
                //
                this.m_ForceField.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null && this.m_SpawnedObject != null)
            {
                //
                // Check if we have equipment onboard.
                //
                var equipment = this.m_SpawnedObject.GetComponent<BaseEquipment>();
                Debug.Assert(equipment != null);

                //
                // Apply it and release slot.
                //
                equipment.Apply(other.gameObject);
                this.DestroyStoredItem();
            }
        }
    }
}
