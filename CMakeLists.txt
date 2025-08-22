cmake_minimum_required(VERSION 3.20)
project(CrimsonEngine)

set(CMAKE_CXX_STANDARD 20)

add_library(CrimsonEngine STATIC library.cpp
        src/Platform/Surface.cpp
        include/Crimson/Platform/Surface.h
        include/Crimson/Math/Size.h
        src/Graphics/Renderer.cpp
        include/Crimson/Graphics/Renderer.h
)

target_include_directories(CrimsonEngine PUBLIC include)

add_subdirectory(lib/SDL)
target_link_libraries(CrimsonEngine PUBLIC SDL3::SDL3)

add_subdirectory(tests)