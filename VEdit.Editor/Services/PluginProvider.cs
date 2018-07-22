using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VEdit.Common;
using VEdit.Plugins;

namespace VEdit.Editor
{
    public interface IPluginProvider
    {
        IEnumerable<Plugin> Load();
    }

    public class PluginProvider : IPluginProvider
    {
        const string PluginsFolder = "Plugins";

        private readonly Common.IServiceProvider _serviceProvider;
        private readonly IPluginFactory _factory;

        private Dictionary<Guid, Plugin> _plugins;

        public PluginProvider(Common.IServiceProvider serviceProvider, IOutputManager errorReporter)
        {
            _serviceProvider = serviceProvider;
            _factory = _serviceProvider.Get<IPluginFactory>();

            _plugins = new Dictionary<Guid, Plugin>();
        }

        public IEnumerable<Plugin> Load()
        {
            if (!Directory.Exists(PluginsFolder))
            {
                Directory.CreateDirectory(PluginsFolder);
            }

            var plugins = new List<Plugin>();

            foreach (var file in Directory.EnumerateFiles(PluginsFolder))
            {
                if (file.GetFileType() == FileType.Plugin)
                {
                    plugins.AddRange(Load(file));
                }
            }

            return plugins;
        }

        private IEnumerable<Plugin> Load(string file)
        {
            var plugins = new List<Plugin>();
            var assembly = Assembly.LoadFrom(file);

            foreach (var type in assembly.GetMarkedTypes<ContainsNodesAttribute>())
            {
                foreach (var method in type.GetMarkedMethods<NodeAttribute>())
                {
                    plugins.Add(CreatePluginAsync(method));
                }
            }

            return plugins;
        }

        private Plugin CreatePluginAsync(MethodInfo method)
        {
            var id = method.GetFullNameWithParameters().ToGuid();
            return _factory.Create(id, method) as Plugin;
        }
    }
}
