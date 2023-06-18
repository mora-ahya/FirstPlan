using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyInitSet
{
    public interface IScene
    {
        public int SceneKind { get; }
        public void InitializeScene();

        public void ActScene();

        public void DeinitializeScene();
    }

    public class MySceneManager : MonoBehaviour, IManagerBase
    {
        public int ActPriority { get; protected set; } = 0;

        [SerializeField] protected GameObject sceneParentObject = default;

        protected readonly Dictionary<int, IScene> sceneDict = new Dictionary<int, IScene>();

        protected IScene currentScene;
        protected int nextSceneNum = -1;

        void Awake()
        {
            OnAwake();
            IScene[] scenes = sceneParentObject.GetComponentsInChildren<IScene>(true);

            foreach (IScene scene in scenes)
            {
                sceneDict.Add(scene.SceneKind, scene);
            }

            currentScene = scenes[0];
        }

        protected virtual void OnAwake()
        {

        }

        public void Act()
        {
            if (nextSceneNum >= 0)
            {
                currentScene.DeinitializeScene();
                currentScene = sceneDict[nextSceneNum];
                currentScene.InitializeScene();
                nextSceneNum = -1;
            }
            else
            {
                currentScene.ActScene();
            }
        }

        public void ChangeScene(int num)
        {
            if (num < 0)
            {
                return;
            }

            if (sceneDict.ContainsKey(num))
            {
                nextSceneNum = num;
            }
        }
    }
}
