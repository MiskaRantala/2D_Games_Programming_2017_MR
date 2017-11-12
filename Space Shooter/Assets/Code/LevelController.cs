﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController Current
        {
            get; private set;
        }

        [SerializeField]
        private Spawner _playerSpawner;

        [SerializeField]
        private Spawner _enemySpawner;

        [SerializeField]
        private GameObject[] _enemyMovementTargets;

        // How often we should spawn a new enemy.
        [SerializeField]
        private float _spawnInterval = 1;

        [SerializeField, Tooltip("The time before the first spawn.")]
        private float _waitToSpawn;

        // Maximum amount of enemies to spawn.
        [SerializeField]
        private int _maxEnemyUnitsToSpawn;

        [SerializeField]
        private GameObjectPool _playerProjectilePool;

        [SerializeField]
        private GameObjectPool _enemyProjectilePool;

        [SerializeField]
        private PlayerSpaceship _playerUnit;

        [SerializeField]
        private Text _healthText;

        //Amount of enemies spawned so far.
        private int _enemyCount;

        protected void Awake()
        {
            if (Current == null)
            {
                Current = this;
            }
            else
            {
                Debug.LogError("Multiple LevelControllers in the scene!");
            }

            if (_playerSpawner == null)
            {
                Debug.Log("No reference to a player spawner!");
                //_playerSpawner = GameObject.FindObjectOfType<Spawner>();
                _playerSpawner = GetComponentInChildren<Spawner>();
            }

            if (_enemySpawner == null)
            {
                Debug.Log("No reference to an enemy spawner!");
                //_enemySpawner = GameObject.FindObjectOfType<Spawner>();
                _enemySpawner = GetComponentInChildren<Spawner>();
            }
        }

        protected void Start()
        {
            _playerUnit = SpawnPlayer();
            SetHealthText();

            //Starts a new coroutine.
            StartCoroutine(SpawnEnemyRoutine());
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            // Wait for a while before spawning the first enemy.
            yield return new WaitForSeconds(_waitToSpawn);

            while(_enemyCount < _maxEnemyUnitsToSpawn)
            {
                EnemySpaceShip enemy = SpawnEnemyUnit();
                if(enemy != null)
                {
                    _enemyCount++;
                }
                else
                {
                    Debug.LogError("Could not spawn an enemy!");
                    yield break; //Stops the execution of this coroutine.
                }
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        public PlayerSpaceship SpawnPlayer()
        {
            PlayerSpaceship playerShip = null;
            GameObject spawnedPlayerObject = _playerSpawner.Spawn();
            if (spawnedPlayerObject != null)
            {
                playerShip = spawnedPlayerObject.GetComponent<PlayerSpaceship>();
            }

            playerShip.BecomeImmortal();

            return playerShip;

            //if (_playerUnit.playerLives > 0)
            //{
            //    _playerSpawner.Spawn();
            //    _playerUnit.GetComponent<Health>().IncreaseHealth(_playerUnit.GetComponent<Health>().MaximumHealth);
            //}
        }

        private EnemySpaceShip SpawnEnemyUnit()
        {
            GameObject spawnedEnemyObject = _enemySpawner.Spawn();
            EnemySpaceShip enemyShip = spawnedEnemyObject.GetComponent<EnemySpaceShip>();
            if(enemyShip != null)
            {
                enemyShip.SetMovementTargets(_enemyMovementTargets);
            }
            return enemyShip;
        }

        public Projectile GetProjectile(SpaceShipBase.Type type)
        {
            GameObject result = null;

            // Try to get pooled object from the correct pool based on
            // the type of the spaceship.
            if(type == SpaceShipBase.Type.Player)
            {
                result = _playerProjectilePool.GetPooledObject();
            }
            else
            {
                result = _enemyProjectilePool.GetPooledObject();
            }

            // If the pooled object was found, get the Projectile
            // component from it and return that. Otherwise just
            // return null.
            if(result != null)
            {
                Projectile projectile = result.GetComponent<Projectile>();
                if (projectile == null)
                {
                    Debug.LogError("Projectile component could not be found from the object fetched from the pool");
                }
                return projectile;
            }
            return null;
        }

        public bool ReturnProjectile(SpaceShipBase.Type type, Projectile projectile)
        {
            if(type == SpaceShipBase.Type.Player)
            {
                return _playerProjectilePool.ReturnObject(projectile.gameObject);
            }
            else
            {
                return _enemyProjectilePool.ReturnObject(projectile.gameObject);
            }
        }

        void Update()
        {
            if (_playerUnit.GetComponent<Health>().CurrentHealth <= 0)
            {
                if (_playerUnit.playerLives > 0)
                {
                    PlayerSpaceship player = SpawnPlayer();
                    _playerUnit.GetComponent<Health>().IncreaseHealth(_playerUnit.GetComponent<Health>().MaximumHealth);
                }
            }
            SetHealthText();
        }

        public void SetHealthText()
        {
            if (_healthText != null)
            {
                int playerHealth = _playerUnit.GetComponent<Health>().CurrentHealth;
                _healthText.text = "Player Health = " + playerHealth;
                Debug.Log(playerHealth);
            }
        }

        public void LivesLost()
        {
            if(GameManager.Instance.CurrentLives <= 0)
            {

            }
            else
            {
                _playerUnit = SpawnPlayer();
            }
        }
    }
}
