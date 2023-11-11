using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jcsilva.AISystem {
    public class Creator {
        public static GameObject CreateWaypointsHolder() {
            GameObject _object = new GameObject();
            _object.name = "--- Waypoints ---";

            return _object;
        }

        public static GameObject CreateGameObject(GameObject parent, string name) {
            GameObject _object = new GameObject();
            _object.name = name;

            _object.transform.parent = parent.transform;

            return _object;
        }

        public static string SetInternalName(string title) {
            if (title != null)
                return title.Replace(" ", "_");
            else
                throw new System.Exception("Internal Name Can't be null.");
        }

    }
}
