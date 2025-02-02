using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private float walkspeedMult = 1f;

        private bool hasInited = false;

        public void changeWalkSpeed(float newWS)
        {
            walkspeedMult = newWS;
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }

        void initVariables()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (!hasInited)
            {    
                initVariables();
                hasInited = true;
            }
      
            Vector3 m_MoveBack = Vector3.zero;

            // read inputs
            float hX = CrossPlatformInputManager.GetAxis("Horizontal");
            float vX = CrossPlatformInputManager.GetAxis("Vertical");

            int v = CrossPlatformInputManager.GetButton("Vertical") ? 1 : 0;
            int h = CrossPlatformInputManager.GetButton("Horizontal") ? 1 : 0;

            h = hX < 0 ? -1*h : h;
            v = vX < 0 ? -1*v : v;

            bool stoppedMov = false;

            if (h == 0 && v == 0)
                stoppedMov = true;

            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
                if (v < 0)
                {
                    m_MoveBack = -1 * v * m_CamForward + -1 * h * m_Cam.right;
                }
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }

            //m_Move *= walkspeedMult;
            //m_MoveBack *= walkspeedMult;

            if (stoppedMov)
            {
                m_Move = m_CamForward;
            }

#if !MOBILE_INPUT

            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, walkspeedMult, crouch, m_Jump, stoppedMov, m_MoveBack, v < 0);
            m_Jump = false;
        }
    }
}
