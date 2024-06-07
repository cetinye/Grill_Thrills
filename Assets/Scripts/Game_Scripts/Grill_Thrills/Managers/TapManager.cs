using UnityEngine;

namespace Grill_Thrills
{
    public class TapManager : MonoBehaviour
    {
        void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray rayCam = LevelManager.instance.mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hitCam;

                if (Physics.Raycast(rayCam, out hitCam) && hitCam.collider.TryGetComponent<IFood>(out IFood _food))
                {
                    _food.Tapped();
                }
            }
        }
    }
}