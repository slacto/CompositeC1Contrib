﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompositeC1Contrib.Rendering.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static C1HtmlHelper C1(this HtmlHelper helper)
        {
            return new C1HtmlHelper(helper);
        }
    }
}
