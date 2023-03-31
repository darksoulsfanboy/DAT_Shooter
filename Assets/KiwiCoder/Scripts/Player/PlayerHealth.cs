using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : Health
{
    public float dieForce = 15.0f;
    public GameObject Weapon;
    public GameOver gameOver;
    public TextMeshProUGUI hp;
    Ragdoll ragdoll;
    ActiveWeapon weapons;
    CharacterAiming aiming;
    PostProcessVolume postProcessing;
    CameraManager cameraManager;

    protected override void OnStart() {
        ragdoll = GetComponent<Ragdoll>();
        weapons = GetComponent<ActiveWeapon>();
        aiming = GetComponent<CharacterAiming>();
        postProcessing = FindObjectOfType<PostProcessVolume>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    protected override void OnDeath(Vector3 direction) {
        hp.text = "HP: 0";
        gameOver.GameOverOn();
        gameObject.SetActive(false);
        Weapon.SetActive(false);
        GameManager.IsGameOver = true;

        /*Debug.Log("Ded");
        ragdoll.ActivateRagdoll();
        direction.Normalize();
        direction.y = 1.0f;
        ragdoll.ApplyForce(direction * dieForce);
        weapons.DropWeapon();
        aiming.enabled = false;
        cameraManager.EnableKillCam();*/
    }

    protected override void OnDamage(Vector3 direction) {
        hp.text = $"HP: {currentHealth}";
        Vignette vignette = postProcessing.profile.GetSetting<Vignette>();
        if (vignette) {
            float percent = 1.0f - (currentHealth / maxHealth);
            vignette.intensity.value = percent * 0.6f;
        }
    }
}
