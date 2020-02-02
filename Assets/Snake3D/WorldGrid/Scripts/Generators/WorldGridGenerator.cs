using EntroPi;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    public abstract class WorldGridGenerator : EntroPiObject
    {
        [SerializeField]
        private string m_PlayerPrefKey = string.Empty;

        public string PlayerPrefKey { get { return m_PlayerPrefKey; } }

        public abstract Vector3Int PlayerStartDirection { get; }

        public abstract WorldGridModel GenerateWorldGridModel(Transform gridRoot);
    }
}