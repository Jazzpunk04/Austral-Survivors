using Health;
using UnityEngine;

namespace Combat
{
    public class PlayerProjectile : MonoBehaviour
    {
        private Vector2 _direction;
        private int _damage;
        private float _speed;
        private LayerMask _targetLayer;
        private Transform _owner;
        private float _destroyTime;
        private bool _hasHit;

        public void Initialize(
            Vector2 direction,
            int damage,
            float speed,
            float lifetime,
            LayerMask targetLayer,
            Transform owner)
        {
            _direction = direction.normalized;
            _damage = damage;
            _speed = speed;
            _targetLayer = targetLayer;
            _owner = owner;
            _destroyTime = Time.time + lifetime;
            _hasHit = false;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Update()
        {
            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));

            if (Time.time >= _destroyTime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasHit || IsOwner(other.transform))
            {
                return;
            }

            if ((_targetLayer.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            _hasHit = true;
            damageable.TakeDamage(_damage, _owner);
            Destroy(gameObject);
        }

        private bool IsOwner(Transform target)
        {
            return _owner != null && (target == _owner || target.IsChildOf(_owner));
        }
    }
}
