using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ScaleShowcase
{
    public interface INewHorizons
    {
        void Create(Dictionary<string, object> config, IModBehaviour mod);

        void LoadConfigs(IModBehaviour mod);

        GameObject GetPlanet(string name);

        UnityEvent<string> GetStarSystemLoadedEvent();

        string GetCurrentStarSystem();
    }
}
