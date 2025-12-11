// Copyright (c) Aquatic Games 2025. All rights reserved.
// This file is licensed to you under the MIT license.

#include "Crimson/App.h"

namespace Crimson
{
    App::App(const AppInfo& info)
    {
        _name = info.AppName;
        _version = info.AppVersion;
    }

    void App::AppRun()
    {

    }

    void App::Run(const AppInfo& info)
    {
        auto app = std::unique_ptr<App>(new App(info));
        if (GApp == nullptr)
            GApp = std::move(app);

        GApp->AppRun();
    }
}
