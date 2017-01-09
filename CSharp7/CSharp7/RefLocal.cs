﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
