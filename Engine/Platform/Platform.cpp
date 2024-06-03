#include "Platform.h"
#include "PlatformTypes.h"

namespace primal::platform {

#ifdef _WIN64
	namespace {
		struct window_info
		{
			HWND    hwnd{ nullptr };
			RECT    client_area{ 0, 0, 1920, 1080 };
			RECT    fullscreen_area{};
			POINT   top_left{ 0, 0 };
			DWORD   style{ WS_VISIBLE };
			bool    is_fullscreen{ false };
			bool    is_closed{ false };
		};

		LRESULT CALLBACK internal_window_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
		{
			LONG_PTR long_ptr{ GetWindowLongPtr(hwnd,0) };
			return long_ptr
				? ((window_proc)long_ptr)(hwnd, msg, wparam, lparam)
				: DefWindowProc(hwnd, msg, wparam, lparam);
		}

	} // anonymous namespace

	window create_window(const window_init_info* const init_info)
	{
		window_proc callback{ init_info ? init_info->callback : nullptr };
		window_handle parent{ init_info ? init_info->parent : nullptr };

		// Setup a window class
		WNDCLASSEX wc;
		ZeroMemory(&wc, sizeof(wc));
		wc.cbSize = sizeof(WNDCLASSEX);
		wc.style = CS_HREDRAW | CS_VREDRAW;
		wc.lpfnWndProc = internal_window_proc;
		wc.cbClsExtra = 0;
		wc.cbWndExtra = callback ? sizeof(callback) : 0;
		wc.hInstance = 0;
		wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wc.hCursor = LoadCursor(NULL, IDC_ARROW);
		wc.hbrBackground = CreateSolidBrush(RGB(26, 48, 76));
		wc.lpszMenuName = NULL;
		wc.lpszClassName = L"PrimalWindow";
		wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);

		// Register	the window class
		RegisterClassEx(&wc);

		window_info info{};
		info.client_area.right = (init_info && init_info->width) ? info.client_area.left + init_info->width : info.client_area.right;
		info.client_area.bottom = (init_info && init_info->height) ? info.client_area.top + init_info->height : info.client_area.bottom;

		RECT rect{ info.client_area };

		// adjust the window size for correct device size
		AdjustWindowRect(&rect, info.style, FALSE);

		const wchar_t* caption{ (init_info && init_info->caption) ? init_info->caption : L"Primal Game" };
		const s32 left{ init_info ? init_info->left : info.top_left.x };
		const s32 top{ init_info ? init_info->top : info.top_left.y };
		const s32 width{ rect.right - rect.left };
		const s32 height{ rect.bottom - rect.top };

		info.style |= parent ? WS_CHILD : WS_OVERLAPPEDWINDOW;

		// Create an instance of the window class
		info.hwnd = CreateWindowEx(
			0,                // extended style
			wc.lpszClassName, // window class name
			caption,          // instance title
			info.style,       // window style
			left, top,        // initial window position
			width, height,    // initial window dimensions
			parent,           // handle to parent window
			NULL,             // handle to menu
			NULL,             // instance of this application
			NULL);            // extra creation parameters

		if (info.hwnd)
		{
		}
		return {};
	}

#elif
#error "must implement at least one platform"
#endif // _WIN64

	}