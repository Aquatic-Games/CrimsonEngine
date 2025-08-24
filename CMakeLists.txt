cmake_minimum_required(VERSION 3.20)
project(CrimsonEngine)

set(CMAKE_CXX_STANDARD 20)

file(GLOB CRIMSON_SRC CONFIGURE_DEPENDS
        include/Crimson/Graphics/*.h
        include/Crimson/Math/*.h
        include/Crimson/Platform/*.h
        include/Crimson/Util/*.h

        src/Graphics/*.h
        src/Graphics/*.cpp
        src/Graphics/Renderers/*.h
        src/Graphics/Renderers/*.cpp
        src/Platform/*.h
        src/Platform/*.cpp
        src/Util/*.h
        src/Util/*.cpp
)

add_library(CrimsonEngine STATIC ${CRIMSON_SRC})

target_include_directories(CrimsonEngine PUBLIC include)

if (WIN32)
    add_compile_definitions(CS_WINDOWS)
elseif(LINUX)
    add_compile_definitions(CS_LINUX)
endif()

add_subdirectory(lib/SDL)
target_link_libraries(CrimsonEngine PUBLIC SDL3::SDL3)

add_subdirectory(tests)