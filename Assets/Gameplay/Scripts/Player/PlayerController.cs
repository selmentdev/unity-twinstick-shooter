using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Bots;
using TestGame.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace TestGame.Player
{
    /// <summary>
    /// Player controller. Supports both mouse+keyboard and gamepad. To use gamepad just use right stick.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        //
        // Pad axis dead zone check.
        //
        public const float DeadZone = 0.2F;

        //
        // Navmesh agent.
        //
        private NavMeshAgent m_Agent;

        //
        // Body of player. Base game object rotates separately. Body rotates toward pointer.
        //
        public GameObject Body;

        //
        // Laser pointer. It really should be part of gun...
        //
        public GameObject LaserPointer;

        //
        // A player character that uses this controller.
        //
        public PlayerCharacter Character;

        //
        // Currently holded weapon.
        //
        public Weapon CurrentWeapon;

        //
        // Index of holded weapon - just to know which one is next/prev.
        //
        private int m_CurrentWeaponIndex = 0;

        //
        // Player weapon slots.
        //
        public Weapon[] WeaponSlots;

        //
        // Socket to where attach weapon on player pawn.
        //
        public GameObject WeaponSocket;

        //
        // How fast player rotate around. Degrees per second.
        //
        public float RotationAngularSpeed = 360.0F;

        //
        // Camera rotation. 
        //
        public float CameraAngularSpeed = 30.0F;

        //
        // Move speed.
        //
        public float MoveSpeed = 4.0F;

        //
        // Sprint speed.
        //
        public float SprintSpeed = 7.0F;

        //
        // Current speed.
        //
        private float m_CurrentSpeed = 0.0F;

        //
        // Look angle.
        //
        private float m_LookAngle = 0.0F;
        
        private bool m_IsRunning = false;

        //
        // Current control type.
        //
        private ControlType m_ControlType = ControlType.Keyboard;

        //
        // Current Rose Axis Index:
        //
        //    7   0   1
        //     \  |  /
        //      \ | /
        //       \|/
        //    6---X---2
        //       /|\
        //      / | \
        //     /  |  \
        //    5   4   3
        //
        private int m_RoseAxisIndex = 1;

        //
        // Sets new control type.
        //
        private void SetControlType(ControlType controlType)
        {
            this.m_ControlType = controlType;

            //
            // Hides cursor when user uses gamepad.
            //
            Cursor.visible = (controlType == ControlType.Keyboard);
        }
        
        private void Start()
        {
            //
            // Acquires navmesh agent.
            //
            this.m_Agent = this.GetComponent<NavMeshAgent>();
            this.m_Agent.updateRotation = false;

            //
            // Acquires character.
            //
            this.Character = this.GetComponent<PlayerCharacter>();

            //
            // Computes initial isometric camera rotation.
            //
            this.transform.rotation = Quaternion.Euler(0.0F, m_RoseAxisIndex * 45.0F, 0.0F);

            //
            // Default to mouse+keyboard steering.
            //
            this.SetControlType(ControlType.Keyboard);
            
            //
            // XXX: Instantiate all prefabs in weapon slots to actual objects.
            //
            for (var i = 0; i < this.WeaponSlots.Length; ++i)
            {
                this.WeaponSlots[i] = (Weapon)Instantiate(this.WeaponSlots[i]);
                this.WeaponSlots[i].gameObject.SetActive(false);
                this.WeaponSlots[i].transform.parent = this.transform;
                this.WeaponSlots[i].transform.localPosition = Vector3.zero;
            }

            //
            // And select first weapon.
            //
            this.SelectWeapon(0);
        }

        /// <summary>
        /// Control type.
        /// </summary>
        public enum ControlType
        {
            Keyboard,
            Gamepad
        }

        private void Update()
        {
            //
            // Acquire delta time to local variable.
            //
            var deltaTime = Time.deltaTime;

            //
            // Check if player is running.
            //
            this.m_IsRunning = Input.GetButton("Sprint");

            RotateCamera(deltaTime);

            //
            // Acquire move direction.
            //
            var rawMove = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );

            //
            // Initial angles for move and look.
            //
            var moveDirectionAngle = 0.0F;
            var lookDirectionAngle = 0.0F;

            moveDirectionAngle = MoveCharacter(deltaTime, rawMove, moveDirectionAngle);

            //
            // Acquire pad look direction.
            //
            var rawPadDirection = new Vector2(
                Input.GetAxis("Look Gamepad X"),
                Input.GetAxis("Look Gamepad Y")
            );

            //
            // And mouse movement.
            //
            var rawMouseDirection = new Vector2(
                Input.GetAxis("Look Mouse X"),
                Input.GetAxis("Look Mouse Y")
            );

            //
            // Check if we have input.
            //
            var padMoved = DetectController(ref lookDirectionAngle, rawPadDirection, rawMouseDirection);


            //
            // Mouse requires special handling, because actor will stare at mouse regardles of its movement.
            //
            lookDirectionAngle = FixupKeyboardDirectionAngle(lookDirectionAngle);

            //
            //  Angle = 
            //
            //              | mouse     | gamepad
            //  ------------|-----------|-----------
            //   run        | moveAngle | moveAngle
            //  ------------|-----------|-----------
            //   walk-look  | lookAngle | lookAngle
            //  ------------|-----------|-----------
            //   walk-only  | lookAngle | moveAngle
            //
            //
            FixupRotationAngle(moveDirectionAngle, lookDirectionAngle, padMoved);

            RotateCharacter(deltaTime, moveDirectionAngle, lookDirectionAngle);

            HandleCommonActions();

            //
            // Handle weapon change.
            //
            HandleWeaponChange();
        }

        private void FixupRotationAngle(float moveDirectionAngle, float lookDirectionAngle, bool padMoved)
        {
            if (this.m_IsRunning || (m_ControlType == ControlType.Gamepad && !padMoved))
            {
                this.m_LookAngle = moveDirectionAngle;
            }
            else
            {
                this.m_LookAngle = lookDirectionAngle;
            }
        }

        private void RotateCharacter(float deltaTime, float moveDirectionAngle, float lookDirectionAngle)
        {
            if (moveDirectionAngle != 0.0F || lookDirectionAngle != 0.0F)
            {
                //
                // Add-up stick space rotation to controller orientation.
                //
                var rotation = Quaternion.Euler(0.0F, m_LookAngle, 0.0F) * this.transform.rotation;
                this.Body.transform.rotation = Quaternion.RotateTowards(this.Body.transform.rotation, rotation, this.RotationAngularSpeed * deltaTime);
            }
        }

        private void HandleWeaponChange()
        {
            var weaponChange = Input.GetAxis("Change Weapon");
            var isSwitched = (weaponChange < -DeadZone || weaponChange > DeadZone);

            if (isSwitched & !this.m_GamepadPreviousWeaponChange)
            {
                //
                // Latch for D-Pad.
                //
                this.m_GamepadPreviousWeaponChange = true;

                if (weaponChange < 0.0F)
                {
                    this.SelectPrevWeapon();
                }
                else
                {
                    this.SelectNextWeapon();
                }
            }
            else if (!isSwitched)
            {
                this.m_GamepadPreviousWeaponChange = false;
            }
        }

        private void HandleCommonActions()
        {
            if (Input.GetButtonDown("Fire") || Input.GetAxis("Fire") > DeadZone * 2.0F)
            {
                //
                // User pressed fire.
                //
                this.Shoot();
            }

            if (Input.GetButtonDown("Prev Weapon"))
            {
                //
                // User switching to previous weapon.
                //
                this.SelectPrevWeapon();
            }

            if (Input.GetButtonDown("Next Weapon"))
            {
                //
                // User switching to next weapon.
                //
                this.SelectNextWeapon();
            }

            if (Input.GetButtonDown("Reload"))
            {
                //
                // Reload clip clicked.
                //
                this.Reload();
            }
        }

        private float MoveCharacter(float deltaTime, Vector2 rawMove, float moveDirectionAngle)
        {
            if (rawMove.sqrMagnitude >= (DeadZone * DeadZone))
            {
                //
                // Move input over dead zone. Compute move direction.
                //
                var direction = new Vector3(rawMove.x, 0.0F, rawMove.y);
                direction.Normalize();

                //
                // Adjust speed.
                //
                this.m_CurrentSpeed = this.m_IsRunning ? this.SprintSpeed : this.MoveSpeed;

                //
                // Compute relative move vector.
                //
                direction *= this.m_CurrentSpeed * deltaTime;

                //
                // And move agent.
                //
                this.m_Agent.Move(this.transform.rotation * direction);

                //
                // Compute angle in "stick" space.
                //
                moveDirectionAngle = Mathf.Rad2Deg * Mathf.Atan2(-rawMove.y, rawMove.x);
            }

            return moveDirectionAngle;
        }

        private void RotateCamera(float deltaTime)
        {
            //
            // Check camera rotation input.
            //
            if (Input.GetButtonDown("Prev Camera"))
            {
                m_RoseAxisIndex = m_RoseAxisIndex + 7;
                m_RoseAxisIndex = m_RoseAxisIndex % 8;
            }
            else if (Input.GetButtonDown("Next Camera"))
            {
                m_RoseAxisIndex = m_RoseAxisIndex + 9;
                m_RoseAxisIndex = m_RoseAxisIndex % 8;
            }

            //
            // Rotate actor foward vector toward one of 8 camera angles.
            //
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(0.0F, m_RoseAxisIndex * 45.0F, 0.0F), this.CameraAngularSpeed * deltaTime);
        }

        private float FixupKeyboardDirectionAngle(float lookDirectionAngle)
        {

            if (m_ControlType == ControlType.Keyboard)
            {
                //
                // Create plane at player position.
                //
                var plane = new Plane(Vector3.up, this.LaserPointer.transform.position);

                //
                // Create ray pointing from screen.
                //
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                var enter = 0.0F;
                if (plane.Raycast(ray, out enter))
                {
                    //
                    // Compute intersection point.
                    //
                    var intersection = ray.origin + enter * ray.direction;

                    //
                    // Compute arm vector between player and cursor.
                    //
                    var direction = Quaternion.Inverse(this.transform.rotation) * (this.transform.position - intersection);

                    //
                    // Make sure that it's outside player deadzone (inaccurate spinning).
                    //
                    if (direction.sqrMagnitude >= 1.0F)
                    {
                        lookDirectionAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.z, -direction.x);
                    }
                }
            }

            return lookDirectionAngle;
        }

        private bool DetectController(ref float lookDirectionAngle, Vector2 rawPadDirection, Vector2 rawMouseDirection)
        {
            var padMoved = rawPadDirection.sqrMagnitude >= (DeadZone * DeadZone);
            var mouseMoved = rawMouseDirection.sqrMagnitude >= (DeadZone * DeadZone);

            if (padMoved)
            {
                //
                // Compute look angle and continue as gamepad.
                //
                lookDirectionAngle = Mathf.Rad2Deg * Mathf.Atan2(rawPadDirection.y, rawPadDirection.x);
                this.SetControlType(ControlType.Gamepad);
            }
            else if (mouseMoved)
            {
                //
                // Continue as keyboard.
                //
                this.SetControlType(ControlType.Keyboard);
            }

            return padMoved;
        }

        private void Reload()
        {
            if (this.CurrentWeapon != null)
            {
                //
                // Just reload weapon.
                //
                this.CurrentWeapon.Reload();
            }
        }

        private bool m_GamepadPreviousWeaponChange = false;

        //
        // I'm not proud of it - it can be done better.
        //
        public void TakeWeapon(Weapon weapon)
        {
            //
            // Iterate over all sockets.
            //
            for (int i = 0; i < this.WeaponSlots.Length; ++i)
            {
                //
                // Check if we have this kind of weapon.
                //
                var current = this.WeaponSlots[i];
                if (current.WeaponType == weapon.WeaponType)
                {
                    //
                    // If so, grab ammo from it - our is better.
                    //
                    current.GrabAmmo(weapon);
                }
            }
        }

        public void SelectNextWeapon()
        {
            //
            // Select next weapon index.
            //
            this.m_CurrentWeaponIndex += 1;
            this.m_CurrentWeaponIndex %= this.WeaponSlots.Length;

            //
            // And switch to it.
            //
            this.SelectWeapon(this.m_CurrentWeaponIndex);
        }

        public void SelectPrevWeapon()
        {
            //
            // Select next weapon index.
            //
            this.m_CurrentWeaponIndex += this.WeaponSlots.Length;
            this.m_CurrentWeaponIndex -= 1;
            this.m_CurrentWeaponIndex %= this.WeaponSlots.Length;

            //
            // And switch to it.
            //
            this.SelectWeapon(this.m_CurrentWeaponIndex);
        }

        private void SelectWeapon(int index)
        {
            if (0 <= index && index < this.WeaponSlots.Length)
            {
                this.m_CurrentWeaponIndex = index;

                //
                // Unpin currently used weapon.
                //
                if (this.CurrentWeapon != null)
                {
                    this.CurrentWeapon.gameObject.SetActive(false);
                    this.CurrentWeapon.transform.parent = null;
                }

                //
                // And pin selected weapon to socket.
                //
                var selected = this.WeaponSlots[this.m_CurrentWeaponIndex];
                selected.gameObject.SetActive(true);
                selected.transform.parent = this.WeaponSocket.transform;
                selected.transform.localPosition = Vector3.zero;
                selected.transform.localRotation = Quaternion.identity;

                this.CurrentWeapon = selected;
            }
        }

        private void Shoot()
        {
            if (this.CurrentWeapon != null)
            {
                //
                // Shoot.
                //
                this.CurrentWeapon.Shoot();
            }
        }
    }
}
