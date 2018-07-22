using System;
using System.Collections.Generic;
using VEdit.Common;

namespace VEdit.UI
{
    public class FileEditorFactory
    {
        private Dictionary<FileType, Func<ProjectFile, FileEditor>> _factory = new Dictionary<FileType, Func<ProjectFile, FileEditor>>();
        private Common.IServiceProvider _provider;

        public FileEditorFactory(Common.IServiceProvider provider)
        {
            _provider = provider;
            Register(FileType.Graph, f => new GraphFileEditor(f, _provider));
            Register(FileType.Image, f => new ImageFileEditor(f, _provider));
            Register(FileType.Text, f => new TextFileEditor(f, _provider));
        }

        public void Register(FileType type, Func<ProjectFile, FileEditor> factory)
        {
            _factory.Add(type, factory);
        }

        public FileEditor Create(FileType type, ProjectFile file)
        {
            if(_factory.TryGetValue(type, out var factory))
            {
                return factory(file);
            }
            return null;
        }
    }
}
