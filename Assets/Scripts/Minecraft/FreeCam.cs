using UnityEngine;

namespace Minecraft {
    public class FreeCam : MonoBehaviour {
        [SerializeField]
        public float speed = 10.0f;
        [SerializeField]
        public float mouseSensitivity = 4.0f;
        private float rotX; // rotation around the right/x axis

        private float rotY; // rotation around the up/y axis

        private void Start() {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }

        private void Update() {
            if (Input.GetKey(KeyCode.Mouse1)) {
                rotY += Input.GetAxis("Mouse X") * mouseSensitivity;
                rotX += Input.GetAxis("Mouse Y") * mouseSensitivity;

                Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
                transform.rotation = localRotation;
            }
        }

        private void FixedUpdate() {
            float currentSpeed = speed;

            if (Input.GetKey(KeyCode.LeftShift)) {
                currentSpeed *= 2;
            }

            float translation = Input.GetAxis("Vertical") * currentSpeed;
            float straffe = Input.GetAxis("Horizontal") * currentSpeed;

            translation *= Time.deltaTime;
            straffe *= Time.deltaTime;

            transform.Translate(straffe, 0, translation);
        }
    }
}
