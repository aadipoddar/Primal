#pragma once
#include "D3D12CommonHeaders.h"
#include "D3D12Resources.h"

namespace primal::graphics::d3d12 {
class d3d12_surface
{
public:
    explicit d3d12_surface(platform::window window)
        : _window{ window }
    {
        assert(_window.handle());
    }
#if USE_STL_VECTOR
    DISABLE_COPY(d3d12_surface);
    constexpr d3d12_surface(d3d12_surface&& o)
        : _swap_chain{ o._swap_chain }, _window{ o._window }, _current_bb_index{ o._current_bb_index }
        , _viewport{ o._viewport }, _scissor_rect{ o._scissor_rect }, _allow_tearing{ o._allow_tearing }
        , _present_flags{ o._present_flags }
    {
        for (u32 i{ 0 }; i < frame_buffer_count; ++i)
        {
            _render_target_data[i].resource = o._render_target_data[i].resource;
            _render_target_data[i].rtv = o._render_target_data[i].rtv;
        }

        o.reset();
    }

    constexpr d3d12_surface& operator=(d3d12_surface&& o)
    {
        assert(this != &o);
        if (this != &o)
        {
            release();
            move(o);
        }

        return *this;
    }
#endif
    ~d3d12_surface() { release(); }

    void create_swap_chain(IDXGIFactory7* factory, ID3D12CommandQueue* cmd_queue, DXGI_FORMAT format);
    void present() const;
    void resize();

    constexpr u32 width() const { return (u32)_viewport.Width; }
    constexpr u32 height() const { return (u32)_viewport.Height; }
    constexpr ID3D12Resource *const back_buffer() const { return _render_target_data[_current_bb_index].resource; }
    constexpr D3D12_CPU_DESCRIPTOR_HANDLE rtv() const { return _render_target_data[_current_bb_index].rtv.cpu; }
    constexpr const D3D12_VIEWPORT& viewport() const { return _viewport; }
    constexpr const D3D12_RECT& scissor_rect() const { return _scissor_rect; }

private:
    void finalize();
    void release();

#if USE_STL_VECTOR
    constexpr void move(d3d12_surface& o)
    {
        _swap_chain = o._swap_chain;
        for (u32 i{ 0 }; i < frame_buffer_count; ++i)
        {
            _render_target_data[i] = o._render_target_data[i];
        }
        _window = o._window;
        _current_bb_index = o._current_bb_index;
        _allow_tearing = o._allow_tearing;
        _present_flags = o._present_flags;
        _viewport = o._viewport;
        _scissor_rect = o._scissor_rect;

        o.reset();
    }

    constexpr void reset()
    {
        _swap_chain = nullptr;
        for (u32 i{ 0 }; i < frame_buffer_count; ++i)
        {
            _render_target_data[i] = {};
        }
        _window = {};
        _current_bb_index = 0;
        _allow_tearing = 0;
        _present_flags = 0;
        _viewport = {};
        _scissor_rect = {};
    }
#endif

    struct render_target_data
    {
        ID3D12Resource* resource{ nullptr };
        descriptor_handle rtv{};
    };

    IDXGISwapChain4*    _swap_chain{ nullptr };
    render_target_data  _render_target_data[frame_buffer_count]{};
    platform::window    _window{};
    mutable u32         _current_bb_index{ 0 };
    u32                 _allow_tearing{ 0 };
    u32                 _present_flags{ 0 };
    D3D12_VIEWPORT      _viewport{};
    D3D12_RECT          _scissor_rect{};
};
}
