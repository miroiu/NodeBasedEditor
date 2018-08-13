using NUnit.Framework;
using System;
using VEdit.Core.Nodes;

namespace VEdit.Core.Tests
{
    [TestFixture]
    [TestOf(typeof(Graph))]
    public class GraphTests
    {
        private readonly Graph _graph = new MethodGraph();

        [Test]
        public void AddNode_ValidNode_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _graph.AddNode(new TestNode()));
        }

        [Test]
        public void AddNode_ParameterIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _graph.AddNode(null));
        }

        [Test]
        public void AddNode_DuplicateNode_ThrowsArgumentException()
        {
            Node testNode = new TestNode();
            _graph.AddNode(testNode);

            Assert.Throws<ArgumentException>(() => _graph.AddNode(testNode));
        }

        [Test]
        public void TryAddNode_ValidNode_ReturnsTrue()
        {
            Assert.IsTrue(_graph.TryAddNode(new TestNode()));
        }

        [Test]
        public void TryAddNode_ParameterIsNull_ReturnsFalse()
        {
            Assert.IsFalse(_graph.TryAddNode(null));
        }

        [Test]
        public void TryAddNode_DuplicateNode_ReturnsFalse()
        {
            Node testNode = new TestNode();

            _graph.AddNode(testNode);

            Assert.IsFalse(_graph.TryAddNode(testNode));
        }
    }
}
