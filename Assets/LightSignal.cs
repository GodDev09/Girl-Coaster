using UnityEngine;

public class LightSignal : MonoBehaviour
{
    [SerializeField] private new Renderer renderer;
    	
    	public void ChangeColor(Color color)
        {
        	 renderer.material.color = color;
        }
}
