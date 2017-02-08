﻿// Copyright 2016 Jon Skeet. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
using System;

namespace CSharp7
{
#pragma warning disable 0649
    struct BigStruct
    {
        internal int x, y, z;
        internal long a, b, c;
    }
#pragma warning restore 0649
    
    class RefLocal
    {
        static void Main()
        {
            BigStruct[] array = new BigStruct[1000];

            Console.WriteLine(array[1].a); // 0
            for (int i = 0; i < array.Length; i++)
            {
                // Creates a copy - not what we want
                BigStruct local = array[i];
                local.b++; // Doesn't affect array
                array[i] = local; // Copy back into array

                ref BigStruct refLocal = ref array[i];
                if (refLocal.x < i)
                {
                    refLocal.a++;
                }
            }
            Console.WriteLine(array[1].a); // 1
        }
    }
}
