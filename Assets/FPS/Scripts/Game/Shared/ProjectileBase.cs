using UnityEngine;
using UnityEngine.Events;
using Bolt;

namespace Unity.FPS.Game
{
    public abstract class ProjectileBase : EntityEventListener<IProjectileState>
    {
        public GameObject Owner { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitialCharge { get; private set; }

        public UnityAction OnShoot;
        
        public bool IsServerInstance = false;

        protected bool isAttached = false;

        public override void Attached()
        {
            isAttached = true;
            state.SetTransforms(state.BlasterProjectileTransform, transform);
        }

        public override void Detached()
        {
            isAttached = false;
        }

        public void Shoot(WeaponController controller)
        {
            Owner = controller.Owner;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            /*if(isAttached)*/ OnShoot?.Invoke();
        }
    }
}