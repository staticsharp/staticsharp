﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StaticSharpWeb {

    public interface IScript : IInclude {
        IEnumerable<IScript> Dependencies { get; }
        public string Path { get; }
    }

    public class Script : IScript {
        public string Path { get; }

        public string Key => GetType().FullName + '\0' + Path;


        public IEnumerable<IScript> Dependencies => Enumerable.Empty<IScript>();


        public Script(string path) => Path = path;

        public virtual async Task<string> GenerateAsync(IStorage storage) => await File.ReadAllTextAsync(Path);

        public string GenerateSuperScript(string[] script) {
            string result = "";
            try {
                foreach(var i in script) {
                    result += File.ReadAllText(i) + "\r\n";
                }
                return result;
            } catch(Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }
    }

}