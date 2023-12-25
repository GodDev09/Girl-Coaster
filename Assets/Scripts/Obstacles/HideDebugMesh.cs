using UnityEngine;

public class HideDebugMesh : MonoBehaviour
{
	[Header("Only set this on one scene object and it wil work on all debug objects"), SerializeField]
	private Material clearMaterial;

	private static Material _sharedClearMaterial;

	private void Awake()
	{
		if(!_sharedClearMaterial && clearMaterial)
			_sharedClearMaterial = clearMaterial;
	}

	private void Start() => GetComponent<Renderer>().material = _sharedClearMaterial;
}