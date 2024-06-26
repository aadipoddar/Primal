#pragma once
#include "CommonHeaders.h"
#include "Renderer.h"
#include "Platform\Window.h"

namespace primal::graphics {
struct platform_interface
{
    bool(*initialize)(void);
    void(*shutdown)(void);

    struct {
        surface(*create)(platform::window);
        void(*remove)(surface_id);
        void(*resize)(surface_id, u32, u32);
        u32(*width)(surface_id);
        u32(*height)(surface_id);
        void(*render)(surface_id);
    } surface;

    graphics_platform platform = (graphics_platform)-1;
};
}
