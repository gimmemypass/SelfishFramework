﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SelfishFramework.Core
{
    [Serializable]
    public partial class Actor
    {
        [NonSerialized]
        public int Id;
        [NonSerialized]
        public int Generation;
        [NonSerialized]
        public bool IsInitted;
        public World World { get; private set; }

        //systems
    }

    public partial class Actor : MonoBehaviour, IDisposable
    {
        [Serializable]
        private class InitModule
        {
            public enum InitWhenMode
            {
                Manually,
                OnAwake,
                OnStart
            }

            [SerializeField] private InitWhenMode initWhen;
            [SerializeField] private int worldIndex;

            public InitWhenMode InitWhen => initWhen;
            public int WorldIndex => worldIndex;
        }

        [SerializeField] private InitModule initModule = new();
        public void Init([NotNull]World world)
        {
            World = world;
            World.RegisterActor(this);
            IsInitted = true;
        }

        public void Dispose()
        {
            if (!IsInitted)
                return;
            World.UnregisterActor(this);
            IsInitted = false; 
        }

        private void Awake()
        {
            if (IsInitted)
            {
                Debug.LogError("Actor already initted");
                return;
            }
            if (initModule.InitWhen == InitModule.InitWhenMode.OnAwake)
            {
                Init(ActorsManager.Worlds[initModule.WorldIndex]);
            }       
        }

        private void Start()
        {
            if (IsInitted)
            {
                Debug.LogError("Actor already initted");
                return;
            }
            if (initModule.InitWhen == InitModule.InitWhenMode.OnStart)
            {
                Init(ActorsManager.Worlds[initModule.WorldIndex]);
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }

}