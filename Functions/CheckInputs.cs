using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Functions
{
    public static class CheckInputs
    {
        // Valid inputs?
        public static bool TextInputs(Entry[] inputs)
        {
            foreach(var i in inputs)
            {
                if (i.Text == null || i.Text == "" || i.Text == " ")
                {
                    i.PlaceholderColor = Colors.Red;
                    return false;
                }
            }

            return true;
        }

        // Valid images?
        public static bool ImageInputs(Image[] images)
        {
            foreach(var i in images)
            {
                if (i.Source is null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
