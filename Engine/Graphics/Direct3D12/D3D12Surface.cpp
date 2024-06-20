#include "D3D12Surface.h"
#include "D3D12Core.h"

namespace primal::graphics::d3d12 {
	namespace {

		constexpr DXGI_FORMAT
			to_non_srgb(DXGI_FORMAT format)
		{
			if (format == DXGI_FORMAT_R8G8B8A8_UNORM_SRGB) return DXGI_FORMAT_R8G8B8A8_UNORM;

			return format;
		}

	} // anonymous namespace

	void
		d3d12_surface::create_swap_chain(IDXGIFactory7 * factory, ID3D12CommandQueue* cmd_queue, DXGI_FORMAT format)
	{
		assert(factory && cmd_queue);
		release();

		DXGI_SWAP_CHAIN_DESC1 desc{};
		desc.AlphaMode = DXGI_ALPHA_MODE_UNSPECIFIED;
		desc.BufferCount = frame_buffer_count;
		desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		desc.Flags = 0;
		desc.Format = to_non_srgb(format);
		desc.Height = _window.height();
		desc.Width = _window.width();
		desc.SampleDesc.Count = 1;
		desc.SampleDesc.Quality = 0;
		desc.Scaling = DXGI_SCALING_STRETCH;
		desc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
		desc.Stereo = false;

		IDXGISwapChain1* swap_chain;
		HWND hwnd{ (HWND)_window.handle() };
		DXCall(factory->CreateSwapChainForHwnd(cmd_queue, hwnd, &desc, nullptr, nullptr, &swap_chain));
		DXCall(factory->MakeWindowAssociation(hwnd, DXGI_MWA_NO_ALT_ENTER));
		DXCall(swap_chain->QueryInterface(IID_PPV_ARGS(&swap_chain)));
		core::release(swap_chain);

		_current_bb_index = _swap_chain->GetCurrentBackBufferIndex();

		for (u32 i{ 0 }; i < frame_buffer_count; ++i)
		{
			_render_target_data[i].rtv = core::rtv_heap().allocate();
		}

		finalize();
	}

	void
		d3d12_surface::finalize()
	{
		// create RTVs for back buffers
		for (u32 i{ 0 }; i < frame_buffer_count; ++i)
		{
			render_target_data& data{ _render_target_data[i] };
			assert(!data.resource);
			DXCall(_swap_chain->GetBuffer(i, IID_PPV_ARGS(&data.resource)));
			D3D12_RENDER_TARGET_VIEW_DESC desc{};
			desc.Format = core::default_render_target_format();
			desc.ViewDimension = D3D12_RTV_DIMENSION_TEXTURE2D;
			core::device()->CreateRenderTargetView(data.resource, &desc, data.rtv.cpu);
		}

		DXGI_SWAP_CHAIN_DESC desc{};
		DXCall(_swap_chain->GetDesc(&desc));
		const u32 width{ desc.BufferDesc.Width };
		const u32 height{ desc.BufferDesc.Height };
		assert(_window.width() == width && _window.height() == height);

		// set viewport and scissor rect
		_viewport.TopLeftX = 0.f;
		_viewport.TopLeftY = 0.f;
		_viewport.Width = (float)width;
		_viewport.Height = (float)height;
		_viewport.MinDepth = 0.f;
		_viewport.MaxDepth = 1.f;

		_scissor_rect = { 0, 0, (s32)width, (s32)height };
	}

	void
		d3d12_surface::present() const
	{
		assert(_swap_chain);
		assert(_swap_chain->Present(0, 0));
		_current_bb_index = _swap_chain->GetCurrentBackBufferIndex();
	}

	void d3d12_surface::resize()
	{
	}

	void
		d3d12_surface::release()
	{
		for (u32 i{ 0 }; i < frame_buffer_count; ++i)
		{
			render_target_data& data{ _render_target_data[i] };
			core::release(data.resource);
			core::rtv_heap().free(data.rtv);
		}

		core::release(_swap_chain);
	}
}
