// Copyright (c) Microsoft. All rights reserved.

namespace Edison.Devices.Onboarding.Client.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Edison.Devices.Onboarding.Client.App());
        }
    }
}
