using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RenderTextureRaycast : MonoBehaviour
{
    public Camera portalCamera;
    [SerializeField]  
    GraphicRaycaster m_Raycaster;

    void Update () 
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         
            // do we hit our portal plane?
            if (Physics.Raycast(ray, out hit)) 
            {
                Debug.Log(hit.collider.gameObject);
                var localPoint = hit.textureCoord;
                // convert the hit texture coordinates into camera coordinates
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = new Vector2(localPoint.x * portalCamera.pixelWidth, localPoint.y * portalCamera.pixelHeight);;
                List<RaycastResult> results = new List<RaycastResult>();
                m_Raycaster.Raycast(pointerData, results);
                // the portal ray for camera
                if (results.Count > 0) {
                    foreach (RaycastResult r in results)
                    {
                        if (r.gameObject.CompareTag("UIButton"))
                        {
                            // You can't click on UI
                            var button = r.gameObject.GetComponent<Button>();
                            // invoke the button component
                            button.onClick.Invoke();
                        }
                    }
                }
            }
        }
     
    }
}