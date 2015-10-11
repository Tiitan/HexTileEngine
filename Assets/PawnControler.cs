using UnityEngine;

public class PawnControler : MonoBehaviour
{
    [SerializeField]
    private float _climbMaxHeight;

    [SerializeField]
    private float _fallMaxHeight;

    [SerializeField]
    private float _movementPoint;

    Vector2i?      _gridPosition;

    void Start ()
    {
        if (HexTerrain.Instance != null)
        {
            _gridPosition = HexTerrain.Instance.RegisterPawn(this);
            if (_gridPosition.HasValue)
            {
                Vector3 newPosition = HexagonUtils.ConvertHexaSpaceToOrthonormal(_gridPosition.Value);
                newPosition.y = HexTerrain.Instance.HexData[_gridPosition.Value].Height;
                transform.position = newPosition;
            }
        }
    }
	
	void Update ()
    {
	
	}
}
