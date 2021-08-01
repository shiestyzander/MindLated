﻿using ControlzEx.Theming;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using MindLated.Protection.Anti;
using MindLated.Protection.Arithmetic;
using MindLated.Protection.CtrlFlow;
using MindLated.Protection.INT;
using MindLated.Protection.InvalidMD;
using MindLated.Protection.LocalF;
using MindLated.Protection.Other;
using MindLated.Protection.Proxy;
using MindLated.Protection.Renamer;
using MindLated.Protection.String;
using MindLated.Protection.StringOnline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;

namespace MindLated
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MethodDef Init;
        public static MethodDef Init2;

        private string _directoryName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();
        }

        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        { Process.Start("https://github.com/Sato-Isolated/MindLated"); }

        public byte MaxValue = byte.MaxValue;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            var module = ModuleDefMD.Load(LoadBox.Text);
            LoadPlugin();
            var assemblyResolver = new AssemblyResolver();
            var moduleContext = new ModuleContext(assemblyResolver);
            assemblyResolver.DefaultModuleContext = moduleContext;
            assemblyResolver.EnableTypeDefCache = true;
            assemblyResolver.DefaultModuleContext = new ModuleContext(assemblyResolver);
            assemblyResolver.PostSearchPaths.Insert(0, LoadBox.Text);
            foreach (var assemblyRef in module.GetAssemblyRefs())
            {
                assemblyResolver.ResolveThrow(assemblyRef, module);
                ConsoleLog.AppendText($@"Resolved {assemblyRef.Name}{Environment.NewLine}");
            }

            if (StringEnc.IsChecked == true)
            {
                StringEncPhase.Execute(module);
                ConsoleLog.Foreground = Brushes.Aqua;
                ConsoleLog.AppendText($"{time} Processing String Encryption{Environment.NewLine}");
            }

            if (SOD.IsChecked == true)
            {
                OnlinePhase.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Online Decryption{Environment.NewLine}");
            }

            if (Cflow.IsChecked == true)
            {
                ControlFlowObfuscation.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Control Flow{Environment.NewLine}");
            }

            if (Ahri.IsChecked == true)
            {
                Arithmetic.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Arithmetic{Environment.NewLine}");
            }

            if (IntConf.IsChecked == true)
            {
                AddIntPhase.Execute2(module);
                ConsoleLog.AppendText($"{time} Processing Int Confusion{Environment.NewLine}");
            }

            if (SUC.IsChecked == true)
            {
                StackUnfConfusion.Execute(module);
                ConsoleLog.AppendText($"{time} Processing StackUnfConfusion{Environment.NewLine}");
            }

            if (LF.IsChecked == true)
            {
                L2F.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Local Field{Environment.NewLine}");
            }

            if (LFV2.IsChecked == true)
            {
                L2FV2.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Local Field V2{Environment.NewLine}");
            }

            if (Calli_.IsChecked == true)
            {
                Calli.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Call To Calli{Environment.NewLine}");
            }

            if (Proxy_String.IsChecked == true)
            {
                ProxyString.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Proxy Strings{Environment.NewLine}");
            }

            if (ProxyConstants.IsChecked == true)
            {
                ProxyInt.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Proxy Constants{Environment.NewLine}");
            }

            if (Proxy_Meth.IsChecked == true)
            {
                ProxyMeth.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Proxy Methods{Environment.NewLine}");
            }

            if (Renamer.IsChecked == true)
            {
                // Protection.Renamer.Renamer.Renamer3.Rename(module);
                RenamerPhase.Execute(module);

                ConsoleLog.AppendText($"{time} Processing Renaming{Environment.NewLine}");
            }

            if (Anti_De4dot.IsChecked == true)
            {
                AntiDe4dot.Execute(module.Assembly);
                ConsoleLog.AppendText($"{time} Processing Anti De4dot{Environment.NewLine}");
            }

            if (JumpCflow.IsChecked == true)
            {
                JumpCFlow.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Jump Control flow{Environment.NewLine}");
            }

            if (AntiDebug.IsChecked == true)
            {
                Protection.Anti.AntiDebug.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Anti Debug{Environment.NewLine}");
            }

            // if (ListPlugin.Count != 0)
            // {
            //     foreach (var meth in ListPlugin)
            //         meth.Invoke(null, new object[] { module });
            // }

            if (Anti_Dump.IsChecked == true)
            {
                AntiDump.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Anti Dump{Environment.NewLine}");
            }
            Watermark.Execute(module);

            if (AntiTamper.IsChecked == true)
            {
                Protection.Anti.AntiTamper.Execute(module);
                ConsoleLog.AppendText($"{time} Processing Anti Tamper{Environment.NewLine}");
            }

            if (InvalidMD.IsChecked == true)
            {
                InvalidMDPhase.Execute(module.Assembly);
                ConsoleLog.AppendText($"{time} Processing Invalid MetaData{Environment.NewLine}");
            }
            Antimanything.Execute(module);
            var text2 = Path.GetDirectoryName(LoadBox.Text);
            if (text2 != null && !text2.EndsWith("\\"))
            {
                text2 += "\\";
            }

            var path = $"{text2}{Path.GetFileNameWithoutExtension(LoadBox.Text)}_protected{Path.GetExtension(LoadBox.Text)}";

            module.Write(path,
                         new ModuleWriterOptions(module)
                         { PEHeadersOptions = { NumberOfRvaAndSizes = 13 }, Logger = DummyLogger.NoThrowInstance });

            ConsoleLog.AppendText($"{time} {path}");

            if (AntiTamper.IsChecked == true)
            {
                Protection.Anti.AntiTamper.Sha256(path);
            }
        }

        private static readonly List<MethodInfo> ListPlugin = new();

        private static void LoadPlugin()
        {
            var allAssemblies = new List<Assembly>();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (var dll in Directory.GetFiles(path ?? throw new InvalidOperationException(), "*.dll"))
            {
                if (dll.Contains("Plugin"))
                    allAssemblies.Add(Assembly.LoadFile(dll));
            }
            if (allAssemblies.Count != 0)
                foreach (var meth in from asm in allAssemblies from type in asm.GetTypes() where type.Name == "MindPlug" select type.GetMethod("Execute"))
                    ListPlugin.Add(meth);
        }

        private void LoadBox_DragEnter(object sender, DragEventArgs e)
        { e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; }

        private void LoadBox_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var array = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (array == null)
                    return;
                var text = array.GetValue(0).ToString();
                var num = text.LastIndexOf(".", StringComparison.Ordinal);
                if (num == -1)
                    return;
                var text2 = text.Substring(num);
                text2 = text2.ToLower();
                if (string.Compare(text2, ".exe", StringComparison.Ordinal) != 0 && string.Compare(text2, ".dll", StringComparison.Ordinal) != 0)
                {
                    return;
                }

                Activate();
                LoadBox.Text = text;
                var num2 = text.LastIndexOf("\\", StringComparison.Ordinal);
                if (num2 != -1)
                {
                    _directoryName = text.Remove(num2, text.Length - num2);
                }

                if (_directoryName.Length == 2)
                {
                    _directoryName += "\\";
                }
            }
            catch
            {
                /* ignored */
            }
        }

        private void LoadBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}