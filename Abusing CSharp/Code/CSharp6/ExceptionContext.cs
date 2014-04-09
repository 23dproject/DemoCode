﻿// Copyright 2014 Jon Skeet. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CSharp6
{
    class ExceptionContext
    {
        static void Main()
        {
            try
            {
                OuterMethod(10);
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: {0}", e);
                Console.WriteLine();
                e.DumpContext();
            }
        }

        static void OuterMethod(int n)
        {
            for (int i = 0; i < n; i++)
            {
                try
                {
                    InnerMethod(i);
                }
                catch (Exception e) if (new LogContextEntry { $n = n, $i = i }.AddTo(e))
                { /* Never get here... */ }
            }
        }

        static void InnerMethod(int x)
        {
            try
            {
                if (x == 7)
                {
                    GoBang();
                }
            }
            catch (Exception e) if (new LogContextEntry { $x = x, $now = DateTime.Now }.AddTo(e))
            { /* Never get here... */ }
        }

        static void GoBang()
        {
            throw new IOException("This is the exception");
        }
    }

    static class ExceptionExtensions
    {
        internal static List<LogContextEntry> GetContext(this Exception e)
        {
            return (List<LogContextEntry>) e.Data.$context;
        }

        internal static void DumpContext(this Exception e)
        {
            var list = e.GetContext();
            if (list == null)
            {
                return;
            }
            Console.WriteLine("Context:");
            foreach (var entry in list)
            {
                Console.WriteLine(entry);
            }
        }

        internal static void AddContext(this Exception e, LogContextEntry entry)
        {
            var list = e.GetContext();
            if (list == null)
            {
                list = new List<LogContextEntry>();
                e.Data.$context = list;
            }
            list.Add(entry);
        }
    }

    sealed class LogContextEntry([CallerFilePath] string file = null,
                            [CallerMemberName] string member = null,
                            [CallerLineNumber] int line = 0)
    {
        public string File { get; } = Path.GetFileName(file);
        public string Member { get; } = member;
        public int Line { get; } = line;

        private readonly List<string> keyOrder = new List<string>();
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public object this[string key]
        {
            get { return values[key]; }
            set
            {
                if (!values.ContainsKey(key))
                {
                    keyOrder.Add(key);
                }
                values[key] = value;
            }
        }

        public bool AddTo(Exception e)
        {
            e.AddContext(this);
            return false;
        }

        public override string ToString()
        {
            string valueText = string.Join(", ", keyOrder.Select(key => string.Format("${0} = {1}", key, values[key])));
            return string.Format("{0}:{1} ({2}) - {3}", File, Line, Member, valueText);
        }
    }
}
