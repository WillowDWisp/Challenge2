using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace jcsilva.AISystem {

    public class AttackBehaviour : AIBehaviour {

        // Behaviour Settings
#pragma warning disable IDE0052 // Remove unread private members
        private bool isActive;
#pragma warning restore IDE0052 // Remove unread private members

        // References
        private Transform selfTransform;
        private Transform target;
        private NavMeshAgent agent;
        private Animator animator;

        // Vision Settings
        private float maxDistance;
        private float maxFieldOfView;
        private float minDistanceToAttack;

        public AttackBehaviour(MonoBehaviour self, AIStateMachine aiStateMachine) : base(self, aiStateMachine, "Attack") {
            this.selfTransform = stateMachine.GetSelfPosition();
            this.target = stateMachine.GetTargetPosition();
            this.agent = stateMachine.GetNavMeshAgent();
            this.maxDistance = stateMachine.GetMaxDistanceToView();
            this.maxFieldOfView = stateMachine.GetMaxFieldOfView();
            this.minDistanceToAttack = stateMachine.GetMinDistanceToAttack();
            this.animator = stateMachine.GetAnimator();
        }

        public override void OnBehaviourStart() {
            isActive = true;
            stateMachine.EventAIEnableAttack?.Invoke();

            // All things that need to be set when this behaviour start, should be added bellow this comment
            // example: Animator.SetBool("AnimationShooting", true);
            animator.SetBool("attack", true);
        }

        public override void OnBehaviourEnd() {
            isActive = false;
            stateMachine.EventAIDisableAttack?.Invoke();

            // All things that need to be set when this behaviour ends, should be added bellow this comment
            // example: Animator.SetBool("AnimationShooting", false);
            animator.SetBool("attack", false);
        }


        public override void OnUpdate() {

            if (AIUtils.HasVisionOfTarget(selfTransform, target, maxDistance, maxFieldOfView)) {
                if (AIUtils.IsInRange(selfTransform, target, minDistanceToAttack)) {
                    selfTransform.LookAt(target);
                    stateMachine.EventAIEnableAttack?.Invoke();
                } else {
                    stateMachine.HandleState(AIEvents.OutOfRange);
                    return;
                }
            } else {
                stateMachine.HandleState(AIEvents.LostPlayer);
                return;
            }

        }
    }
}
