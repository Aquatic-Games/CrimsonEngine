// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <string>
#include <source_location>
#include <format>
#include <stdexcept>

#define CS_LOG(severity, ...) Crimson::Util::Logger::Log(Crimson::Util::Logger::Severity::severity, std::format(__VA_ARGS__))
#define CS_TRACE(...) CS_LOG(Trace, __VA_ARGS__)
#define CS_DEBUG(...) CS_LOG(Debug, __VA_ARGS__)
#define CS_INFO(...) CS_LOG(Info, __VA_ARGS__)
#define CS_WARN(...) CS_LOG(Warning, __VA_ARGS__)
#define CS_ERROR(...) CS_LOG(Error, __VA_ARGS__)
#define CS_FATAL_THROW(exception, ...) {\
    CS_LOG(Fatal, __VA_ARGS__);\
    throw exception(std::format(__VA_ARGS__));\
}
#define CS_FATAL(...) CS_FATAL_THROW(std::runtime_error, __VA_ARGS__)

namespace Crimson::Util
{
    class Logger final
    {
    public:
        enum class Severity
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
            Fatal
        };

        static void Log(Severity severity, const std::string& message,
                        const std::source_location& location = std::source_location::current());
    };
}