// Copyright (c) Aquatic Games 2025. All rights reserved.
// This file is licensed to you under the MIT license.

#pragma once

#include <string>
#include <memory>

#include "Core.h"

namespace Crimson
{
    CS_EXPORT inline std::unique_ptr<class App> GApp = nullptr;

    struct AppInfo
    {
        std::string AppName;
        std::string AppVersion;
    };

    class App
    {
        std::string _name;
        std::string _version;

        explicit App(const AppInfo& info);

    public:
        [[nodiscard]] std::string Name() const
        {
            return _name;
        }

        [[nodiscard]] std::string Version() const
        {
            return _version;
        }

        static void Run(const AppInfo& info);
    };
}