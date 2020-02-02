using EntroPi;
using UnityEngine;

namespace Snake3D.Player
{
    public class PlayerInput : EntroPiBehaviour
    {
        public enum InputAction { None, TurnLeft, TurnRight };

        [SerializeField]
        private KeyCode m_TurnLeftKey = KeyCode.LeftArrow;

        [SerializeField]
        private KeyCode m_TurnRightKey = KeyCode.RightArrow;

        private InputAction m_LastInputAction = InputAction.None;

        public InputAction GetAndResetLastInputAction()
        {
            InputAction lastInputAction = m_LastInputAction;
            m_LastInputAction = InputAction.None;

            return lastInputAction;
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_TurnLeftKey) == true)
            {
                m_LastInputAction = InputAction.TurnLeft;
            }
            if (Input.GetKeyUp(m_TurnRightKey) == true)
            {
                m_LastInputAction = InputAction.TurnRight;
            }
        }
    }
}