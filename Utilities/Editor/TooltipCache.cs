using System.Collections.Generic;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities.Editor
{
    public class TooltipCache
    {
        private Dictionary<string, GUIContent> _tooltipDictionary = new Dictionary<string, GUIContent>();

        public void RegisterTooltip(string tooltipId, GUIContent content)
        {
            if (_tooltipDictionary.ContainsKey(tooltipId))
            {
                Debug.LogWarning("There is a tooltip with the same ID already registered. Overwriting it");
                _tooltipDictionary[tooltipId] = content;
                return;
            }
            
            _tooltipDictionary.Add(tooltipId, content);
        }

        public GUIContent GetTooltip(string tooltipID)
        {
            var tooltipFound = _tooltipDictionary.TryGetValue(tooltipID, out var content);
            
            if (!tooltipFound)
            {
                Debug.LogWarning($"Tooltip with ID {tooltipID} not found.");
            }
            
            return content;
        }

        public void FLush()
        {
            _tooltipDictionary = new Dictionary<string, GUIContent>();
        }
    }
}