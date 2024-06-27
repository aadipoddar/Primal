#include "D3D12GPass.h"
#include "D3D12Core.h"
#include "D3D12Shaders.h"

namespace primal::graphics::d3d12::gpass {
	namespace {

		constexpr DXGI_FORMAT	main_buffer_format{ DXGI_FORMAT_R16G16B16A16_FLOAT };
		constexpr DXGI_FORMAT	depth_buffer_format{ DXGI_FORMAT_D32_FLOAT };
		constexpr math::u32v2	initial_dimensions{ 100,100 };

		d3d12_render_texture	gpas_main_buffer{};
		d3d12_depth_buffer		gpas_depth_buffer{};
		math::u32v2				dimensions{ initial_dimensions };

		ID3D12RootSignature*	gpass_root_sig{ nullptr };
		ID3D12PipelineState*	gpass_pso{ nullptr };

#if _DEBUG
		constexpr f32 clear_value[4]{ 0.5f, 0.5f, 0.5f, 1.f };
#else
		constexpr f32 clear_value[4]{ };
#endif // _DEBUG


		bool
			create_buffers(math::u32v2 size)
		{
			assert(size.x && size.y);
			gpas_main_buffer.release();
			gpas_depth_buffer.release();

			D3D12_RESOURCE_DESC desc{};
			desc.Alignment = 0; // NOTE: 0 is the same as 64KB (or 4MB for MSAA)
			desc.DepthOrArraySize = 1;
			desc.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;
			desc.Flags = D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET;
			desc.Format = main_buffer_format;
			desc.Height = size.y;
			desc.Layout = D3D12_TEXTURE_LAYOUT_UNKNOWN;
			desc.MipLevels = 0; // make space for all mip levels
			desc.SampleDesc = { 1, 0 };
			desc.Width = size.x;

			// Create the main buffer
			{
				d3d12_texture_init_info info{};
				info.desc = &desc;
				info.initial_state = D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE;
				info.clear_value.Format = desc.Format;
				memcpy(&info.clear_value.Color, &clear_value[0], sizeof(clear_value));
				gpas_main_buffer = d3d12_render_texture{ info };
			}

			desc.Flags = D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL;
			desc.Format = depth_buffer_format;
			desc.MipLevels = 1;

			// Create the depth buffer
			{
				d3d12_texture_init_info info{};
				info.desc = &desc;
				info.initial_state = D3D12_RESOURCE_STATE_DEPTH_READ | D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE | D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE;
				info.clear_value.Format = desc.Format;
				info.clear_value.DepthStencil.Depth = 0.f;
				info.clear_value.DepthStencil.Stencil = 0;

				gpas_depth_buffer = d3d12_depth_buffer{ info };
			}

			NAME_D3D12_OBJECT(gpas_main_buffer.resource(), L"Gpass Main Buffer");
			NAME_D3D12_OBJECT(gpas_depth_buffer.resource(), L"Gpass Depth Buffer");

			return gpas_main_buffer.resource() && gpas_depth_buffer.resource();
		}

		bool
			create_gpass_pso_and_root_signature()
		{
			assert(!gpass_root_sig && !gpass_pso);

			// Craete GPass root signature
			d3dx::d3d12_root_parameter paramters[1]{};
			paramters[0].as_constants(1, D3D12_SHADER_VISIBILITY_PIXEL, 1);
			const d3dx::d3d12_root_signature_desc root_signature{ &paramters[0],_countof(paramters) };
			gpass_root_sig = root_signature.create();
			assert(gpass_root_sig);
			NAME_D3D12_OBJECT(gpass_root_sig, L"GPass Root Signature");

			// Craete GPass PSO
			struct
			{
				d3dx::d3d12_pipeline_state_subobject_root_signature			root_signature{ gpass_root_sig };
				d3dx::d3d12_pipeline_state_subobject_vs						vs{ shaders::get_engine_shader(shaders::engine_shader::fullscreen_triangle_vs) };
				d3dx::d3d12_pipeline_state_subobject_ps						ps{ shaders::get_engine_shader(shaders::engine_shader::fill_color_ps) };
				d3dx::d3d12_pipeline_state_subobject_primitive_topology		primitive_topology{ D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE };
				d3dx::d3d12_pipeline_state_subobject_render_target_formats	render_target_formats;
				d3dx::d3d12_pipeline_state_subobject_depth_stencil_format	depth_stencil_format{ depth_buffer_format };
				d3dx::d3d12_pipeline_state_subobject_rasterizer				rasterizer{ d3dx::rasterizer_state.no_cull };
				d3dx::d3d12_pipeline_state_subobject_depth_stencil1			depth{ d3dx::depth_state.disabled };
			} stream;

			D3D12_RT_FORMAT_ARRAY rtf_array{};
			rtf_array.NumRenderTargets = 1;
			rtf_array.RTFormats[0] = main_buffer_format;

			stream.render_target_formats = rtf_array;

			gpass_pso = d3dx::create_pipeline_state(&stream, sizeof(stream));
			NAME_D3D12_OBJECT(gpass_pso, L"GPass Pipeline State Object");

			return gpass_root_sig && gpass_pso;
		}

	} // anonymous namespace

	bool
		initialize()
	{
		return create_buffers(initial_dimensions) &&
			create_gpass_pso_and_root_signature();
	}

	void
		shutdown()
	{
		gpas_main_buffer.release();
		gpas_depth_buffer.release();
		dimensions = initial_dimensions;

		core::release(gpass_root_sig);
		core::release(gpass_pso);
	}

	void set_size(math::u32v2 size)
	{
		math::u32v2& d{ dimensions };
		if (size.x > d.x || size.y > d.y)
		{
			d = { std::max(size.x, d.x), std::max(size.y, d.y) };
			create_buffers(d);
		}
	}

	void depth_prepass(id3d12_graphics_command_list * cmd_list, const d3d12_frame_info & info)
	{

	}

	void render(id3d12_graphics_command_list * cmd_list, const d3d12_frame_info & info)
	{
		cmd_list->SetGraphicsRootSignature(gpass_root_sig);
		cmd_list->SetPipelineState(gpass_pso);
	}

}