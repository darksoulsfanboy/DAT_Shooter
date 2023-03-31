using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileWeaponSystem : MonoBehaviour
{
    [Header("Bullet")]

    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _shootForce, _upwardForce;

    [Header("Gun Stats")]
    [SerializeField] private float _timeBetweenShooting, _spread, _reloadTime, _timeBetweenShots;
    [SerializeField] private int _magazineSize, _bulletsPerTap;
    [SerializeField] private bool _allowButtonHold;

    private int _bulletsLeft, _bulletsShot;
    private bool _isShooting, _isReadyToShoot, _isReloading;

    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private float _recoilForce;

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _attackPoint;

    [Header("Graphics")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private TextMeshProUGUI _ammoDisplay;

    public bool AllowInvoke = true;

    private void Awake()
    {
        _bulletsLeft = _magazineSize;
        _isReadyToShoot = true;
    }

    private void Update()
    {
        PlayerInput();

        _ammoDisplay?.SetText($"{_bulletsLeft / _bulletsPerTap} / {_magazineSize / _bulletsPerTap}");
    }

    private void PlayerInput()
    {
        if (_allowButtonHold) 
            _isShooting = Input.GetKey(KeyCode.Mouse0);
        else 
            _isShooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < _magazineSize && !_isReloading) 
            Reload();

        if (_isReadyToShoot && _isShooting && !_isReloading && _bulletsLeft <= 0)
            Reload();

        if (_isReadyToShoot && _isShooting && !_isReloading && _bulletsLeft > 0)
        {
            _bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {

        _isReadyToShoot = false;

        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.44f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - _attackPoint.position;

        float xSpread = Random.Range(-_spread, _spread);
        float ySpread = Random.Range(-_spread, _spread);
        Vector3 spread = new Vector3(xSpread, ySpread, 0);


        Vector3 directionWithSpread = directionWithoutSpread + spread;

        GameObject currentBullet = Instantiate(_bullet, _attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce((directionWithSpread.normalized ) * _shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(_camera.transform.up * _upwardForce, ForceMode.Impulse);


        if (_muzzleFlash != null)
            _muzzleFlash.Emit(1);
           // Instantiate(_muzzleFlash, _attackPoint.position, Quaternion.identity);

        _bulletsLeft--;
        _bulletsShot++;

        if (AllowInvoke)
        {
            Invoke("ResetShot", _timeBetweenShooting);
            AllowInvoke = false;

            _playerRb.AddForce(-directionWithSpread.normalized * _recoilForce, ForceMode.Impulse);
        }

        if (_bulletsShot < _bulletsPerTap && _bulletsLeft > 0)
            Invoke("Shoot", _timeBetweenShots);
    }

    private void ResetShot()
    {
        _isReadyToShoot = true;
        AllowInvoke = true;
    }

    private void Reload()
    {
        _isReloading = true;
        Invoke("ReloadFinished", _reloadTime);
    }

    private void ReloadFinished()
    {
        _bulletsLeft = _magazineSize;
        _isReloading = false;
    }

}
