using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace DiagramDesigner.BaseClass
{
    public static class DropHelper
    {

        private static object _dragObj;
        public static void OnDragData(object obj) => _dragObj = obj;


        public static object GetDragData()
        {
            var temp = _dragObj;

            _dragObj = null;

            return temp;
        }
    }
}
