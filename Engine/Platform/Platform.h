#pragma once
#include "CommonHeaders.h"
#include "Window.h"

namespace primal::platform {

	struct window_init_info;

	window create_window(const window_init_info* const init_info = nullptr);
	void remove_window(window_id id);
}