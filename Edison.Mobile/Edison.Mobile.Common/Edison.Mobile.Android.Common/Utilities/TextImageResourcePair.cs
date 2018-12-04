using System;
using System.Collections.Generic;
using System.Linq;


namespace Edison.Mobile.Android.Common
{
    public class TextImageResourcePair
    {

        public string Text { get; set; }
        public int ImageResource { get; set; } = 0;

        public TextImageResourcePair() {}

        public TextImageResourcePair(string text, int imageResource)
        {
            Text = text;
            ImageResource = imageResource;
        }

    }
}