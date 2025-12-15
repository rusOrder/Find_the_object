using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Настройки")]
    [SerializeField] private LevelConfig levelConfig;

    [Header("UI Компоненты")]
    [SerializeField] private RectTransform gameArea;
    [SerializeField] private GameUI gameUI;

    [Header("Префабы")]
    [SerializeField] private GameObject uiItemPrefab;
    [SerializeField] private UIItemPlacer uiItemPlacerPrefab;
    [SerializeField] private GameController gameControllerPrefab;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<GameInitializedSignal>();

        Container.DeclareSignal<CurrentItemsChangedSignal>();

        if (levelConfig == null)
        {
            Debug.LogError("LevelConfig is not assigned!");
        }
        else
        {
            Container.Bind<LevelConfig>().FromInstance(levelConfig).AsSingle();
            Debug.Log("LevelConfig bound");
        }

        Container.BindInterfacesAndSelfTo<ItemManager>().AsSingle();

        Debug.Log("ItemManager bound");

        Container.BindInterfacesAndSelfTo<GameTimer>().AsSingle();

        Debug.Log("GameTimer bound");

        if (uiItemPlacerPrefab != null && gameArea != null)
        {
            var placer = Container.InstantiatePrefabForComponent<UIItemPlacer>(uiItemPlacerPrefab, gameArea);
            Container.BindInterfacesAndSelfTo<UIItemPlacer>().FromInstance(placer).AsSingle();

            Debug.Log("UIItemPlacer bound with interfaces");
        }
        else
        {
            Debug.LogWarning("UIItemPlacer prefab or GameArea not assigned");
        }

        if (uiItemPrefab != null)
        {
            Container.Bind<GameObject>().WithId("UIItemPrefab").FromInstance(uiItemPrefab);
            Debug.Log("UIItem prefab bound");
        }

        if (gameArea != null)
        {
            Container.Bind<RectTransform>().WithId("GameArea").FromInstance(gameArea);
            Debug.Log("GameArea bound");
        }

        if (gameUI != null)
        {
            Container.Bind<GameUI>().FromInstance(gameUI).AsSingle();
            Debug.Log("GameUI bound");
        }
        else
        {
            Debug.LogError("GameUI is not assigned!");
        }

        if (gameControllerPrefab != null)
        {
            var controller = Container.InstantiatePrefabForComponent<GameController>(
                gameControllerPrefab
            );
            Container.Bind<GameController>().FromInstance(controller).AsSingle();
            Debug.Log("GameController created from prefab");
        }
        else
        {
            Container.Bind<GameController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Debug.Log("GameController created as new GameObject");
        }

        Debug.Log("All Bindings Installed");
    }
}