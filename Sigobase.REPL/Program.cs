using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using Sigobase.Database;
using Sigobase.Utils;

namespace Sigobase.REPL {
    internal class Program {
        private static readonly Writer DefaultWriter = Writer.Pretty;
        private static ISigo context = Sigo.Create(0);
        private static Configuration config;

        private static void LoadConfiguration() {
            try {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                context = Sigo.Parse(config.AppSettings.Settings["context"].Value);
            } catch (Exception) {
                // ignored
            }
        }

        private static void Main(string[] args) {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-vn");
            LoadConfiguration();
            Console.CancelKeyPress += (sender, e) => SaveConfiguration();

            Console.WriteLine("Welcome to sigo REPL.");
            while (true) {
                Console.Write("sigo> ");
                var src = Console.ReadLine();
                if (src == null) {
                    return;
                }

                if (src == "") {
                    continue;
                }

                if (src == "exit") {
                    SaveConfiguration();
                    return;
                }

                if (src == "dir") {
                    foreach (var (k, v) in context) {
                        Console.WriteLine($"{k}: {v}");
                    }

                    continue;
                }

                if (src == "cls" || src == "clear") {
                    Console.Clear();
                    continue;
                }

                try {
                    Console.WriteLine(Sigo.Parse(src, context, out context).ToString(DefaultWriter));
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        private static void SaveConfiguration() {
            config.AppSettings.Settings["context"].Value = context.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}