#pragma once

#include "D3D12CommonHeaders.h"

namespace primal::graphics::d3d12 {
	struct d3d12_frame_info;
}

namespace primal::graphics::d3d12::gpass {

	bool initialize();
	void shutdown();

	// NOTE: call this every frame before rendering anything n gpass
	void set_size(math::u32v2 size);
	void depth_prepass(id3d12_graphics_command_list* cmd_list, const d3d12_frame_info& info);
	void render(id3d12_graphics_command_list* cmd_list, const d3d12_frame_info& info);
}