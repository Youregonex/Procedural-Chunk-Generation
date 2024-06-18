using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode
    {
        NoiseMap,
        ColorMap
    }

    [Header("Noise Settings")]
    [SerializeField] private DrawMode _drawMode;
    [Range(1, 200)]
    [SerializeField] private int _mapWidth;
    [Range(1, 200)]
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves;
    [SerializeField] private Noise.NormalizeMode _normalizeMode;

    [Range(0, 1)]
    [SerializeField] private float _persistance;

    [SerializeField] private float _lacunarity;
    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private TerrainTypeStruct[] _regions;
    [field: SerializeField] public bool autoUpdate { get; private set; }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset, _normalizeMode);

        Color[] colorMap = new Color[_mapWidth * _mapHeight];

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < _regions.Length; i++)
                {
                    if(currentHeight <= _regions[i].height)
                    {
                        colorMap[y * _mapWidth + x] = _regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if(_drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));

        if(_drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, _mapWidth, _mapHeight));

    }

    private void OnValidate()
    {
        if (_lacunarity < 1)
            _lacunarity = 1;

        if (_octaves < 0)
            _octaves = 0;
    }
}

[System.Serializable]
public struct TerrainTypeStruct
{
    public string name;
    public float height;
    public Color color;
}