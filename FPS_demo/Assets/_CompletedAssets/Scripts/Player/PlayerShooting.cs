using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
		public int damagePerShot = 50;                  // The damage inflicted by each bullet.
		public float timeBetweenBullets = 0.1f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.
		public Slider ammoSlider;
		public int maxBullets;
		public Image enemyIcon;
		public Slider enemyHealthSlider;
		public float reloadTime = 2.0f;        // The time between each shot.
		public int currentBullets;
		public Light faceLight;								// Duh
		public Transform gun;

		private float reloadingTimer;
		private GameObject enemyHealthObject;


        float timer;                                    // A timer to determine when to fire.
        Ray shootRay;                                   // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
		AudioSource gunAudio;                           // Reference to the audio source.
		AudioSource gunReloadAudio;                     // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
        float effectsDisplayTime = 0.1f;                // The proportion of the timeBetweenBullets that the effects will display for.
		Vector3 gunInitialRotation;
		Vector3 rotationOffset;
		Vector3 initialRotation;

        void Awake ()
        {
			shootRay = new Ray ();
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

			initialRotation = transform.localRotation.eulerAngles;
			gunInitialRotation = gun.localRotation.eulerAngles;

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem> ();
			gunLine = GetComponent <LineRenderer> ();

			foreach (AudioSource aSource in GetComponents<AudioSource>())
			{
				Debug.Log ("AudioSource: " + aSource.clip.name);
				if (aSource.clip.name.Equals ("Player GunShot"))
					gunAudio = aSource;
				else if (aSource.clip.name.Equals ("ak47reload"))
					gunReloadAudio = aSource;
			}

            gunLight = GetComponent<Light> ();
			faceLight = GetComponentInChildren<Light> ();
			ammoSlider.maxValue = maxBullets;
			currentBullets = maxBullets;
			enemyHealthObject = enemyHealthSlider.gameObject;
        }


        void Update ()
        {
			rotationOffset = gunInitialRotation - gun.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(initialRotation + rotationOffset);

            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;
			if (reloadingTimer >= 0)
				reloadingTimer -= Time.deltaTime;

			// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
			shootRay.origin = transform.position;
			shootRay.direction = transform.forward;

			// Perform the raycast against gameobjects on the shootable layer and if it hits something...
			if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
			{
				// Try and find an EnemyHealth script on the gameobject hit.
				EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
				if (enemyHealth) {
					enemyIcon.sprite = enemyHealth.icon;
					enemyIcon.enabled = true;
					enemyHealthObject.SetActive (true);
					enemyHealthSlider.value = enemyHealth.currentHealth;
					enemyHealthSlider.maxValue = enemyHealth.startingHealth;
				} else
				{
					enemyIcon.enabled = false;
					enemyHealthObject.SetActive (false);
				}
			}

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0 && reloadingTimer <= 0)
            {
                // ... shoot the gun.
                Shoot ();
            }
			if (Input.GetButton("Reload"))
			{
				ReloadAmmo();
			}
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


		public void ReloadAmmo()
		{
			gunReloadAudio.Play ();

			reloadingTimer = reloadTime;
			currentBullets = maxBullets;
		}


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
        {
            // Reset the timer.
            timer = 0f;

			if (currentBullets <= 0) return;
			currentBullets -=1;

			ammoSlider.value = currentBullets;
            // Play the gun shot audioclip.
            gunAudio.Play ();

            // Enable the lights.
            gunLight.enabled = true;
			faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop ();
            gunParticles.Play ();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
			gunLine.SetPosition (0, transform.position);

			// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
			shootRay.origin = transform.position;
			shootRay.direction = transform.forward;

			// Perform the raycast against gameobjects on the shootable layer and if it hits something...
			if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
			{
				// Try and find an EnemyHealth script on the gameobject hit.
				EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();

                // If the EnemyHealth component exist...
                if(enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage (damagePerShot, shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition (1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
            }
        }
    }
}