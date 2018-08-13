using System;
using NUnit.Framework;

namespace VEdit.Core.Tests
{
    [TestFixture]
    public class NodeBuilderTests
    {
        private readonly NodeBuilder<GenericNode, GenericNodeBuilder> _builder = new GenericNodeBuilder();

        [Test]
        public void AddInput_Chain()
        {
            _builder.AddInput<int>().AddInput<bool>();
        }
    }
}
