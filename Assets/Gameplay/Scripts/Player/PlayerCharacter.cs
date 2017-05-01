using TestGame.Core;
using TestGame.Gameplay;

namespace TestGame.Player
{
    public class PlayerCharacter : CharacterBase
    {
        public PlayerController Controller;

        public void Start()
        {
            this.Controller = this.GetComponent<PlayerController>();
        }
        
        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            //
            // When player takes damage, notify HUD to update itself.
            //
            HudController.Instance.NotifyUpdateHealth();
        }

        protected override void OnCharacterDied()
        {
            //
            // Notify game character that player passe away.
            //
            GameController.Instance.PlayerPassedAway();
        }
    }
}
