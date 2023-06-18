using System.Collections.Generic;
using UnityEngine;

namespace MyInitSet
{
    public interface ISceneActable
    {
        public int ActPriority { get; }
        public void ActSceneChild();
    }

    public interface ISceneChild<SceneT> where SceneT : IScene
    {
        public void SetScene(SceneT sceneT);
    }

    public class MySceneBase : MonoBehaviour, IScene
    {
        public virtual int SceneKind { get; } = -1;

        protected class SceneChildComparer : IComparer<ISceneActable>
        {
            public int Compare(ISceneActable mb1, ISceneActable mb2)
            {
                if (mb1.ActPriority < mb2.ActPriority)
                {
                    return 1;
                }
                else if (mb1.ActPriority > mb2.ActPriority)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        protected static readonly SceneChildComparer comparer = new SceneChildComparer();

        protected readonly List<ISceneActable> sceneChildren = new List<ISceneActable>();

        void Awake()
        {
            OnAwake();
            GetComponentsInChildren<ISceneActable>(true, sceneChildren);
            sceneChildren.Sort(comparer);
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual void OnDeinitialize()
        {

        }

        protected void SetUpChildren<SceneT>(SceneT sceneT) where SceneT : IScene
        {
            ISceneChild<SceneT>[] sceneChildren = GetComponentsInChildren<ISceneChild<SceneT>>(true);

            foreach (ISceneChild<SceneT> sceneChild in sceneChildren)
            {
                sceneChild.SetScene(sceneT);
            }
        }

        public void AddSceneChild(ISceneActable sceneChild)
        {
            sceneChildren.Add(sceneChild);
        }

        public void SortSceneChildren()
        {
            sceneChildren.Sort(comparer);
        }

        #region IScene Implement
        public void InitializeScene()
        {
            OnInitialize();
        }

        public void ActScene()
        {
            foreach (ISceneActable sceneChild in sceneChildren)
            {
                sceneChild.ActSceneChild();
            }
        }

        public void DeinitializeScene()
        {
            OnDeinitialize();
        }
        #endregion
    }
}
