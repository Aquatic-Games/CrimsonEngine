// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <string>
#include <source_location>
#include <format>
#include <stdexcept>

#define CS_LOG(severity, message, ...) Crimson::Util::Logger::Log(Crimson::Util::Logger::Severity::severity, std::format(message, __VA_ARGS__))
#define CS_TRACE(message, ...) CS_LOG(Trace, message, __VA_ARGS__)
#define CS_DEBUG(message, ...) CS_LOG(Debug, message, __VA_ARGS__)
#define CS_INFO(message, ...) CS_LOG(Info, message, __VA_ARGS__)
#define CS_WARN(message, ...) CS_LOG(Warning, message, __VA_ARGS__)
#define CS_ERROR(message, ...) CS_LOG(Error, message, __VA_ARGS__)
#define CS_FATAL_THROW(exception, message, ...) CS_LOG(Fatal, message, __VA_ARGS__);throw exception(std::format(message, __VA_ARGS__))
#define CS_FATAL(message, ...) CS_FATAL_THROW(std::runtime_error, message, __VA_ARGS__)

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