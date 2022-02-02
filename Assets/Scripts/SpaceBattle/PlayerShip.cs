using System;
using DG.Tweening;
using UnityEngine;

namespace SpaceBattle
{
    public class PlayerShip : MonoBehaviour
    {
        [SerializeField] private Transform turretTransform;


        public void LookAtTarget(Transform doTransform, Vector3 targetPosition, float duration)
        {
            doTransform.DOKill();

            var dir = targetPosition - doTransform.position;
            var rotation = Quaternion.FromToRotation(doTransform.up, dir) * doTransform.rotation;

            doTransform.DORotateQuaternion(rotation, duration);
        }

    }
}
