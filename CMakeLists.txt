cmake_minimum_required(VERSION 3.20)
project(CrimsonEngine)

set(CMAKE_CXX_STANDARD 20)

add_library(CrimsonEngine STATIC
        src/Platform/Surface.cpp
        include/Crimson/Platform/Surface.h
        include/Crimson/Math/Size.h
        src/Graphics/Renderer.cpp
        include/Crimson/Graphics/Renderer.h
        include/Crimson/Util/Logger.cpp
        include/Crimson/Util/Logger.h
)

target_include_directories(CrimsonEngine PUBLIC include)

if (WIN32)
    add_compile_definitions(CS_WINDOWS)
elseif(LINUX)
    add_compile_definitions(CS_LINUX)
endif()

add_subdirectory(lib/SDL)
target_link_libraries(CrimsonEngine PUBLIC SDL3::SDL3)

add_subdirectory(tests)