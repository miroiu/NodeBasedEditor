using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using VEdit.Core.Nodes;

namespace VEdit.Core.Tests
{
    [TestFixture]
    public class SerializationTest
    {
        [Test]
        public void Serialize_ValidNode_DoesNotThrow()
        {
            Node node = new TestNode();

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, node);

                stream.Seek(0, SeekOrigin.Begin);

                Node result = (TestNode)formatter.Deserialize(stream);
            }
        }
    }
}
