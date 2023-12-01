using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day7
    {

        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day7_input.txt");

            GenerateFileSystem(inputs, out _Directory root);

            // find all directories that have a size of < 100.000;
            int count = 0;
            root.ExecuteForAll((x) =>
            {
                if (x is _Directory y && x.Size < 100_000)
                    count += y.Size;
            });

            return count.ToString();
        }

        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("Y22/day7_input.txt");

            GenerateFileSystem(inputs, out _Directory root);

            // find all directories that have a size of < 100.000;
            int totalSpace = 70_000_000;
            int neededSpace = 30_000_000 - (totalSpace - root.Size);
            int foundDirSize = root.Size;
            root.ExecuteForAll((x) =>
            {
                if (x is _Directory y && x.Size >= neededSpace && x.Size < foundDirSize)
                    foundDirSize = x.Size;
            });

            return foundDirSize.ToString();
        }




        private interface IStorageComponent
        {
            public string Name { get; init; }
            public IStorageComponent? Parent { get; }

            public int Size { get; }

            public void SetParent(IStorageComponent? parent);
            public void ExecuteForAll(Action<IStorageComponent> action);
        }

        private class _File : IStorageComponent
        {
            public string Name { get; init; }
            public IStorageComponent? Parent { get; private set; }
            public int Size { get; init; }
            public void SetParent(IStorageComponent? parent) => Parent = parent;
            public void ExecuteForAll(Action<IStorageComponent> action) => action(this);
        }
        private class _Directory : IStorageComponent
        {
            public string Name { get; init; }
            public IStorageComponent? Parent { get; private set; }
            private Dictionary<string, IStorageComponent> components = new Dictionary<string, IStorageComponent>();
            public void AddComponent(IStorageComponent component)
            {
                components.Add(component.Name, component);
                component.SetParent(this);
            }
            public void SetParent(IStorageComponent? parent) => Parent = parent;

            public int Size
            {
                get
                {
                    int count = 0;
                    foreach (var component in components)
                        count += component.Value.Size;
                    return count;
                }
            }
            public void ExecuteForAll(Action<IStorageComponent> action)
            {
                action(this);
                foreach (var component in components)
                {
                    component.Value.ExecuteForAll(action);
                }
            }

            public bool Contains(string name)
            {
                return components.ContainsKey(name);
            }
            public IStorageComponent? GetComponent(string name)
            {
                if (components.TryGetValue(name, out var value))
                    return value;
                return null;
            }
        }


        private static void GenerateFileSystem(string[] inputs, out _Directory root)
        {
            if (inputs == null || inputs.Length == 0)
                throw new ArgumentException("Empty", nameof(inputs));
            if (inputs[0] != "$ cd /")
                throw new ArgumentException("To generate a file system you need to start with $ cd /", nameof(inputs));

            root = new _Directory() { Name = "root" };
            _Directory workingDir = root;
            for (var i = 1; i < inputs.Length; i++)
            {
                if (inputs[i].StartsWith("$ cd"))
                    MoveToDir(ref workingDir, inputs[i]);
                else if (inputs[i].StartsWith("$ ls"))
                {
                    DirInfo(ref workingDir, inputs, ref i);
                    continue;
                }
            }
        }


        private static void DirInfo(ref _Directory workingDir, string[] commands, ref int index)
        {
            if (!commands[index].StartsWith("$ ls"))
                throw new ArgumentException(nameof(commands));

            while (true)
            {
                index++;
                if (index >= commands.Length || commands[index].StartsWith('$'))
                {
                    index--;
                    return;
                }
                if (int.TryParse(commands[index][..1], out _))
                {
                    var info = commands[index].Split();
                    var FileName = info[1];
                    if (!workingDir.Contains(FileName))
                    {
                        var file = new _File { Size = int.Parse(info[0]), Name = FileName };
                        workingDir.AddComponent(file);
                    }
                }
                if (commands[index].StartsWith("dir"))
                {
                    var info = commands[index].Split();
                    var dirName = commands[index].Split(" ")[1];
                    if (!workingDir.Contains(dirName))
                    {
                        var dir = new _Directory { Name = dirName };
                        workingDir.AddComponent(dir);
                    }
                }
            }

        }

        private static void MoveToDir(ref _Directory workingDir, string command)
        {
            if (command == "$ cd ..")
                workingDir = workingDir.Parent != null ? workingDir.Parent as _Directory : throw new Exception("Current dir does not contain a parent");
            else if (command == "$ cd /")
                while (workingDir.Parent != null)
                    workingDir = workingDir.Parent as _Directory;
            else if (command.StartsWith("$ cd"))
            {
                var dirName = command.Split(" ").Last();
                var comp = workingDir.GetComponent(dirName);
                if (comp == null)
                {
                    var dir = new _Directory() { Name = dirName };
                    workingDir.AddComponent(dir);
                    workingDir = dir;
                }
                else
                {
                    if (comp is _Directory dir)
                        workingDir = dir;
                    else
                        throw new Exception($"Cannot create \'{dirName}\' directory. There already exist a file with the same name");
                }
            }
        }

    }
}
