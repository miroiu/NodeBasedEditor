using System;
using System.Collections.Generic;
using System.Reflection;
using VEdit.Common;
using VEdit.Plugins;

namespace VEdit.Editor
{
    public interface IPluginFactory
    {
        NodeEntry Create(Guid id, MethodInfo method);
    }

    public class PluginBuilder : IPluginFactory
    {
        private string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        private List<PinData> CreatePinsFromAttribute(GenerateArrayAttribute minCount, Type elementType)
        {
            var pinsToAdd = new List<PinData>();

            var count = minCount.Value > 0 ? minCount.Value : 1;
            for (int i = 0; i < minCount.Value; i++)
            {
                var name = minCount.Name == Pattern.Letter ? _alphabet[i].ToString() : minCount.Name == Pattern.Number ? i.ToString() : string.Empty;
                pinsToAdd.Add(new PinData(name, elementType));
            }

            return pinsToAdd;
        }

        private void AddExecutionPins(NodeEntry plugin, MethodInfo method)
        {
            plugin.Input.Add(new PinData());
            plugin.Output.Add(new PinData());
        }

        private void AddDataPins(NodeEntry plugin, MethodInfo method)
        {
            bool hasOutParams = false;
            foreach (var param in method.GetParameters())
            {
                var pin = new PinData(param.Name.Beautify(), param.ParameterType);
                var pinsToAdd = new List<PinData>() { pin };

                // TODO: Add pin factory button for "in/out params"
                if (param.IsDefined(typeof(ParamArrayAttribute)))
                {
                    if (param.IsDefined(typeof(GenerateArrayAttribute)))
                    {
                        var minCount = param.GetCustomAttribute<GenerateArrayAttribute>();
                        pinsToAdd = CreatePinsFromAttribute(minCount, param.ParameterType.GetElementType());
                        pinsToAdd[0].IsFactory = true;
                    }
                    else
                    {
                        pin = new PinData(string.Empty, param.ParameterType.GetElementType())
                        {
                            IsFactory = true
                        };
                    }
                }

                if (param.IsOut)
                {
                    plugin.Output.AddRange(pinsToAdd);
                    hasOutParams = true;
                }
                else
                {
                    plugin.Input.AddRange(pinsToAdd);
                }
            }

            if (method.ReturnType != typeof(void))
            {
                // TODO: Override return name
                plugin.Output.Add(new PinData(plugin.Output.Count == 0 ? string.Empty : hasOutParams ? "Return" : string.Empty, method.ReturnType));
            }
        }

        public Plugin CreateExecutable(Guid id, ExecutableNodeAttribute attribute, MethodInfo method)
        {
            Plugin plugin = new Plugin(id, method)
            {
                Tooltip = attribute.Tooltip,
                DisplayName = attribute.DisplayName ?? method.Name.Beautify(),
            };

            return plugin;
        }

        public NodeEntry CreatePure(Guid id, PureNodeAttribute attribute, MethodInfo method)
        {
            Plugin plugin = new Plugin(id, method)
            {
                Tooltip = attribute.Tooltip,
                DisplayName = attribute.DisplayName,
                CompactName = attribute.CompactNodeTitle
            };

            return plugin;
        }

        public NodeEntry Create(Guid id, MethodInfo method)
        {
            NodeEntry plugin = null;
            var attribute = method.GetCustomAttribute<NodeAttribute>();

            if (attribute is PureNodeAttribute pure)
            {
                plugin = CreatePure(id, pure, method);
            }
            else if (attribute is ExecutableNodeAttribute exec)
            {
                plugin = CreateExecutable(id, exec, method);
                AddExecutionPins(plugin, method);
            }

            AddDataPins(plugin, method);

            plugin.Category = attribute.Category;
            plugin.Keywords = attribute.Keywords;

            return plugin;
        }
    }
}
