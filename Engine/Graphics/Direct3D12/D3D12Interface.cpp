#include "CommonHeaders.h"
#include "D3D12Interface.h"
#include "Graphics/GraphicsPlatformInterface.h"

namespace primal::graphics::d3d12 {

	void
		get_platform_interface(platform_interface& pi)
	{
		pi.initialize = core::initialize;
		pi.shutdown = core::shutdown;
	}
}
