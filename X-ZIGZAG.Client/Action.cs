using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace frameless
{
    internal class Action
    {
        public static async Task<string> ExecuteCsharpCodeAsync(string code, object[] parameters)
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Runtime.dll");
            compilerParams.ReferencedAssemblies.Add("System.Threading.Tasks.dll");
            compilerParams.ReferencedAssemblies.Add("System.Net.Http.dll");
            compilerParams.ReferencedAssemblies.Add("System.Web.Extensions.dll");
            compilerParams.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParams.ReferencedAssemblies.Add("System.Security.dll");
            compilerParams.ReferencedAssemblies.Add("System.Dynamic.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add("System.Management.dll");

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, code);

            if (results.Errors.HasErrors)
            {
                var errorBuilder = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    errorBuilder.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                return errorBuilder.ToString();
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type type = assembly.GetType("Script");
                MethodInfo method = type?.GetMethod("ExecuteAsync");

                if (method == null)
                {
                    return "No 'ExecuteAsync' method";
                }
                try
                {
                    object obj = Activator.CreateInstance(type);

                    // Pass the parameters when invoking the method
                    Task<string> task = (Task<string>)method.Invoke(obj, parameters);
                    string result = await task;
                    return result ?? "no output.";
                }
                catch (Exception ex)
                {
                    return $"Error during execution: {ex.Message}";
                }
            }
        }

    }
}
