using UnityEngine;
using UnityEngine.Events;
using Bolt;

namespace Unity.FPS.Game
{
    public class Health : GlobalEventListener
    {
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction<GameObject> OnDie;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        bool m_IsDead;

        public GameObject lastDamageSource = null;

        void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnHeal action
            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            BoltEntity _sourceEntity = null;
            if (damageSource != null) _sourceEntity = damageSource.GetComponent<BoltEntity>();

            PlayerTakeDamageEvent damageEvent = PlayerTakeDamageEvent.Create();
            damageEvent.DamageTargetEntity = gameObject.GetComponent<BoltEntity>();
            damageEvent.DamageNumber = damage;

            if (_sourceEntity != null) damageEvent.DamageSourceEntity = _sourceEntity;

            damageEvent.Send();
        }

        public override void OnEvent(PlayerTakeDamageEvent evt)
        {
            if (gameObject.GetComponent<BoltEntity>().NetworkId != evt.DamageTargetEntity.NetworkId) return;

            // Set parameters
            float damage = evt.DamageNumber;

            GameObject damageSource = null;
            if (evt.DamageSourceEntity != null)
            {
                damageSource = evt.DamageSourceEntity.gameObject;
            }
            else
            {
                Debug.LogWarning("Damage ignored : Null damage source.");
                return;
            }

            // Inflict damage if not invincible or dead
            if (Invincible || m_IsDead)
                return;

            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            // call OnDamage action
            float trueDamageAmount = healthBefore - CurrentHealth;
            if (trueDamageAmount > 0f)
            {
                OnDamaged?.Invoke(trueDamageAmount, damageSource);
            }

            if (m_IsDead) return;
            lastDamageSource = damageSource;
            HandleDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f;

            // call OnDamage action
            OnDamaged?.Invoke(MaxHealth, null);

            lastDamageSource = null;
            HandleDeath();
        }

        void HandleDeath()
        {
            if (m_IsDead)
                return;

            // call OnDie action
            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;
                OnDie?.Invoke(gameObject);
            }
        }

        public void Revive()
        {
            if (!m_IsDead) return;
            lastDamageSource = null;
            CurrentHealth = MaxHealth;
            m_IsDead = false;
        }
    }
}