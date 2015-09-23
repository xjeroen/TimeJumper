﻿using Assets.Scripts.Parallaxing;
using Assets.Scripts.Perspective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class PerspectiveInitializer : MonoBehaviour
    {
        public static PerspectiveInitializer s_Instance;

        private GameObject mainCharacter;// = GameObject.Find("Main Character");

        private Theme currentTheme;// = Theme.Default();
        private Perspectives currentPerspective;

        private Theme newTheme;
        private Perspectives newPerspective;

        private GameObject dragon;

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
            Invoke("SwitchPerspective", 40.0f + (UnityEngine.Random.value * 20.0f));
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

            //SpawnManager.Instance.CancelInvoke();
            SpawnManager.Instance.RemoveAll();
            //SpawnManager.Instance.Init();
        }

        public void LoadVerticalPerspective()
        {
            Global.Instance.orientation = 1;

            GameObject mainCharacter = GameObject.Find("Main Character");
            Global.Instance.HealthBarBackground.GetComponent<Image>().enabled = false;
            Global.Instance.HealthBar.GetComponent<Image>().enabled = false;

            Global.Instance.TimeText.transform.localRotation = Quaternion.Euler(0, 0, -90);
            Global.Instance.TimeText.transform.localPosition = new Vector3(-300, 110, Global.Instance.TimeText.transform.localPosition.z);

            Global.Instance.DistanceText.transform.localRotation = Quaternion.Euler(0, 0, -90);
            Global.Instance.DistanceText.transform.localPosition = new Vector3(-300, -40, Global.Instance.DistanceText.transform.localPosition.z);

            mainCharacter.transform.localRotation = Quaternion.Euler(mainCharacter.transform.localRotation.x, mainCharacter.transform.localRotation.y, -90);
            //GameObject.Destroy(GameObject.Find("Platform"));
            mainCharacter.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            mainCharacter.GetComponent<Rigidbody2D>().gravityScale = 0;

            GameObject.Find("Background Manager").GetComponent<BackgroundManager>().InitVertical();

            mainCharacter.GetComponent<BoxCollider2D>().size = new Vector2(0.8542318f, 1.699413f) * 2;
            mainCharacter.GetComponent<Rigidbody2D>().transform.localPosition = new Vector3(2.66f, 3.1f, mainCharacter.GetComponent<Rigidbody2D>().transform.localPosition.z);
            mainCharacter.GetComponent<SpriteRenderer>().sprite = newTheme.m_mainCharacter;
            mainCharacter.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(newTheme.m_mainCharacterAnimationString);

            dragon = GameObject.Find("Dragon");
            dragon.transform.localPosition = new Vector3(5.50f,3.12f,0.5f);
            dragon.transform.localRotation = Quaternion.Euler(0, 0, 270);
            dragon.transform.localScale = new Vector3(1,1,1);
            dragon.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Dragon2");

            SpawnManager.Instance.spawnables.AddRange(newTheme.m_spawnables);
            SpawnManager.Instance.collectables.Add(newTheme.m_collectables[0]);
            SpawnManager.Instance.Init();

            if (mainCharacter.GetComponent<MainCharacter>().inVehicle)
            {
                Global.Instance.speed -= 2f;
                mainCharacter.GetComponent<MainCharacter>().inVehicle = false;
                mainCharacter.GetComponent<Animator>().enabled = true;
            }

            //SpawnManager.Instance.platformManager.FinalizeObject();
        }

        public void LoadHorizontalPerspective()
        {
            Global.Instance.orientation = 0;

            Global.Instance.TimeText.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Global.Instance.TimeText.transform.localPosition = new Vector3(-239, 180, Global.Instance.TimeText.transform.localPosition.z);

            Global.Instance.DistanceText.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Global.Instance.DistanceText.transform.localPosition = new Vector3(171, 180, Global.Instance.DistanceText.transform.localPosition.z);

            mainCharacter.transform.localRotation = Quaternion.Euler(mainCharacter.transform.localRotation.x, mainCharacter.transform.localRotation.y, 0);
            //GameObject.Destroy(GameObject.Find("Platform"));
            mainCharacter.GetComponent<Rigidbody2D>().gravityScale = 1;

            GameObject.Find("Background Manager").GetComponent<BackgroundManager>().InitHorizontal();

            mainCharacter.GetComponent<BoxCollider2D>().size = new Vector2(0.8542318f, 1.699413f);
            mainCharacter.GetComponent<Rigidbody2D>().transform.localPosition = new Vector3(-3.459f, 2.216f, mainCharacter.GetComponent<Rigidbody2D>().transform.localPosition.z);
            mainCharacter.GetComponent<SpriteRenderer>().sprite = Resources.Load("Images/character-v2") as Sprite;
            mainCharacter.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/character_0");

            dragon.transform.localPosition = new Vector3(-5.68f,2.76f,0.5f);
            dragon.transform.localRotation = Quaternion.Euler(0, 0, 0);
            dragon.transform.localScale = new Vector3(1.73f, 1.57f, 1);
            dragon.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Dragon");

            //GameObject.Find("Platform").active = true;

            SpawnManager.Instance.platformManager.Spawn();
            SpawnManager.Instance.spawnables.AddRange(newTheme.m_spawnables);
            SpawnManager.Instance.collectables.AddRange(newTheme.m_collectables);
            SpawnManager.Instance.Init();
        }

        public void SwitchPerspective()
        {
            if (Global.Instance.orientation == 0)
            {
                newTheme = Theme.LoadTestLevel();
                newTheme.m_perspectiveTransitions[0].m_trigger();
            }
            else
            {
                PerspectiveInitializer.s_Instance.CleanPerspective();

                /* GameObject gobject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + "pillar-crash"), new Vector3(0, 5, 0.5f), new Quaternion(0, 0, 0, 0));
                gobject.transform.localPosition += Global.Instance.GlobalObject.transform.localPosition + Global.Instance.ForegroundObject.transform.localPosition;
                gobject.transform.parent = Global.Instance.ForegroundObject.transform; */

                Fader.s_Instance.InvokeMethod("Enable", 1.5f);
                PerspectiveInitializer.s_Instance.InvokeMethod("LoadHorizontalPerspective", 2f);
                Fader.s_Instance.InvokeMethod("Disable", 2.5f);
            }

            Invoke("SwitchPerspective", 40.0f + (UnityEngine.Random.value * 20.0f));
        }
    }
}