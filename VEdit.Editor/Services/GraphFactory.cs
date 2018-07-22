using System;
using System.Collections.Generic;

namespace VEdit.Editor
{
    public interface IGraphFactory
    {
        Graph Create<T>() where T : Graph;
        Graph Create(Type type);
        void Register<T>(Func<Graph> factory);
    }

    public class GraphFactory : IGraphFactory
    {
        private Dictionary<Type, Func<Graph>> _factory = new Dictionary<Type, Func<Graph>>();
        private INodeFactory _nodeFactory;

        public GraphFactory(Common.IServiceProvider serviceProvider)
        {
            _nodeFactory = serviceProvider.Get<INodeFactory>();

            Register<TextGraph>(() => new TextGraph(serviceProvider));
            Register<ImageGraph>(() => new ImageGraph(serviceProvider));
            Register<FunctionGraph>(() => new FunctionGraph(serviceProvider));
        }

        public Graph Create<T>() where T : Graph
        {
            return Create(typeof(T));
        }

        public Graph Create(Type type)
        {
            if (_factory.TryGetValue(type, out Func<Graph> creator))
            {
                return creator();
            }
            return null;
        }

        public void Register<T>(Func<Graph> factory)
        {
            _factory.Add(typeof(T), factory);
        }
    }
}
