﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{ 
    public class EnemySpaceShip : SpaceShipBase
    {
        [SerializeField]
        private float _reachedDistance = 0.5f;

        private GameObject[] _movementTargets;

        [SerializeField]
        private GameObject healthPrefab;

        [SerializeField]
        private int healthSpawner;

        private int _currentMovementTargetIndex = 0;

        public Transform CurrentMovementTarget
        {
            get
            {
                return _movementTargets[_currentMovementTargetIndex].transform;
            }
        }

        public override Type UnitType
        {
            get { return Type.Enemy; }
        }

        protected override void Update()
        {
            base.Update();

            Shoot();
        }

        protected override void Die()
        {
            base.Die();

            int i = Random.Range(0, 100);

            if (i < healthSpawner)
            {
                SpawnHealth();

                Debug.Log("SHOULD'VE SPAWNED A HEALTH PACK");
            }

        }

        public void SpawnHealth()
        {
            healthPrefab = Instantiate(healthPrefab, transform.position, transform.rotation);
        }

        public void SetMovementTargets(GameObject[] movementTargets)
        {
            _movementTargets = movementTargets;
            _currentMovementTargetIndex = 0;
        }

        protected override void Move()
        {
            if(_movementTargets == null || _movementTargets.Length == 0)
            {
                return;
            }

            UpdateMovementTarget();
            Vector3 direction = (CurrentMovementTarget.position - transform.position).normalized;
            transform.Translate(direction * Speed * Time.deltaTime);
        }

        private void UpdateMovementTarget()
        {
            if(Vector3.Distance(transform.position, CurrentMovementTarget.position) < _reachedDistance)
            {
                if(_currentMovementTargetIndex >= _movementTargets.Length - 1)
                {
                    _currentMovementTargetIndex = 0;
                }
                else
                {
                    _currentMovementTargetIndex++;
                }
            }
        }
    }
}

