cmake_minimum_required(VERSION 4.1)
project(Crimson)

set(CMAKE_CXX_STANDARD 17)

add_library(Crimson STATIC
        Engine/include/Crimson/App.h
        Engine/include/Crimson/Core.h
        Engine/src/App.cpp
)

target_include_directories(Crimson PUBLIC Engine/include)