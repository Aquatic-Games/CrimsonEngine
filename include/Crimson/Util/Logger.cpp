// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#include "Logger.h"

#include <sstream>
#include <chrono>
#include <format>
#include <iostream>

namespace Crimson::Util
{
    static std::stringstream stream;

    void Logger::Log(const Severity severity, const std::string& message, const std::source_location& location)
    {
        stream.str(std::string());

        auto now = std::chrono::system_clock::now();
        stream << std::format("{:%Y-%m-%d %H:%M:%OS} ", now);

        switch (severity)
        {
            case Severity::Trace:
                stream << "[Trace] ";
                break;
            case Severity::Debug:
                stream << "[Debug] ";
                break;
            case Severity::Info:
                stream << "[Info]  ";
                break;
            case Severity::Warning:
                stream << "[Warn]  ";
                break;
            case Severity::Error:
                stream << "[Error] ";
                break;
            case Severity::Fatal:
                stream << "[Fatal] ";
                break;
        }

        stream << '(' << location.file_name() << ':' << location.line() << ") " << message;

        std::cout << stream.str() << std::endl;
    }
}
