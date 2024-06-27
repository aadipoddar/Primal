#pragma once
#include "D3D12CommonHeaders.h"

namespace primal::graphics::d3d12::fx {

	bool initialize();
	void shutdown();

	void post_process(id3d12_graphics_command_list* cmd_list, D3D12_CPU_DESCRIPTOR_HANDLE target_rtv);

}