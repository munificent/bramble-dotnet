﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bramble.Core
{
    public static class Obj
    {
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp;
            temp = left;
            left = right;
            right = temp;
        }
    }
}
