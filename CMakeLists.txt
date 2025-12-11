cmake_minimum_required(VERSION 4.1)
project(Crimson)

set(CMAKE_CXX_STANDARD 17)

add_library(Crimson STATIC
        Engine/include/Crimson/App.h
        Engine/include/Crimson/Core.h
        Engine/src/App.cpp
        Engine/include/Crimson/Platform/Surface.h
        Engine/src/Platform/SDL/SDLSurface.cpp
        Engine/src/Platform/SDL/SDLSurface.h
        Engine/src/Platform/Surface.cpp
)

target_include_directories(Crimson PUBLIC Engine/include)

add_subdirectory(lib/SDL)
target_link_libraries(Crimson PUBLIC SDL3::SDL3)