using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackPlayerState : AiState
{
    public AiStateId GetId() {
        return AiStateId.AttackPlayer;
    }

    public void Enter(AiAgent agent) {
        agent.weapons.ActivateWeapon();
        agent.weapons.SetTarget(agent.playerTransform);

        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
    }

    public void Update(AiAgent agent) {
        agent.navMeshAgent.destination = agent.playerTransform.position;
        ReloadWeapon(agent);
        SelectWeapon(agent);
        UpdateFiring(agent);
        if (agent.playerTransform.GetComponent<Health>().IsDead()) {
            agent.stateMachine.ChangeState(AiStateId.Idle);
        }
    }

    private void UpdateFiring(AiAgent agent) {
        if (agent.sensor.IsInSight(agent.playerTransform.gameObject)) {
            agent.weapons.SetFiring(true);
        } else {
            agent.weapons.SetFiring(false);
        }
    }

    public void Exit(AiAgent agent) {
        agent.navMeshAgent.stoppingDistance = 0.0f;
    }

    void ReloadWeapon(AiAgent agent) {
        var weapon = agent.weapons.currentWeapon;
        if (weapon && weapon.ammoCount <= 0) {
            agent.weapons.ReloadWeapon();
        }
    }

    void SelectWeapon(AiAgent agent) {
        var bestWeapon = ChooseWeapon(agent);
        if (bestWeapon != agent.weapons.currentWeaponSlot) {
            agent.weapons.SwitchWeapon(bestWeapon);
        }
    }

    AiWeapons.WeaponSlot ChooseWeapon(AiAgent agent) {
        float distance = Vector3.Distance(agent.playerTransform.position, agent.transform.position);
        if (distance > agent.config.attackCloseRange) {
            return AiWeapons.WeaponSlot.Primary;
        } else {
            return AiWeapons.WeaponSlot.Secondary;
        }
    }
}
