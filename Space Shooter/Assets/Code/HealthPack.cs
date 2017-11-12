using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class HealthPack : MonoBehaviour
    {

        public int healthLifeSpan;
        public int healthAmount;
        public float movementSpeed;



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);

            healthLifeSpan -= 1;

            if (healthLifeSpan == 0)
            {
                Destroy(gameObject);    // Destroy the health after given time.
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            Health playerHealth = other.GetComponent<Health>(); //Get the player Health component

            playerHealth.IncreaseHealth(healthAmount);         //Heal the player up. 
            Debug.Log("Health Increased!");
            Destroy(gameObject);                                // Destroy the health item after usage.
        }
    }
}