using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;
        private Rigidbody2D rb2d;
        private TriggerEventHandler upTrigger;
        private PlayerMovementScriptable movementValues;
        private float powerTimeLeft = 0f;
        private float cooldownTimeLeft = 0f;
        private bool isActive = false;

        public TrianglePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler upTrigger, PlayerMovementScriptable movementValues)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.upTrigger = upTrigger;
            this.movementValues = movementValues;
            eventBus.Subscribe<TrianglePowerInputEvent>(activate);
        }

        private void activate(TrianglePowerInputEvent e)
        {
            if (isActive) return;
            if (cooldownTimeLeft > 0f) return;
            isActive = true;
            playerState.activePower = Power.Triangle;
            rb2d.linearVelocity = new Vector2(0, 5f).normalized * movementValues.trianglePowerForce;
            powerTimeLeft = movementValues.trianglePowerDuration;

            upTrigger.OnTriggerEnter2DAction.AddListener(onTriggerEnter);

            eventBus.Subscribe<UpdateEvent>(reduceTimeLeft);
            eventBus.Subscribe<UpdateEvent>(deactivateOnMomentumLoss);
            eventBus.Publish(new ToggleTrianglePowerEvent(true));
        }

        private void reduceTimeLeft(UpdateEvent e)
        {
            powerTimeLeft -= Time.deltaTime;
            if (0 < powerTimeLeft) return;
            deactivate();
        }

        private void deactivateOnMomentumLoss(UpdateEvent e)
        {
            if (0 < Mathf.Abs(rb2d.linearVelocity.y)) return;
            deactivate();
        }

        private void deactivate()
        {
            isActive = false;
            playerState.activePower = Power.None;
            upTrigger.OnTriggerEnter2DAction.RemoveListener(onTriggerEnter);

            cooldownTimeLeft = movementValues.trianglePowerCooldown;
            eventBus.Subscribe<UpdateEvent>(reducePowerCooldown);

            eventBus.Unsubscribe<UpdateEvent>(reduceTimeLeft);
            eventBus.Unsubscribe<UpdateEvent>(deactivateOnMomentumLoss);
            eventBus.Publish(new ToggleTrianglePowerEvent(false));
        }

        private void reducePowerCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft > 0f) return;

            eventBus.Unsubscribe<UpdateEvent>(reducePowerCooldown);
        }

        private void onTriggerEnter(Collider2D other)
        {
            if (other.CompareTag("Breakable"))
            {
                eventBus.Publish(new PlayPlayerSounEffect("TriangleBreakWall", 0.5f));
            }
            other.gameObject.GetComponent<IPlayerPowerInteractable>()?.PlayerPowerInteraction(playerState);
        }
    }
}