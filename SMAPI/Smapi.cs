using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SMAPI_ModPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console and game title
            Console.Title = "SMAPIV2 | running SDV With {Mods} Mods";

            // Get Mods folder path
            string modsFolderPath = Path.Combine(Environment.CurrentDirectory, "Mods");

            // Ensure Mods folder exists
            if (!Directory.Exists(modsFolderPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Mods folder not found!");
                Console.ResetColor();
                return;
            }

            // Load all mod assemblies
            foreach (string modFile in Directory.GetFiles(modsFolderPath, "*.dll"))
            {
                try
                {
                    // Load the assembly with Mono.Cecil
                    AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(modFile);

                    // Patch methods in the assembly
                    PatchMethods(assembly);

                    // Save the modified assembly
                    string patchedFilePath = Path.Combine(modsFolderPath, Path.GetFileNameWithoutExtension(modFile) + "_patched.dll");
                    assembly.Write(patchedFilePath);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Patched mod: {Path.GetFileName(modFile)}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error patching mod '{Path.GetFileName(modFile)}': {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("Mod patching completed.");

            // Launch Stardew Valley
            LaunchStardewValley();
        }

        static void PatchMethods(AssemblyDefinition assembly)
        {
            // Iterate through types in the assembly
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                // Iterate through methods in the type
                foreach (MethodDefinition method in type.Methods)
                {
                    // Check if the method is the one you want to patch
                    if (method.Name == "MethodNameToPatch")
                    {
                        // Modify the IL code of the method
                        method.Body.Instructions.Clear();
                        method.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
                    }
                }
            }
        }

        static void LaunchStardewValley()
        {
            try
            {
                // Extract Stardew Valley executable from resources
                string sdvExePath = Path.Combine(Environment.CurrentDirectory, "Stardew Valley.exe");
                if (!File.Exists(sdvExePath))
                {
                    // Extract embedded resource to disk
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SMAPI_ModPatcher.StardewValley.exe"))
                    {
                        using (FileStream fileStream = new FileStream(sdvExePath, FileMode.Create))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }

                // Launch Stardew Valley
                Process.Start(sdvExePath);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Error launching Stardew Valley: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
