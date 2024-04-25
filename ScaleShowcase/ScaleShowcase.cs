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
        public static GameObject backerSatellite;
        public static GameObject brittleHollow;
        public static GameObject darkBramble;
        public static GameObject emberTwin;
        public static GameObject eye;
        public static GameObject giantsDeep;
        public static GameObject hollowsLantern;
        public static GameObject iceShuttle;
        public static GameObject ringedPlanet;
        public static GameObject mapSatellite;
        public static GameObject nomaiProbe;
        public static GameObject orbitalProbeCannon;
        public static GameObject quantumMoon;
        public static GameObject signalJammer;
        public static GameObject skyShutterSatellite;
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
            NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
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

            if (loadScene == OWScene.TitleScreen && !IsInitialized)
            {
                Log("Going to grab the eye.");
                LoadManager.LoadSceneAsync(OWScene.EyeOfTheUniverse, true, LoadManager.FadeType.ToBlack, 0.1f, true);
            }

            if (loadScene == OWScene.EyeOfTheUniverse && !IsInitialized)
            {
                Log("Grabbing the eye");
                IsInitialized = true;
                EyePrefab = GameObject.Instantiate(GameObject.Find("EyeOfTheUniverse_Body"));
                EyePrefab.SetActive(false);
                DontDestroyOnLoad(EyePrefab);
                LoadManager.LoadSceneAsync(OWScene.TitleScreen, true, LoadManager.FadeType.ToBlack, 0.1f, true);
            }

            if (loadScene == OWScene.SolarSystem && NewHorizonsAPI.GetCurrentStarSystem() == ScaleSystemName)
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
            backerSatellite = NewHorizonsAPI.GetPlanet("Backer Satellite Copy");
            brittleHollow = NewHorizonsAPI.GetPlanet("Brittle Hollow Copy");
            darkBramble = NewHorizonsAPI.GetPlanet("Dark Bramble Copy");
            emberTwin = NewHorizonsAPI.GetPlanet("Ember Twin Copy");
            eye = NewHorizonsAPI.GetPlanet("Eye Copy");
            giantsDeep = NewHorizonsAPI.GetPlanet("Giant's Deep Copy");
            hollowsLantern = NewHorizonsAPI.GetPlanet("Hollow's Lantern Copy");
            iceShuttle = NewHorizonsAPI.GetPlanet("Ice Shuttle Copy");
            ringedPlanet = NewHorizonsAPI.GetPlanet("Ringed Planet Copy");
            mapSatellite = NewHorizonsAPI.GetPlanet("Map Satellite Copy");
            nomaiProbe = NewHorizonsAPI.GetPlanet("Nomai Probe Copy");
            orbitalProbeCannon = NewHorizonsAPI.GetPlanet("Orbital Probe Cannon Copy");
            quantumMoon = NewHorizonsAPI.GetPlanet("Quantum Moon Copy");
            signalJammer = NewHorizonsAPI.GetPlanet("Signal Jammer Copy");
            skyShutterSatellite = NewHorizonsAPI.GetPlanet("Sky Shutter Satellite Copy");
            stranger = NewHorizonsAPI.GetPlanet("Stranger Copy");
            sun = NewHorizonsAPI.GetPlanet("The Sun");
            sunStation = NewHorizonsAPI.GetPlanet("Sun Station Copy");
            shuttle = NewHorizonsAPI.GetPlanet("Shuttle Copy");
            timberHearth = NewHorizonsAPI.GetPlanet("Timber Hearth Copy");
            vessel = NewHorizonsAPI.GetPlanet("Vessel");
            whiteHole = NewHorizonsAPI.GetPlanet("White Hole Copy");
            whiteHoleStation = NewHorizonsAPI.GetPlanet("White Hole Station Copy");

            // Add in the eye
            var eyeSector = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/Sector_EyeSurface").gameObject);
            eyeSector.transform.parent = eye.transform;
            eyeSector.transform.localPosition = Vector3.zero;
            eyeSector.SetActive(true);
            foreach (var cull in eyeSector.GetComponentsInChildren<SectorCullGroup>(true)) GameObject.Destroy(cull);

            var eyeLighting = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/Lighting_SixthPlanet").gameObject);
            eyeLighting.transform.parent = eye.transform;
            eyeLighting.transform.localPosition = Vector3.zero;
            eyeLighting.SetActive(true);
            eyeLighting.transform.Find("Light_ChuteEntrance").gameObject.SetActive(false);
            foreach (var light in eyeLighting.GetComponentsInChildren<HeatLightningController>(true)) light.gameObject.SetActive(false);

            var eyeAmbient = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/AmbientLight_EYE").gameObject);
            eyeAmbient.transform.parent = eye.transform;
            eyeAmbient.transform.localPosition = Vector3.zero;
            eyeAmbient.SetActive(true);

            var qmAtmosphere = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/QuantumMoonProxy_Pivot/QuantumMoonProxy_Root/MoonState_Root/AtmoSphere").gameObject);
            qmAtmosphere.transform.parent = quantumMoon.transform;
            qmAtmosphere.transform.localPosition = Vector3.zero;
            qmAtmosphere.SetActive(true);

            var qmAmbientLight = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/QuantumMoonProxy_Pivot/QuantumMoonProxy_Root/MoonState_Root/AmbientLight_QM").gameObject);
            qmAmbientLight.transform.parent = quantumMoon.transform;
            qmAmbientLight.transform.localPosition = Vector3.zero;
            qmAmbientLight.SetActive(true);

            var jammerGeo = GameObject.Instantiate(EyePrefab.transform.Find("Sector_EyeOfTheUniverse/SixthPlanet_Root/SignalJammer_Pivot/SignalJammer_Root").gameObject);
            jammerGeo.transform.parent = signalJammer.transform;
            jammerGeo.transform.localPosition = Vector3.zero;
            jammerGeo.SetActive(true);

            foreach (var cloakProxy in stranger.GetComponentsInChildren<CloakingFieldProxy>())
            {
                cloakProxy.OnPlayerEnterCloakingField();
            }

            // Ringed planet 
            foreach (var renderer in ringedPlanet.transform.Find("Sector/Prefab_IP_VisiblePlanet/VisiblePlanet_Pivot/Rings_IP_VisiblePlanet").GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = true;
            }
            GameObject.DestroyImmediate(ringedPlanet.transform.Find("Sector/Prefab_IP_VisiblePlanet/VisiblePlanet_Pivot/Rings_IP_VisiblePlanet/OtherSide").GetComponent<RotateTransform>());
            ringedPlanet.transform.Find("Sector/Prefab_IP_VisiblePlanet/VisiblePlanet_Pivot/Rings_IP_VisiblePlanet/OtherSide").transform.localRotation = Quaternion.Euler(180, 0, 0);

            // Try fixing freecam while we're at it
            if (Instance.ModHelper.Interaction.ModExists("misternebula.FreeCam"))
            {
                Log("Freecam is installed");
                Instance.ModHelper.Events.Unity.RunWhen(() => GameObject.Find("FREECAM") != null, () =>
                {
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
            if (freeCamLight != null && Keyboard.current[Key.F].wasPressedThisFrame)
            {
                freeCamLight.enabled = !freeCamLight.enabled;
            }

            // Fix some rotations but CR doesn't want us to
            if (darkBramble != null && darkBramble.transform.rotation != Quaternion.identity)
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
