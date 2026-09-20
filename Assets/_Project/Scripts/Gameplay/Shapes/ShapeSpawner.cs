using System.Collections;
using ColorMatch.Data;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorMatch.Gameplay.Shapes
{
    public class ShapeSpawner : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private FallingShape[] prefabs;
        [SerializeField] private ShapePalette palette;

        [Header("Randomization")]
        [SerializeField] private Vector2 scaleRange = new Vector2(0.35f, 0.55f);
        [SerializeField] private Vector2 angularVelocityRange = new Vector2(-180f, 180f);

        [Header("Placement")]
        [SerializeField] private float spawnHeightPadding = 1f;
        [SerializeField] private float edgePadding = 0.4f;

        [Header("Pool")]
        [SerializeField] private int defaultCapacity = 8;
        [SerializeField] private int maxSize = 32;

        // Overwritten by Configure; the initial values only matter if no difficulty is applied.
        private Vector2 _spawnDelayRange = new Vector2(0.6f, 1.4f);
        private Vector2 _fallSpeedRange = new Vector2(2.5f, 3.5f);

        private ObjectPool<FallingShape>[] _pools;
        private Camera _mainCamera;
        private float _spawnY;
        private float _minX;
        private float _maxX;

        private void Awake()
        {
            _mainCamera = Camera.main;
            CacheBounds();
            CreatePools();
        }

        private void OnEnable() => StartCoroutine(SpawnLoop());

        public void Configure(DifficultySettings difficulty)
        {
            _spawnDelayRange = difficulty.SpawnDelayRange;
            _fallSpeedRange = difficulty.FallSpeedRange;
        }

        private void CacheBounds()
        {
            float camHalfHeight = _mainCamera.orthographicSize;
            float camHalfWidth = camHalfHeight * _mainCamera.aspect;
            Vector3 camPosition = _mainCamera.transform.position;

            _spawnY = camPosition.y + camHalfHeight + spawnHeightPadding;
            _minX = camPosition.x - camHalfWidth + edgePadding;
            _maxX = camPosition.x + camHalfWidth - edgePadding;
        }

        private void CreatePools()
        {
            _pools = new ObjectPool<FallingShape>[prefabs.Length];

            for (int i = 0; i < prefabs.Length; i++)
            {
                int poolIndex = i;
                FallingShape prefab = prefabs[i];

                _pools[i] = new ObjectPool<FallingShape>(
                    createFunc: () => CreateInstance(prefab, poolIndex),
                    actionOnRelease: shape => shape.gameObject.SetActive(false),
                    actionOnDestroy: shape => Destroy(shape.gameObject),
                    defaultCapacity: defaultCapacity,
                    maxSize: maxSize);
            }
        }

        private FallingShape CreateInstance(FallingShape prefab, int poolIndex)
        {
            FallingShape shape = Instantiate(prefab, transform);
            shape.gameObject.SetActive(false);
            shape.Released += released => _pools[poolIndex].Release(released);
            return shape;
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_spawnDelayRange.x, _spawnDelayRange.y));
                Spawn();
            }
        }

        private void Spawn()
        {
            ObjectPool<FallingShape> pool = _pools[Random.Range(0, _pools.Length)];
            FallingShape shape = pool.Get();

            int colorIndex = palette.RandomIndex();

            shape.Init(
                position: new Vector2(Random.Range(_minX, _maxX), _spawnY),
                colorIndex: colorIndex,
                color: palette.GetColor(colorIndex),
                scale: Random.Range(scaleRange.x, scaleRange.y),
                rotation: Random.Range(0f, 360f),
                fallSpeed: Random.Range(_fallSpeedRange.x, _fallSpeedRange.y),
                angularVelocity: Random.Range(angularVelocityRange.x, angularVelocityRange.y)
                );
        }
    }
}
