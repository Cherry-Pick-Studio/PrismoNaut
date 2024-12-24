using UnityEngine;

namespace PlayerSystem
{
    public class TrianglePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;

        private Rigidbody2D rb2d;
        private TriggerEventHandler upTrigger;

        private readonly float powerDuration = 0.5f;
        private float powerTimeLeft = 0f;
        private float cooldownTimeLeft = 0f;
        private bool isActive = false;

        public TrianglePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler upTrigger)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.upTrigger = upTrigger;

            eventBus.Subscribe<TrianglePowerInputEvent>(activate);
        }

        private void activate(TrianglePowerInputEvent e)
        {
            if (isActive) return;
            if (cooldownTimeLeft > 0f) return;

            isActive = true;
            rb2d.velocity = new Vector2(0, 10f);
            powerTimeLeft = powerDuration;

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
            if (0 < Mathf.Abs(rb2d.velocity.y)) return;
            deactivate();
        }

        private void deactivate()
        {
            isActive = false;

            upTrigger.OnTriggerEnter2DAction.RemoveListener(onTriggerEnter);

            cooldownTimeLeft = 1f;
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
            if (other.CompareTag("Breakable")) Object.Destroy(other.gameObject);
        }
    }
}