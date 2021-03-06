﻿using Assets.Scripts.Parallaxing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PerspectiveInitializer : MonoBehaviour
    {
        public enum ThemeState
        {
            Mine,
            Ice
        }

        public ThemeState themeState = ThemeState.Ice;

        public static PerspectiveInitializer s_Instance;

        private GameObject mainCharacter;
        private GameObject dragon;
        private GameObject normalPillar;
        private GameObject icePiller;
        private GameObject normalCrash;
        private GameObject iceCrash;
        private Vector3 pillarPosition = new Vector3(3f, 11f, -8f);

        public static void FinalizeObject()
        {
            s_Instance = null;
        }

        void Awake()
        {
            s_Instance = this;
        }

        public void Start()
        {
            mainCharacter = GameObject.Find("Main Character");
            dragon = GameObject.Find("Dragon");
            normalPillar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + "pillar-crash"), new Vector3(-30f, 11f, -8f), new Quaternion(0, 0, 0, 0));
            icePiller = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + "ice-pillar-crash"), new Vector3(-30f, 11f,-8f), new Quaternion(0, 0, 0, 0));
            normalCrash = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + "crash"), new Vector3(-12, 0, 0), new Quaternion(0, 0, 0, 0));
            iceCrash = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + "ice-crash"), new Vector3(-12, 0, 0), new Quaternion(0, 0, 0, 0));

            Invoke("repositionPillar", 30);
            Invoke("SwitchPerspective", 40.0f + (UnityEngine.Random.value * 20.0f));
        }

        private void repositionPillar()
        {
            normalPillar.transform.localPosition = pillarPosition;
            icePiller.transform.localPosition = pillarPosition;
        }

        public void InvokeMethod(string methodName, float delay)
        {
            Invoke(methodName, delay);
        }

        public void CleanPerspective()
        {
            //Cancel all invokes except for the ones that are used in changing perspective.
            foreach (MonoBehaviour behaviour in GameObject.FindObjectsOfType(typeof(MonoBehaviour)))
            {
                if (behaviour.name == "Fader" || behaviour.name == "PerspectiveInitializer" || behaviour.name == "Theme")
                {
                    continue;
                }

                behaviour.CancelInvoke();
            }
            
            SpawnManager.Instance.DisableAll();
        }

        public void LoadVerticalPerspective()
        {
            CleanPerspective(); //Another one just in case.
            Global.Instance.LoadVertical(themeState);
            mainCharacter.GetComponent<MainCharacter>().LoadVertical(themeState);
            dragon.GetComponent<Dragon>().LoadVertical(themeState);
            GameObject.Find("Background Manager").GetComponent<BackgroundManager>().InitVertical(themeState);
            GameObject.Find("Resource Manager").GetComponent<ResourceManager>().RemoveVehicle();
            SpawnManager.Instance.LoadVertical(themeState);
        }

        public void LoadHorizontalPerspective()
        {
            CleanPerspective(); //Another one just in case.
            Global.Instance.LoadHorizontal(themeState);
            mainCharacter.GetComponent<MainCharacter>().LoadHorizontal(themeState);
            dragon.GetComponent<Dragon>().LoadHorizontal(themeState);
            GameObject.Find("Background Manager").GetComponent<BackgroundManager>().InitHorizontal(themeState);
            SpawnManager.Instance.LoadHorizontal(themeState);
        }

        void disableGesture()
        {
            Global.Instance.Gesture.SetActive(false);
        }

        public void SwitchPerspective()
        {
            if (Global.Instance.orientation == 0)
            {
                GameObject gobject;
                if (themeState == ThemeState.Mine)
                    gobject = normalPillar;
                else
                    gobject = icePiller;

                gobject.GetComponent<Pillar>().Trigger();
            }
            else
            {
                PerspectiveInitializer.s_Instance.CleanPerspective();

                Global.Instance.Gesture.SetActive(true);

                Fader.s_Instance.InvokeMethod("Enable", 1.25f);
                PerspectiveInitializer.s_Instance.InvokeMethod("LoadHorizontalPerspective", 2f);
                Fader.s_Instance.InvokeMethod("Disable", 2.5f);

                Invoke("disableGesture", 4f);
            }
            Invoke("SwitchPerspective", 40.0f + (UnityEngine.Random.value * 20.0f));
        }
    }
}