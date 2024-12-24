using Unity.VisualScripting;
using UnityEngine;

namespace PlayerSystem
{
    public class SquarePowerModule
    {
        private EventBus eventBus;
        private PlayerState playerState;

        private Rigidbody2D rb2d;
        private TriggerEventHandler groundTrigger;

        private readonly float minPowerDuration = 0.2f;
        private float powerTimeSum = 0f;
        private readonly float cooldownDuration = 0.5f;
        private float cooldownTimeLeft = 0f;
        private bool isActive = false;

        public SquarePowerModule(EventBus eventBus, PlayerState playerState, Rigidbody2D rb2d, TriggerEventHandler groundTrigger)
        {
            this.eventBus = eventBus;
            this.playerState = playerState;
            this.rb2d = rb2d;
            this.groundTrigger = groundTrigger;

            eventBus.Subscribe<SquarePowerInputEvent>(togglePower);
        }

        private void togglePower(SquarePowerInputEvent e)
        {
            if (e.toggle == isActive) return;
            if (e.toggle) activate();
            else /*if (minPowerDuration < powerTimeSum)*/ deactivate();
            // Todo: Current implementation of minPowerDuration feels uncomfortable during play, figure a way to better handle deactivation input
        }

        private void activate()
        {
            if (0f < cooldownTimeLeft) return;

            isActive = true;
            this.playerState.activePower = Power.Square;
            powerTimeSum = 0;
            rb2d.velocity = new Vector2(0, -10f);
            groundTrigger.OnTriggerEnter2DAction.AddListener(onTriggerEnter);

            eventBus.Subscribe<UpdateEvent>(addTimeSum);
            eventBus.Publish(new ToggleSquarePowerEvent(true));
        }

        private void addTimeSum(UpdateEvent e)
        {
            powerTimeSum += Time.deltaTime;
        }

        private void onTriggerEnter(Collider2D other)
        {
            if (other.CompareTag("Breakable")) GameObject.Destroy(other.gameObject);
            if (other.gameObject.CompareTag("Platform"))
            {
                other.gameObject.GetComponent<IPlatform>()?.PlatformEnterAction(playerState, rb2d);
            }
            // Todo: Place enemy collision logic here
        }

        private void deactivate()
        {
            isActive = false;
            this.playerState.activePower = Power.None;
            cooldownTimeLeft = cooldownDuration;
            groundTrigger.OnTriggerEnter2DAction.RemoveListener(onTriggerEnter);

            eventBus.Subscribe<UpdateEvent>(reduceCooldown);
            eventBus.Unsubscribe<UpdateEvent>(addTimeSum);
            eventBus.Publish(new ToggleSquarePowerEvent(false));
        }

        private void reduceCooldown(UpdateEvent e)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (0 < cooldownTimeLeft) return;

            eventBus.Unsubscribe<UpdateEvent>(reduceCooldown);
        }
    }
}