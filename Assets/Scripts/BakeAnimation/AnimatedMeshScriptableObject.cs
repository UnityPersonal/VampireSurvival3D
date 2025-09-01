using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMeshScriptableObject : ScriptableObject
{
    public int AnimationFPS;
    public List<Animation> Animations = new();

    [Serializable]
    public struct Animation
    {
        public string Name;
        public bool loop;
        public List<Mesh> Meshes;
    }
}
