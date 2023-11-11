using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace jcsilva.AISystem {

    public class ChaseBehaviour : AIBehaviour {

        // Behaviour Settings
        private bool isActive;

        private Transform target;
        private Transform selfTransform;
        private NavMeshAgent agent;
        private Animator animator;

        // Vision Settings
        private float maxDistance;
        private float maxFieldOfView;
        private float minDistanceToAttack;

        public ChaseBehaviour(MonoBehaviour self, AIStateMachine aIStateMachine) : base(self, aIStateMachine, "Chase") {
            this.target = stateMachine.GetTargetPosition();
            this.selfTransform = stateMachine.GetSelfPosition();
            this.agent = stateMachine.GetNavMeshAgent();
            this.maxDistance = stateMachine.GetMaxDistanceToView();
            this.maxFieldOfView = stateMachine.GetMaxFieldOfView();
            this.minDistanceToAttack = stateMachine.GetMinDistanceToAttack();
            this.animator = stateMachine.GetAnimator();
        }

        public override void OnBehaviourStart() {
            isActive = true;
            selfTransform.LookAt(target);
            stateMachine.EventAIEnableChase?.Invoke();

            ChasePlayer();

            // All things that need to be set when this behaviour starts should be put bellow this comment
            // example: Animator.SetBool("AnimationRun", true);
            animator.SetBool("run", true);
        }

        public override void OnBehaviourEnd() {
            isActive = false;
            agent.ResetPath();
            stateMachine.EventAIDisableChase?.Invoke();

            // All things that need to be set when this behaviour ends should be put bellow this comment
            // example: Animator.SetBool("AnimationRun", false);
            animator.SetBool("run", false);
        }


        public override void OnUpdate() {
            if (isActive) {
                if (AIUtils.HasVisionOfTarget(selfTransform, target, maxDistance, maxFieldOfView)) {
                    if (AIUtils.IsInRange(selfTransform, target, minDistanceToAttack)) {
                        stateMachine.HandleState(AIEvents.InRange);
                        return;
                    } else {
                        ChasePlayer();
                    }
                } else {
                    stateMachine.HandleState(AIEvents.LostPlayer);
                    return;
                }
            }

        }

        private void ChasePlayer() {
            selfTransform.LookAt(target);
            agent.SetDestination(target.position);
        }
    }
}