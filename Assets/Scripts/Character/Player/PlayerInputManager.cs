using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JH
{
    public class PlayerInputManager : MonoBehaviour
    {
        private static PlayerInputManager _instance;
        public static PlayerInputManager instance => _instance;
        public PlayerManager player;
        PlayerControls playerControls;

        [Header("Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Camera Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("Player Action Input")]
        [SerializeField] bool dodgeInput = false;

        private void Awake()
        {
            if (null == _instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnSceneChange(Scene OldScene, Scene NewScene)
        {
            if (WorldSaveGameManager.instance.GetWorldSceneIndex() == NewScene.buildIndex)
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (null == playerControls)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandleCameraMovementInput();
            HandlePlayerMovementInput();
            HandleDodgeInput();
        }

        // Movement
        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            if (0.5f >= moveAmount && 0.0f < moveAmount)
            {
                moveAmount = 0.5f;
            }
            else if (0.5f < moveAmount &&  1.0f >= moveAmount)
            {
                moveAmount = 1;
            }

            if (null == player)
            {
                return;
            }

            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
        
        // Action
        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;

                player.playerLocomotionManager.AttempToPerformDodge();
            }
        }
    }
}

