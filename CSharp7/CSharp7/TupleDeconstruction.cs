﻿// Copyright 2016 Jon Skeet. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;

namespace CSharp7
{
    class TupleDeconstruction
    {
        static void Main()
        {
            var (a, b, c) = CreateTuple();
            // Equivalent to:
            // var tuple = CreateTuple();
            // var a = tuple.x;
            // var b = tuple.y;
            // var c = tuple.z;
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
        }

        static (int x, int y, string z) CreateTuple()
        {
            return (5, 3, "hello");
        }
    }
}
