using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScaleShowcase
{
    public class ScaleShowcase : ModBehaviour
    {
        public static ScaleShowcase Instance;
        public static GameObject EyePrefab = null;
        public static bool IsInitialized = false;

        public static INewHorizons NewHorizonsAPI;

        private const string ScaleSystemName = "xen.ScaleShowcase";

        public static Light freeCamLight;

        // Planets
        public static GameObject anglerfish;
        public static GameObject ashTwin;
        public static GameObject attlerock;
        public static GameObject brittleHollow;
        public static GameObject darkBramble;
        public static GameObject emberTwin;
        public static GameObject eye;
        public static GameObject giantsDeep;
        public static GameObject hollowsLantern;
        public static GameObject ringedPlanet;
        public static GameObject nomaiProbe;
        public static GameObject orbitalProbeCannon;
        public static GameObject quantumMoon;
        public static GameObject signalJammer;
        public static GameObject stranger;
        public static GameObject sun;
        public static GameObject sunStation;
        public static GameObject shuttle;
        public static GameObject timberHearth;
        public static GameObject vessel;
        public static GameObject whiteHole;
        public static GameObject whiteHoleStation;


        private void Start()
        {
            Instance = this;
            Log($"{nameof(ScaleShowcase)} is installed");

            // Load NH stuff
            NewHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizonsAPI.LoadConfigs(this);
            NewHorizonsAPI.GetStarSystemLoadedEvent().AddListener(OnLoadStarSystem);

            // Events
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            // We're on the title scene actually
            OnCompleteSceneLoad(OWScene.None, OWScene.TitleScreen);
        }

        private void OnCompleteSceneLoad(OWScene scene, OWScene loadScene)
        {
            Log($"Loading into {loadScene}");

            if(loadScene == OWScene.TitleScreen && !IsInitialized)
            {
                Log("Going to grab the eye.");
                LoadManager.LoadSceneAsync(OWScene.EyeOfTheUniverse, true, LoadManager.FadeType.ToBlack, 0.1f, true);
            }

            if(loadScene == OWScene.EyeOfTheUniverse && !IsInitialized)
            {
                Log("Grabbing the eye");
                IsInitialized = true;
                EyePrefab = GameObject.Instantiate(GameObject.Find("EyeOfTheUniverse_Body"));
                EyePrefab.SetActive(false);
                DontDestroyOnLoad(EyePrefab);
                LoadManager.LoadSceneAsync(OWScene.TitleScreen, true, LoadManager.FadeType.ToBlack, 0.1f, true);
            }

            if(loadScene == OWScene.SolarSystem && NewHorizonsAPI.GetCurrentStarSystem() == ScaleSystemName)
            {
                Log("Entering the scale showcase system");

                // Get the Stranger to work
                var streamingGroup = GameObject.Find("RingWorld_Body/StreamingGroup_RW").GetComponent<StreamingGroup>();
                streamingGroup.LoadGeneralAssets();
                streamingGroup.LoadRequiredAssets();
                streamingGroup.LoadRequiredColliders();
                streamingGroup._locked = true;
            }
        }

        // Happens once NH is done doing stuff
        private void OnLoadStarSystem(string starSystem)
        {
            if (starSystem != ScaleSystemName) return;

            Log("Scale showcase system should be done loading");

            ModHelper.Events.Unity.FireInNUpdates(OnNHComplete, 10); 
        }

        public static void OnNHComplete()
        {
            Log("NH is done loading");

            // Now we get all the planets
            anglerfish = NewHorizonsAPI.GetPlanet("Anglerfish Copy");
            ashTwin = NewHorizonsAPI.GetPlanet("Ash Twin Copy");
            attlerock = NewHorizonsAPI.GetPlanet("Attlerock Copy");
            brittleHollow = NewHorizonsAPI.GetPlanet("Brittle Hollow Copy");
            darkBramble = NewHorizonsAPI.GetPlanet("Dark Bramble Copy");
            emberTwin = NewHorizonsAPI.GetPlanet("Ember Twin Copy");
            eye = NewHorizonsAPI.GetPlanet("Eye Copy");
            giantsDeep = NewHorizonsAPI.GetPlanet("Giant's Deep Copy");
            hollowsLantern = NewHorizonsAPI.GetPlanet("Hollow's Lantern Copy");
            ringedPlanet = NewHorizonsAPI.GetPlanet("Ringed Planet Copy");
            nomaiProbe = NewHorizonsAPI.GetPlanet("Nomai Probe Copy");
            orbitalProbeCannon = NewHorizonsAPI.GetPlanet("Orbital Probe Cannon Copy");
            quantumMoon = NewHorizonsAPI.GetPlanet("Quantum Moon Copy");
            signalJammer = NewHorizonsAPI.GetPlanet("Signal Jammer Copy");
            stranger = NewHorizonsAPI.GetPlanet("Stranger Copy");
            sun = NewHorizonsAPI.GetPlanet("The Sun");
            sunStation = NewHorizonsAPI.GetPlanet("Sun Station Copy");
            shuttle = NewHorizonsAPI.GetPlanet("Shuttle Copy");
            timberHearth = NewHorizonsAPI.GetPlanet("Timber Hearth Copy");
            vessel = NewHorizonsAPI.GetPlanet("Vessel Copy");
            whiteHole = NewHorizonsAPI.GetPlanet("White Hole Copy");
            whiteHoleStation = NewHorizonsAPI.GetPlanet("White Hole Station Copy");

            // Add in the eye
            var eyeSector = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/Sector_EyeSurface").gameObject);
            eyeSector.transform.parent = eye.transform;
            eyeSector.transform.localPosition = Vector3.zero;
            eyeSector.SetActive(true);

            var eyeSurface = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/Proxy_SixthPlanet").gameObject);
            eyeSurface.transform.parent = eye.transform;
            eyeSurface.transform.localPosition = Vector3.zero;
            eyeSurface.SetActive(true);
            eyeSurface.transform.Find("Effects_EYE_Symbol").gameObject.SetActive(false);

            var jammerGeo = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/SignalJammer_Pivot/SignalJammer_Root").gameObject);
            jammerGeo.transform.parent = signalJammer.transform;
            jammerGeo.transform.localPosition = Vector3.zero;
            jammerGeo.SetActive(true);

            foreach (var cloakProxy in stranger.GetComponentsInChildren<CloakingFieldProxy>())
            {
                cloakProxy.OnPlayerEnterCloakingField();
            }

            // Ringed planet 
            foreach (var renderer in ringedPlanet.transform.Find("Sector/Prefab_IP_VisiblePlanet(Clone)/VisiblePlanet_Pivot").GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = true;
            }

            // Try fixing freecam while we're at it
            if(Instance.ModHelper.Interaction.ModExists("misternebula.FreeCam"))
            {
                Log("Freecam is installed");
                Instance.ModHelper.Events.Unity.RunWhen(() => GameObject.Find("FREECAM") != null, () => {
                    var freecam = GameObject.Find("FREECAM");
                    freecam.transform.parent = Locator.GetPlayerTransform();
                    freeCamLight = freecam.AddComponent<Light>();
                    freeCamLight.range = 5000f;
                    freeCamLight.enabled = false;
                });
            }

            Log("Scale Showcase finished setting up");
        }

        public static void Log(string message, MessageType type = MessageType.Info)
        {
            Instance.ModHelper.Console.WriteLine(message, type);
        }

        void Update()
        {
            if(freeCamLight != null && Keyboard.current[Key.F].wasPressedThisFrame)
            {
                freeCamLight.enabled = !freeCamLight.enabled;
            }

            // Fix some rotations but CR doesn't want us to
            if(darkBramble.transform.rotation != Quaternion.identity)
            {
                darkBramble.transform.rotation = Quaternion.identity;
                vessel.transform.rotation = Quaternion.identity;
                orbitalProbeCannon.transform.rotation = Quaternion.Euler(0, 45, 0);
                giantsDeep.transform.rotation = Quaternion.identity;
                sunStation.transform.rotation = Quaternion.identity;
                quantumMoon.transform.rotation = Quaternion.identity;
                anglerfish.transform.rotation = Quaternion.Euler(0, 135, 0);
                ringedPlanet.transform.rotation = Quaternion.Euler(0, 275, 45);
                shuttle.transform.rotation = Quaternion.identity;
                signalJammer.transform.rotation = Quaternion.Euler(0, 45, 0);
            }
        }
    }
}
