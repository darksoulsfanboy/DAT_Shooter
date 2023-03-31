using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 0)]
public class SO_Weapon : ScriptableObject
{
    public string Name;
    public float FireRate;
    public GameObject Prefab;
    public float Bloom;
    public float Recoil;
    public float Damage;
    public float AimSpeed;
    public float Kickback;
}
