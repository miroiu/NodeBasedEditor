using System;
using VEdit.Common;

namespace VEdit.Editor
{
    public abstract class EditableNode : Node
    {
        public EditableNode(Graph root) : base(root, Guid.Empty)
        {
        }

        public bool IsEditable { get; set; }

        public abstract Pin AddExecPin(string name = null);
        public abstract Pin AddDataPin(Type dataType, string name = null);

        public override void LoadPins(IArchive pinsArchive, bool input)
        {
            foreach (var archive in pinsArchive.ReadAll<Archive>())
            {
                var index = archive.Read<int>(nameof(Pin.Index));
                if (input)
                {
                    if (Input.Count > index)
                    {
                        Input[index].Load(archive);
                        continue;
                    }
                }
                else
                {
                    if (Output.Count > index)
                    {
                        Output[index].Load(archive);
                        continue;
                    }
                }

                if (archive.HasKey(nameof(Pin.Type)))
                {
                    var type = archive.Read<Type>(nameof(Pin.Type));
                    var newPin = AddDataPin(type);
                    newPin.Load(archive);
                }
                else
                {
                    var newPin = AddExecPin();
                    newPin.Load(archive);
                }
            }
        }

        public void CopyPin(Pin pin)
        {
            if (IsEditable)
            {
                Pin newPin = null;
            if (pin.IsExec)
            {
                newPin = AddExecPin(pin.Name);
            }
            else
            {
                newPin = AddDataPin(pin.Type, pin.Name);
            }
            }
        }

        public override bool TryToConnectPin(Pin pin)
        {
            CopyPin(pin);

            return base.TryToConnectPin(pin);
        }
    }
}
