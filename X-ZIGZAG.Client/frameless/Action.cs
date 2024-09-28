using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading.Tasks;

namespace frameless
{
    internal class Action
    {
        public static async Task<object> ExecuteCsharpCodeAsync(string code, object[] parameters)
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };
            string[] assmbls= {
                    "System.dll",
                    "System.Runtime.dll",
                    "System.Threading.Tasks.dll",
                    "System.Net.Http.dll",
                    "System.Web.Extensions.dll",
                    "Microsoft.CSharp.dll",
                    "System.Security.dll",
                    "System.Dynamic.dll",
                    "System.Core.dll",
                    "mscorlib.dll",
                    "System.Windows.Forms.dll",
                    "System.Management.dll",
                    "System.Runtime.Serialization.Json.dll",
                    "System.Runtime.Serialization.dll",
                    "System.Xml.dll",
                    "System.Drawing.dll"
                };
            foreach (string assembly in assmbls)
            {
                compilerParams.ReferencedAssemblies.Add(assembly);
            }


            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);

            if (results.Errors.HasErrors)
            {
                return null;
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type type = assembly.GetType("Script");
                MethodInfo method = type?.GetMethod("ExecuteAsync");
                if (method == null)
                {
                    return null;
                }
                try
                {
                    object obj = Activator.CreateInstance(type);
                    Task<object> task = (Task<object>)method.Invoke(obj, parameters);
                    object result = await task;
                    return result ?? null;
                }
                catch
                {
                    return null;
                }
            }
        }

    }
}
