using Input;
using Movement;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private MovementHandler movementHandler;

        private void Update()
        {
            Vector2 input = inputHandler.MoveInput;
            movementHandler.SetDirection(input);
        }
    }
}
