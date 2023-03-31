using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] private SO_Weapon[] _loadout;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Camera _camera;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private ParticleSystem _impact;
    [SerializeField] private Image _crosshair;
    [SerializeField] private float _bulletSpeed;

    private float _range = 100f;
    private float _currentCooldown;
    private int _currentIndex;
    private GameObject _currentWeapon;
    private float _origFOV = 80;
    private float _aimFOV = 50;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);

        if (_currentWeapon != null)
        {
            Aim(Input.GetMouseButton(1));
            Shoot(Input.GetMouseButtonDown(0) && _currentCooldown <= 0);

            _currentWeapon.transform.localPosition = Vector3.Lerp(_currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            if (_currentCooldown > 0) _currentCooldown -= Time.deltaTime;
        }
    }

    private void Shoot(bool isShooting)
    {

        if (isShooting)
        {
            Animator anim = _currentWeapon.GetComponent<Animator>();
            anim.SetTrigger("Shoot");
            ParticleSystem muzzleflash = _currentWeapon.transform.Find("Anchor/MuzzleFlash01").GetComponent<ParticleSystem>();
            TrailRenderer trail = Instantiate(_bulletTrail, muzzleflash.transform.position, Quaternion.identity); // etogo ne bilo
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            RaycastHit hitInfo;
            
            muzzleflash.Play();

            /*Vector3 bloom = _camera.transform.position + _camera.transform.forward * 1000f;
            bloom += Random.Range(_loadout[_currentIndex].Bloom, _loadout[_currentIndex].Bloom) * _camera.transform.up;
            bloom += Random.Range(_loadout[_currentIndex].Bloom, _loadout[_currentIndex].Bloom) * _camera.transform.right;
            bloom -= _camera.transform.position;
            bloom.Normalize();*/

            if (Physics.Raycast(ray, out hitInfo, _range))
            {
                var hitBox = hitInfo.collider.GetComponent<HitBox>();

                if (hitInfo.rigidbody != null)
                {
                    hitInfo.rigidbody.AddForceAtPosition(-hitInfo.normal * 1000f, hitInfo.point);
                }

                if (hitBox)
                {
                    hitBox.OnRaycastHit(_loadout[_currentIndex].Damage, ray.direction);
                }

                StartCoroutine(SpawnTrail(trail, hitInfo.point, hitInfo.normal, hitInfo, true));    
            }

            else
            {
                StartCoroutine(SpawnTrail(trail, _camera.transform.forward * 100000f, _camera.transform.forward, hitInfo, true));
            }


            _currentWeapon.transform.Rotate(-_loadout[_currentIndex].Recoil, 0, 0);
            _currentWeapon.transform.position -= _currentWeapon.transform.forward * _loadout[_currentIndex].Kickback;

            _currentCooldown = _loadout[_currentIndex].FireRate;
        }
    }

    private void Equip(int weaponIndex)
    {
        if (_currentWeapon != null) Destroy(_currentWeapon);

        _currentIndex = weaponIndex;

        GameObject newEquipment = Instantiate(_loadout[weaponIndex].Prefab, _weaponParent.position, _weaponParent.rotation, _weaponParent) as GameObject;
        newEquipment.transform.localPosition = Vector3.zero;
        newEquipment.transform.localEulerAngles = Vector3.zero;

        _currentWeapon = newEquipment;
        //_bulletSpawnPoint = _currentWeapon.transform.Find("Anchor/MuzzleFlash01");


    }

    private void Aim(bool isAiming)
    {
        Transform anchorEquipment = _currentWeapon.transform.Find("Anchor");
        Transform adsState = _currentWeapon.transform.Find("States/ADS");
        Transform hipState = _currentWeapon.transform.Find("States/Hip");


        if (isAiming)
        {
            _crosshair.gameObject.SetActive(false);
            anchorEquipment.position = Vector3.Lerp(anchorEquipment.position, adsState.position, Time.deltaTime * _loadout[_currentIndex].AimSpeed);
            _camera.fieldOfView = _aimFOV;
            _camera.fieldOfView = Mathf.LerpUnclamped(_aimFOV, _origFOV, Time.deltaTime * 5f);
        }



        else
        {
            _crosshair.gameObject.SetActive(true);
            anchorEquipment.position = Vector3.Lerp(anchorEquipment.position, hipState.position, Time.deltaTime * _loadout[_currentIndex].AimSpeed);
            _camera.fieldOfView = Mathf.LerpUnclamped(_origFOV, _aimFOV, Time.deltaTime * 5f);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, RaycastHit hit, bool MadeImpact)
    {
        // This has been updated from the video implementation to fix a commonly raised issue about the bullet trails
        // moving slowly when hitting something close, and not
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= _bulletSpeed * Time.deltaTime;

            yield return null;
        }
        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            var impact = Instantiate(_impact, HitPoint, Quaternion.LookRotation(HitNormal));
            impact.transform.parent = hit.transform;
            
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
