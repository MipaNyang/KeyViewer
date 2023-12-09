using UnityEngine;

namespace KeyViewer
{
    public static class KeyInput
    {
        public static bool AnyKey => AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl ? AsyncInputCompat.AnyKey : Input.anyKey;
        public static bool AnyKeyDown => AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl ? AsyncInputCompat.AnyKeyDown : Input.anyKeyDown;
        public static bool GetKey(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKey(code);
            return Input.GetKey(code);
        }
        public static bool GetKeyUp(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKeyUp(code);
            return Input.GetKeyUp(code);
        }
        public static bool GetKeyDown(KeyCode code)
        {
            if (AsyncInputManager.isActive && scrController.instance.currentState == States.PlayerControl)
                return AsyncInputCompat.GetKeyDown(code);
            return Input.GetKeyDown(code);
        }
    }
}
