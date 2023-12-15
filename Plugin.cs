using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using Settings = SettingsAPI.Plugin;
using SettingsAPI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;
using Devdog.General.UI;

namespace StationTeleport
{
    [BepInPlugin("vaproxy.station.teleport", "StationTeleport", "1.0.1")]
    public class Plugin: BaseUnityPlugin
    {
        public static ManualLogSource Log;
        bool initialized = false;
        bool registered = false;
        GameObject MonoBehaviourObj;

        internal void Awake()
        {
            Log = Logger;
            Log.LogInfo("StationTeleport awake.");
        }

        internal void SceneCallback(Scene old, Scene newS)
            {
            if (newS.name != "Intro" && newS.name != "Menu")
            {
                MonoBehaviourObj = new GameObject("MonoBehaviourTemp");
                DontDestroyOnLoad(MonoBehaviourObj);
                MonoBehaviourObj.AddComponent<behaviour>().StartCoroutine(InitCoroutine());
                SceneManager.activeSceneChanged -= SceneCallback;
            }
        }

        internal void OnDestroy()
        {

            SceneManager.activeSceneChanged += SceneCallback;
        }

        internal IEnumerator InitCoroutine()
        {
            while (GameObject.FindObjectsOfType<Station>().Length < 10)
            {
                Log.LogInfo("Waiting for DATA.");
                yield return null;
            }
            Init();
        }

        internal void Init()
        {
            if (!initialized && !registered)
            {
                Log.LogInfo("Initializing StationTeleport.");
                initialized = true;
                /*GameObject game = new GameObject("StationTeleport");
                DontDestroyOnLoad(game);
                game.AddComponent<Teleporter>();*/
                Option[] Options = Array.Empty<Option>();
                foreach (Station station in GameObject.FindObjectsOfType<Station>())
                {
                    Option option = new Option
                    {
                        Create = (GameObject page) =>
                        {
                            GameObject button = ComponentUtils.CreateButton(station.name, $"stationteleport.{station.name}");
                            Button b = button.GetComponent<Button>();
                            b.onClick.AddListener(() =>
                            {
                                Log.LogInfo($"Teleporting to {station.name}");
                                station.Context.Invoke();
                                GameObject.FindObjectOfType<Inventory>().transform.position = station.spawn.position;
                            });
                            Text text = button.Find("ItemName").GetComponent<Text>();
                            text.resizeTextForBestFit = true;
                            text.verticalOverflow = VerticalWrapMode.Overflow;
                            text.horizontalOverflow = HorizontalWrapMode.Overflow;
                            LayoutElement elem = button.AddComponent<LayoutElement>();
                            elem.minWidth = 50f;
                            elem.minWidth = 50f;
                            button.SetParent(page.Find("Viewport/Content"), false);
                        }
                    };
                    Options = Options.Append(option);
                }
                void CreationCallback(GameObject page)
                {
                    GameObject ScrollbarVertical = GameObject.Find("MAINMENU/Canvas/Pages/Inventory/Content/__INVENTORY_CONTAINER/Container/InventorySlots/Scrollbar Vertical");
                    GameObject ScrollVertical = ScrollbarVertical.Instantiate();
                    ScrollVertical.name = "Scrollbar Vertical";
                    ScrollVertical.SetParent(page, false);
                    ScrollVertical.GetComponent<RectTransform>().anchoredPosition = new Vector2(483.2355f, -113.0717f);
                    Scrollbar bar = ScrollVertical.GetComponent<Scrollbar>();
                    bar.direction = Scrollbar.Direction.TopToBottom;
                    bar.interactable = true;
                    bar.navigation = Navigation.defaultNavigation;
                    bar.useGUILayout = true;
                    ScrollVertical.GetComponent<RectTransform>().localScale = new Vector3(1, 5, 1);
                    GameObject Viewport = new GameObject("Viewport");
                    RectTransform ViewportRect = Viewport.AddComponent<RectTransform>();
                    Viewport.AddComponent<CanvasRenderer>();
                    Viewport.AddComponent<Animator>();
                    CanvasGroup canvas = Viewport.AddComponent<CanvasGroup>();
                    canvas.blocksRaycasts = true;
                    canvas.interactable = true;
                    Viewport.AddComponent<Mask>();
                    Viewport.AddComponent<AnimatorHelper>();
                    Viewport.SetParent(page, false);
                    ViewportRect.anchoredPosition = new Vector2(73.3544f, -190.1612f);
                    ViewportRect.sizeDelta = new Vector2(1000, 700);
                    ScrollRect scroll = page.AddComponent<ScrollRect>();
                    scroll.inertia = true;
                    scroll.horizontal = false;
                    scroll.decelerationRate = 0.135f;
                    scroll.elasticity = 0.1f;
                    scroll.movementType = ScrollRect.MovementType.Elastic;
                    scroll.scrollSensitivity = 25;
                    scroll.vertical = true;
                    scroll.verticalScrollbar = bar;
                    scroll.viewport = ViewportRect;
                    scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                    GameObject Content = Viewport.AddObject("Content");
                    RectTransform contentTransform = Content.AddComponent<RectTransform>();
                    contentTransform.anchoredPosition = new Vector2(-11.8327f, 0.0009f);
                    contentTransform.sizeDelta = new Vector2(900, 600);
                    scroll.content = contentTransform;
                    GridLayoutGroup group = Content.AddComponent<GridLayoutGroup>();
                    group.childAlignment = TextAnchor.UpperLeft;
                    group.spacing = new Vector2(80, 20);
                    group.cellSize = new Vector2(350, 50);
                    group.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    group.startAxis = GridLayoutGroup.Axis.Horizontal;
                    group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    group.constraintCount = 2;
                }
                Log.LogInfo("Registering StationTeleport.");
                Settings.API.RegisterMod("tairasoul.station.teleports", "StationTeleports", Options, CreationCallback);
                registered = true;
                Log.LogInfo("StationTeleport initialized.");
                Destroy(MonoBehaviourObj);
            }
        }
    }
}
