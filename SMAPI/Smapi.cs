using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;

namespace SMAPI_ModPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set console and game title
            Console.Title = "SMAPIV2 | running SDV With {Mods} Mods";

            // Initialize Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("com.SMAPI.smapimod");

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
            List<Assembly> modAssemblies = new List<Assembly>();
            foreach (string modFile in Directory.GetFiles(modsFolderPath, "*.dll"))
            {
                try
                {
                    Assembly modAssembly = Assembly.LoadFile(modFile);
                    modAssemblies.Add(modAssembly);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error loading mod assembly '{modFile}': {ex.Message}");
                    Console.ResetColor();
                }
            }

            // Apply patches from mod assemblies
            foreach (Assembly modAssembly in modAssemblies)
            {
                try
                {
                    harmony.PatchAll(modAssembly);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Patched mod: {modAssembly.FullName}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error patching mod '{modAssembly.FullName}': {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("Mod patching completed.");
        }
    }
}
