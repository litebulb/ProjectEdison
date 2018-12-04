using System.Text;


namespace Edison.Mobile.Android.Common
{
    public static class StateListUtilities
    {

        public static string StateListToString(int[] states)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (int state in states)
            {

                switch (state)
                {
                    case (global::Android.Resource.Attribute.StateEnabled):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Enabled");
                        break;

                    case (-global::Android.Resource.Attribute.StateEnabled):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Disabled");
                        break;

                    case (global::Android.Resource.Attribute.StateFocused):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Focused");
                        break;

                    case (-global::Android.Resource.Attribute.StateFocused):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Unfocused");
                        break;

                    case (global::Android.Resource.Attribute.StateSelected):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Selected");
                        break;

                    case (-global::Android.Resource.Attribute.StateSelected):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Unselected");
                        break;

                    case (global::Android.Resource.Attribute.StatePressed):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Pressed");
                        break;

                    case (-global::Android.Resource.Attribute.StatePressed):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("NotPressed");
                        break;

                    case (global::Android.Resource.Attribute.StateChecked):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Checked");
                        break;

                    case (-global::Android.Resource.Attribute.StateChecked):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Unchecked");
                        break;

                    case (global::Android.Resource.Attribute.StateActivated):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Activated");
                        break;

                    case (-global::Android.Resource.Attribute.StateActivated):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Deactivated");
                        break;

                    case (global::Android.Resource.Attribute.StateActive):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Active");
                        break;

                    case (-global::Android.Resource.Attribute.StateActive):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("NotActive");
                        break;

                    case (global::Android.Resource.Attribute.StateCheckable):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Checkable");
                        break;

                    case (-global::Android.Resource.Attribute.StateCheckable):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("NotCheckable");
                        break;

                    case (global::Android.Resource.Attribute.StateExpanded):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Expanded");
                        break;

                    case (-global::Android.Resource.Attribute.StateExpanded):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Collapsed");
                        break;

                    case (global::Android.Resource.Attribute.StateHovered):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Hovered");
                        break;

                    case (-global::Android.Resource.Attribute.StateHovered):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("NotHovered");
                        break;

                    case (global::Android.Resource.Attribute.StateWindowFocused):
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("WindowFocused");
                        break;

                    case (-global::Android.Resource.Attribute.StateWindowFocused):


                    default:
                        if (i > 0)
                            sb.Append("  &  ");
                        sb.Append("Other");
                        break;
                }
                i++;
            }
            return sb.ToString();
        }


    }
}