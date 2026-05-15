using Input;
using Movement;
using PlayerAnimations;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private MovementHandler movementHandler;
        [SerializeField] private PlayerAnimation playerAnimation;

        private void Awake()
        {
            if (playerAnimation == null)
            {
                playerAnimation = GetComponent<PlayerAnimation>();
            }
        }

        private void Update()
        {
            Vector2 input = inputHandler.MoveInput;
            movementHandler.SetDirection(input);
            playerAnimation?.SetMovementInput(input);
        }
    }
}
