using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jcsilva.AISystem;

namespace jcsilva.CharacterController {

    public class EnemyController : MonoBehaviour {

        [Header("References")]
        [SerializeField] AIStateMachine stateMachine;

        [Header("Enemy Settings")]
        [SerializeField] float fireRate = 1f;
        [SerializeField] float elapsedTime;

        private bool canShoot;

        private void Awake() {
            if (stateMachine == null) {
                stateMachine = GetComponent<AIStateMachine>();
            }
        }

        private void OnEnable() {
            stateMachine.EventAIEnableAttack += Shoot;
            stateMachine.EventAIDisableAttack += CantShoot;
        }

        private void OnDisable() {
            stateMachine.EventAIEnableAttack -= Shoot;
            stateMachine.EventAIDisableAttack -= CantShoot;
        }

        // Update is called once per frame
        void Update() {
            if (canShoot) {
                if(elapsedTime > fireRate) {
                    IsShooting();
                    elapsedTime = 0f;
                } else {
                    elapsedTime += Time.deltaTime;
                }
            } else if (!canShoot && elapsedTime > 0f) {
                if(elapsedTime > fireRate) {
                    elapsedTime = 0f;
                } else {
                    elapsedTime += Time.deltaTime;
                }
            }
            
        }

        private void Shoot() {
            canShoot = true;
        }

        private void CantShoot() {
            canShoot = false;
        }

        private void IsShooting() {
            Debug.Log("I'm Shooting");
        }
    }
}
