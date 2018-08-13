namespace VEdit.Core
{
    public abstract class Builder<T, TBuilder>
        where T: class, new()
        where TBuilder: Builder<T, TBuilder>
    {
        protected T Object;
        protected readonly TBuilder _this;

        public Builder()
        {
            Object = new T();
            _this = (TBuilder)this;
        }

        public T Build()
        {
            T result = Object;
            Object = null;
            return result;
        }
    }

    public abstract class NodeBuilder<TNode, TBuilder> : Builder<TNode, TBuilder>
        where TNode: Node, new()
        where TBuilder: NodeBuilder<TNode, TBuilder>
    {
        public TBuilder AddInput<T>()
        {
            Object.AddSocket(new DataSocket<T>(Object, SocketType.Input));
            return _this;
        }
    }

    public class GenericNodeBuilder : NodeBuilder<GenericNode, GenericNodeBuilder>
    {

    }
}
