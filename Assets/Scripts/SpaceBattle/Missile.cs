using UnityEngine;

namespace SpaceBattle
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float rotationSpeed = 30;

        private Transform targetTransform;
        private Transform thisTransform;

        public void Fire(Transform target)
        {
            enabled = true;
            Destroy(gameObject, lifeTime);
            targetTransform = target;
            thisTransform = transform;
        }
        
        private void Update()
        {
            var dir = targetTransform.position - thisTransform.position;
            var rotation = Quaternion.FromToRotation(thisTransform.up, dir) * transform.rotation;
            thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rotation, Time.deltaTime * rotationSpeed);
            
            thisTransform.Translate(Time.deltaTime * speed * transform.up, Space.World);
        }
    }
}
