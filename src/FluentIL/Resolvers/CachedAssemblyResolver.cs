using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace FluentIL.Resolvers
{
    public class CachedAssemblyResolver : BaseAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> _cache = new Dictionary<string, AssemblyDefinition>();

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var result = this._cache.ContainsKey(name.FullName) ? this._cache[name.FullName] : null;

            if (result == null)
            {
                result = this.LookupAssembly(name, parameters);
                this._cache[name.FullName] = result;
            }

            return result;
        }

        protected virtual AssemblyDefinition LookupAssembly(AssemblyNameReference name, ReaderParameters parameters)
        {
            return base.Resolve(name, parameters);
        }

        protected internal void RegisterAssembly(AssemblyDefinition assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var name = assembly.Name.FullName;
            if (this._cache.ContainsKey(name))
                return;

            this._cache[name] = assembly;
        }
    }
}