using UnityEngine;
using UnityEngine.InputSystem;

namespace MantaMinigames.Fishing
{
    // Entrada local para pruebas aisladas o botones de UI del minijuego.
    public sealed class FishingInputHandler : MonoBehaviour
    {
        [SerializeField] private bool pollKeyboardInput = true;
        [SerializeField] private bool pollMouseInput = true;
        [SerializeField] private Key confirmKey = Key.Space;
        [SerializeField] private Key alternativeConfirmKey = Key.Enter;
        [SerializeField] private Key cancelKey = Key.Escape;

        private bool confirmPressed;
        private bool cancelPressed;

        private void Update()
        {
            if (pollKeyboardInput && Keyboard.current != null)
            {
                if (Keyboard.current[confirmKey].wasPressedThisFrame || Keyboard.current[alternativeConfirmKey].wasPressedThisFrame)
                {
                    RegisterConfirmPressed();
                }

                if (Keyboard.current[cancelKey].wasPressedThisFrame)
                {
                    RegisterCancelPressed();
                }
            }

            if (pollMouseInput && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                RegisterConfirmPressed();
            }
        }

        public void RegisterConfirmPressed()
        {
            confirmPressed = true;
        }

        public void RegisterCancelPressed()
        {
            cancelPressed = true;
        }

        public bool ConsumeConfirmPressed()
        {
            if (!confirmPressed)
            {
                return false;
            }

            confirmPressed = false;
            return true;
        }

        public bool ConsumeCancelPressed()
        {
            if (!cancelPressed)
            {
                return false;
            }

            cancelPressed = false;
            return true;
        }

        public void ResetInput()
        {
            confirmPressed = false;
            cancelPressed = false;
        }
    }
}
