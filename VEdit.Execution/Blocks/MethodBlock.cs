using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VEdit.Execution
{
    public class MethodBlock : ExecutionBlock
    {
        public MethodInfo Method { get; }

        private bool _hasReturnType;
        private object[] _outParams;
        private int _factoryIndex = -1;

        public MethodBlock(IExecutionContext context, MethodInfo info, int outParamsCount) : base(context)
        {
            Method = info;

            _hasReturnType = Method.ReturnType != typeof(void);
            _outParams = new object[_hasReturnType ? outParamsCount - 1 : outParamsCount];

            var parameters = info.GetParameters().ToList();
            _factoryIndex = parameters.FindIndex(p => p.IsDefined(typeof(ParamArrayAttribute)));
        }

        public override void Execute()
        {
            base.Execute();

            object[] result = null;
            var parameters = In.Select(p => p.Value).ToList();

            if (_factoryIndex != -1)
            {
                var array = Array.CreateInstance(parameters[0].GetType(), parameters.Count - _factoryIndex);
                for(int i = 0; i < parameters.Count - _factoryIndex; i++)
                {
                    array.SetValue(parameters[i + _factoryIndex], i);
                }

                var temp = new List<object> { array };
                temp.AddRange(_outParams);
                result = temp.ToArray();
            }
            else
            {
                parameters.AddRange(_outParams);
                result = parameters.ToArray();
            }

            var returnValue = Method?.Invoke(null, result);

            var total = result.Length - (_hasReturnType ? 1 : 0); 
            for (int i = In.Count; i < total; i++)
            {
                Out[i - In.Count].Value = result[i];
            }

            if (_hasReturnType)
            {
                Out[Out.Count - 1].Value = returnValue;
            }

            ExecuteNext();
        }
    }
}
