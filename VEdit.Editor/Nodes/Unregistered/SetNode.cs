using System;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class SetNode : Node
    {
        public static Guid Template = Guid.Parse("f492c0f0-401d-4d38-bc10-7b2a27f25449");

        private Pin _input;

        public VariableNode Variable { get; }

        // Don't new it, use Variable.CreateSetNode
        public SetNode(VariableNode variable) : base(variable.Graph, Guid.Empty)
        {
            CanCopy = false;

            Variable = variable;
            IsCompact = true;
            Name = "SET";

            X = variable.X + 100;
            Y = variable.Y + 100;

            AddExecPin(Direction.Input);
            AddExecPin(Direction.Output);

            _input = AddDataPin(Direction.Input, variable.Type, variable.Name);
            AddDataPin(Direction.Output, variable.Type);

            variable.Renamed += OnRenamed;

            var cmdProvider = ServiceProvider.Get<ICommandProvider>();
            Actions.Add("Go to variable", cmdProvider.Create(GoToVariable));
        }

        private void GoToVariable()
        {
            Variable.Graph.JumpToNode(Variable);
        }

        private void OnRenamed()
        {
            _input.Name = Variable.Name;
        }

        public Pin GetExecIn() => Input[0];
        public Pin GetInput() => Input[1];
        public Pin GetOutput() => Output[1];

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new SetVariableBlock(Variable.Id, context);
        }
    }
}
