using System;
using System.Collections.Generic;

namespace VEdit.Core
{
    [Serializable]
    public abstract class Node
    {
        // Represents the instance id
        public Guid Id { get; }

        private readonly List<DataSocket> _inputData = new List<DataSocket>();
        public IReadOnlyList<DataSocket> InputData => _inputData;

        private readonly List<DataSocket> _outputData = new List<DataSocket>();
        public IReadOnlyList<DataSocket> OutputData => _outputData;

        private readonly List<ExecSocket> _inputExec = new List<ExecSocket>();
        public IReadOnlyList<ExecSocket> InputExec => _inputExec;

        private readonly List<ExecSocket> _outputExec = new List<ExecSocket>();
        public IReadOnlyList<ExecSocket> OutputExec => _outputExec;

        public Node()
        {
            Id = Guid.NewGuid();
        }

        public virtual bool AddSocket(Socket socket)
        {
            if (socket is DataSocket ds)
            {
                UseDataSocket(socket.Type, container => container.Add(ds));
            }
            else if (socket is ExecSocket es)
            {
                UseExecSocket(socket.Type, container => container.Add(es));
            }
            return true;
        }

        public virtual bool RemoveSocket(Socket socket)
        {
            if (socket is DataSocket ds)
            {
                UseDataSocket(socket.Type, container => container.Remove(ds));
            }
            else if (socket is ExecSocket es)
            {
                UseExecSocket(socket.Type, container => container.Remove(es));
            }
            return true;
        }

        public override int GetHashCode() => Id.GetHashCode();

        private void UseDataSocket(SocketType type, Action<List<DataSocket>> operation)
        {
            if (type == SocketType.Input)
            {
                operation(_inputData);
            }
            else
            {
                operation(_outputData);
            }
        }

        private void UseExecSocket(SocketType type, Action<List<ExecSocket>> operation)
        {
            if (type == SocketType.Input)
            {
                operation(_inputExec);
            }
            else
            {
                operation(_outputExec);
            }
        }
    }
}
