using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jcsilva.AISystem {

    /// <summary>
    /// Utility Class to support all common behaviours logic
    /// </summary>
    public class AIUtils {

        /// <summary>
        /// Method that will return if self has vision of his target
        /// </summary>
        /// <param name="self">Self Transform</param>
        /// <param name="target">Target Transform</param>
        /// <param name="maxDistanceRange">Maximum Distance allowed between self and target</param>
        /// <param name="maxFieldOfView">Maximum Field of View allowed between self and target</param>
        /// <returns>If self has vision of the target</returns>
        public static bool HasVisionOfTarget(Transform self, Transform target, float maxDistanceRange, float maxFieldOfView) {

            // Calculate the direction between self and the target
            Vector3 direction = target.position - self.position;

            // Calculates the distance between self and the target
            float distance = direction.magnitude;

            // Case the distance between self and target is greater than the Maximum Distance return false.
            if (distance > maxDistanceRange) {
                return false;
            }

            // Calculate the View Distance between self and the target
            float viewAngle = Vector3.Angle(self.transform.forward, direction);

            // Case the angle between self and target is greater than the Maximum angle vision return false.
            if (viewAngle > maxFieldOfView) {
                return false;
            }

            // If all the above settings aren't true, then we will create a Raycast that will check if 
            // self actualy has vision of the target
            Ray visionRay = new Ray(self.position + Vector3.up, direction);
            RaycastHit hitInfo;

            if (Physics.Raycast(visionRay, out hitInfo, maxDistanceRange)) {
                if (hitInfo.transform.tag == target.tag) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInRange(Transform self, Transform target, float distanceToAttack) {

            if (Vector3.Distance(self.position, target.position) < distanceToAttack) {
                return true;
            }

            return false;
        }

    }
}