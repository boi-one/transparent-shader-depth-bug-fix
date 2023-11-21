// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerInputMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputMap"",
    ""maps"": [
        {
            ""name"": ""PlayerControls"",
            ""id"": ""b415aec9-fb71-4aad-85f2-b1dc66430568"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""72ce53e3-af2e-4ce7-905f-a2a07e804e43"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2abfb5ed-8597-45f4-9a02-02050d58ed9d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BaseAttack"",
                    ""type"": ""Button"",
                    ""id"": ""b88dc54c-3a44-4112-8d72-ebf876f33c2a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpecialAttack"",
                    ""type"": ""Button"",
                    ""id"": ""af7b0142-5c0d-4504-b6e7-aa119d745cfc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Grab"",
                    ""type"": ""Button"",
                    ""id"": ""b2061972-e974-4c74-9d41-fd83d565998a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TargetingCancelled"",
                    ""type"": ""Button"",
                    ""id"": ""88afca7e-aac9-40a4-bab3-ae359d395e53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                },
                {
                    ""name"": ""TargetingStart"",
                    ""type"": ""Button"",
                    ""id"": ""da7f6d31-8162-41ba-85e1-a92dea14a5bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""8b3cf89a-f296-4595-b99e-c6eb541d1bde"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ControllerAim"",
                    ""type"": ""Value"",
                    ""id"": ""fdaba3ac-9363-44f3-8437-b0210c6103c9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SprintActivator"",
                    ""type"": ""Value"",
                    ""id"": ""2d06bad0-f504-453b-b28d-9c3834b9ea01"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ReleaseGrab"",
                    ""type"": ""Button"",
                    ""id"": ""828bdc36-47fd-4292-bd3c-bdc593820771"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ClimbUp"",
                    ""type"": ""Button"",
                    ""id"": ""2d4e06e0-7e22-476b-9c3c-d9d31fd6d6de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetLevel"",
                    ""type"": ""Button"",
                    ""id"": ""d8bb6fcd-ba14-42b1-9365-d2d230139ce9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""acab2140-fb25-478b-86b6-21a82c6a1714"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bf194996-23e9-460a-9229-faa93517944b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c780e8f2-34e9-4b06-ad26-21512bb08c8f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fe7cf668-6765-4986-b8a5-f57d96bb0850"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ab79de6e-5ae9-4536-9a80-c83d4e678668"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4929abe0-1aa0-4e82-9de0-bc1d7eabca6c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe210168-3fbc-40c4-86ad-12193597dc5e"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""BaseAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b6fc99b-796c-4137-8386-aa75021b3f1e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""BaseAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0446160b-a153-484d-8b37-98c166fef730"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6f9be2c-ba50-4f97-867a-10554233a75c"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""SpecialAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6067444c-823b-4e2b-abf0-c43e2478281b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SpecialAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3f0fb89a-6060-49c1-8b9a-e495542318aa"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b8d7264-2c95-4588-94a1-073fb54dcdaa"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7fc8a26-53ff-48ba-9f2a-6e65bbfea47a"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""TargetingStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a02e0ef-f57a-4c2b-bd3d-57adaff6f539"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""TargetingStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f21ed4e-70bf-460a-a682-9606b4715792"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9b3dbe4-48e9-47b8-855c-e3be6a021679"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d593c9e9-ec9e-49dc-89e3-c714797bb439"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControllerAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4eafdca4-d6a5-4d7d-9442-2aee8595846b"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetingCancelled"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cff842ff-a15b-4ec6-94e7-dfd40acc6e8f"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TargetingCancelled"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""747b5981-c221-48af-ae2f-47bdc2e57ca6"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""GenericGamepad"",
                    ""action"": ""SprintActivator"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf9ae43f-3582-42ba-aa98-434cae9e1c6f"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SprintActivator"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a65d45f5-e609-4e0b-8983-8274c562f637"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86bde9a9-3338-4829-a564-4859c225ba07"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b07451a-a0ce-45d1-9cbd-7ff964f1ded6"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5e53462-0a58-4ce3-84ab-8c3968e3a6dc"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClimbUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94c3d413-09e5-4f02-b438-4a6988335d70"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""994b5e89-7d7f-47e9-b2f7-30b2608a9995"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3322ab1e-e150-4161-a471-1d8737dba0d2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ReleaseGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9add430f-7137-428d-a628-b8b9e88e02f8"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClimbUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bddd21af-c76a-4bc3-96dd-8b9c1c56f04a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ClimbUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c20afde4-c067-42ef-9e6c-04e94c6b3d5f"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetLevel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f64ed108-a492-4220-9dd2-e931e31130d0"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetLevel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""New action map"",
            ""id"": ""fef8319e-621c-4370-ae50-6883321f99f9"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""396561e0-12a9-4085-a8a7-e3e0cb592048"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4545ea88-40df-4187-9b47-a39922b67037"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""GenericGamepad"",
            ""bindingGroup"": ""GenericGamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerControls
        m_PlayerControls = asset.FindActionMap("PlayerControls", throwIfNotFound: true);
        m_PlayerControls_Move = m_PlayerControls.FindAction("Move", throwIfNotFound: true);
        m_PlayerControls_Jump = m_PlayerControls.FindAction("Jump", throwIfNotFound: true);
        m_PlayerControls_BaseAttack = m_PlayerControls.FindAction("BaseAttack", throwIfNotFound: true);
        m_PlayerControls_SpecialAttack = m_PlayerControls.FindAction("SpecialAttack", throwIfNotFound: true);
        m_PlayerControls_Grab = m_PlayerControls.FindAction("Grab", throwIfNotFound: true);
        m_PlayerControls_TargetingCancelled = m_PlayerControls.FindAction("TargetingCancelled", throwIfNotFound: true);
        m_PlayerControls_TargetingStart = m_PlayerControls.FindAction("TargetingStart", throwIfNotFound: true);
        m_PlayerControls_MousePosition = m_PlayerControls.FindAction("MousePosition", throwIfNotFound: true);
        m_PlayerControls_ControllerAim = m_PlayerControls.FindAction("ControllerAim", throwIfNotFound: true);
        m_PlayerControls_SprintActivator = m_PlayerControls.FindAction("SprintActivator", throwIfNotFound: true);
        m_PlayerControls_ReleaseGrab = m_PlayerControls.FindAction("ReleaseGrab", throwIfNotFound: true);
        m_PlayerControls_ClimbUp = m_PlayerControls.FindAction("ClimbUp", throwIfNotFound: true);
        m_PlayerControls_ResetLevel = m_PlayerControls.FindAction("ResetLevel", throwIfNotFound: true);
        // New action map
        m_Newactionmap = asset.FindActionMap("New action map", throwIfNotFound: true);
        m_Newactionmap_Newaction = m_Newactionmap.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerControls
    private readonly InputActionMap m_PlayerControls;
    private IPlayerControlsActions m_PlayerControlsActionsCallbackInterface;
    private readonly InputAction m_PlayerControls_Move;
    private readonly InputAction m_PlayerControls_Jump;
    private readonly InputAction m_PlayerControls_BaseAttack;
    private readonly InputAction m_PlayerControls_SpecialAttack;
    private readonly InputAction m_PlayerControls_Grab;
    private readonly InputAction m_PlayerControls_TargetingCancelled;
    private readonly InputAction m_PlayerControls_TargetingStart;
    private readonly InputAction m_PlayerControls_MousePosition;
    private readonly InputAction m_PlayerControls_ControllerAim;
    private readonly InputAction m_PlayerControls_SprintActivator;
    private readonly InputAction m_PlayerControls_ReleaseGrab;
    private readonly InputAction m_PlayerControls_ClimbUp;
    private readonly InputAction m_PlayerControls_ResetLevel;
    public struct PlayerControlsActions
    {
        private @PlayerInputMap m_Wrapper;
        public PlayerControlsActions(@PlayerInputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerControls_Move;
        public InputAction @Jump => m_Wrapper.m_PlayerControls_Jump;
        public InputAction @BaseAttack => m_Wrapper.m_PlayerControls_BaseAttack;
        public InputAction @SpecialAttack => m_Wrapper.m_PlayerControls_SpecialAttack;
        public InputAction @Grab => m_Wrapper.m_PlayerControls_Grab;
        public InputAction @TargetingCancelled => m_Wrapper.m_PlayerControls_TargetingCancelled;
        public InputAction @TargetingStart => m_Wrapper.m_PlayerControls_TargetingStart;
        public InputAction @MousePosition => m_Wrapper.m_PlayerControls_MousePosition;
        public InputAction @ControllerAim => m_Wrapper.m_PlayerControls_ControllerAim;
        public InputAction @SprintActivator => m_Wrapper.m_PlayerControls_SprintActivator;
        public InputAction @ReleaseGrab => m_Wrapper.m_PlayerControls_ReleaseGrab;
        public InputAction @ClimbUp => m_Wrapper.m_PlayerControls_ClimbUp;
        public InputAction @ResetLevel => m_Wrapper.m_PlayerControls_ResetLevel;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControlsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControlsActions instance)
        {
            if (m_Wrapper.m_PlayerControlsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                @BaseAttack.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBaseAttack;
                @BaseAttack.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBaseAttack;
                @BaseAttack.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBaseAttack;
                @SpecialAttack.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSpecialAttack;
                @SpecialAttack.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSpecialAttack;
                @SpecialAttack.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSpecialAttack;
                @Grab.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnGrab;
                @Grab.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnGrab;
                @Grab.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnGrab;
                @TargetingCancelled.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingCancelled;
                @TargetingCancelled.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingCancelled;
                @TargetingCancelled.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingCancelled;
                @TargetingStart.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingStart;
                @TargetingStart.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingStart;
                @TargetingStart.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnTargetingStart;
                @MousePosition.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMousePosition;
                @ControllerAim.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnControllerAim;
                @ControllerAim.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnControllerAim;
                @ControllerAim.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnControllerAim;
                @SprintActivator.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSprintActivator;
                @SprintActivator.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSprintActivator;
                @SprintActivator.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnSprintActivator;
                @ReleaseGrab.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnReleaseGrab;
                @ReleaseGrab.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnReleaseGrab;
                @ReleaseGrab.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnReleaseGrab;
                @ClimbUp.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnClimbUp;
                @ClimbUp.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnClimbUp;
                @ClimbUp.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnClimbUp;
                @ResetLevel.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnResetLevel;
                @ResetLevel.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnResetLevel;
                @ResetLevel.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnResetLevel;
            }
            m_Wrapper.m_PlayerControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @BaseAttack.started += instance.OnBaseAttack;
                @BaseAttack.performed += instance.OnBaseAttack;
                @BaseAttack.canceled += instance.OnBaseAttack;
                @SpecialAttack.started += instance.OnSpecialAttack;
                @SpecialAttack.performed += instance.OnSpecialAttack;
                @SpecialAttack.canceled += instance.OnSpecialAttack;
                @Grab.started += instance.OnGrab;
                @Grab.performed += instance.OnGrab;
                @Grab.canceled += instance.OnGrab;
                @TargetingCancelled.started += instance.OnTargetingCancelled;
                @TargetingCancelled.performed += instance.OnTargetingCancelled;
                @TargetingCancelled.canceled += instance.OnTargetingCancelled;
                @TargetingStart.started += instance.OnTargetingStart;
                @TargetingStart.performed += instance.OnTargetingStart;
                @TargetingStart.canceled += instance.OnTargetingStart;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @ControllerAim.started += instance.OnControllerAim;
                @ControllerAim.performed += instance.OnControllerAim;
                @ControllerAim.canceled += instance.OnControllerAim;
                @SprintActivator.started += instance.OnSprintActivator;
                @SprintActivator.performed += instance.OnSprintActivator;
                @SprintActivator.canceled += instance.OnSprintActivator;
                @ReleaseGrab.started += instance.OnReleaseGrab;
                @ReleaseGrab.performed += instance.OnReleaseGrab;
                @ReleaseGrab.canceled += instance.OnReleaseGrab;
                @ClimbUp.started += instance.OnClimbUp;
                @ClimbUp.performed += instance.OnClimbUp;
                @ClimbUp.canceled += instance.OnClimbUp;
                @ResetLevel.started += instance.OnResetLevel;
                @ResetLevel.performed += instance.OnResetLevel;
                @ResetLevel.canceled += instance.OnResetLevel;
            }
        }
    }
    public PlayerControlsActions @PlayerControls => new PlayerControlsActions(this);

    // New action map
    private readonly InputActionMap m_Newactionmap;
    private INewactionmapActions m_NewactionmapActionsCallbackInterface;
    private readonly InputAction m_Newactionmap_Newaction;
    public struct NewactionmapActions
    {
        private @PlayerInputMap m_Wrapper;
        public NewactionmapActions(@PlayerInputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Newactionmap_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Newactionmap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NewactionmapActions set) { return set.Get(); }
        public void SetCallbacks(INewactionmapActions instance)
        {
            if (m_Wrapper.m_NewactionmapActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_NewactionmapActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_NewactionmapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public NewactionmapActions @Newactionmap => new NewactionmapActions(this);
    private int m_GenericGamepadSchemeIndex = -1;
    public InputControlScheme GenericGamepadScheme
    {
        get
        {
            if (m_GenericGamepadSchemeIndex == -1) m_GenericGamepadSchemeIndex = asset.FindControlSchemeIndex("GenericGamepad");
            return asset.controlSchemes[m_GenericGamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerControlsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnBaseAttack(InputAction.CallbackContext context);
        void OnSpecialAttack(InputAction.CallbackContext context);
        void OnGrab(InputAction.CallbackContext context);
        void OnTargetingCancelled(InputAction.CallbackContext context);
        void OnTargetingStart(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnControllerAim(InputAction.CallbackContext context);
        void OnSprintActivator(InputAction.CallbackContext context);
        void OnReleaseGrab(InputAction.CallbackContext context);
        void OnClimbUp(InputAction.CallbackContext context);
        void OnResetLevel(InputAction.CallbackContext context);
    }
    public interface INewactionmapActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}