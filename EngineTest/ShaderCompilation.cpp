#include "ShaderCompilation.h"
#include "Graphics/Direct3D12/D3D12Core.h"
#include "Graphics/Direct3D12/D3D12Shaders.h"

#include <dxcapi.h>
#include <d3d12shader.h>
#include <filesystem>
#include <fstream>

using namespace primal;
using namespace primal::graphics::d3d12::shaders;
using namespace Microsoft::WRL;

namespace {

	struct shader_file_info
	{
		const char*			file;
		const char*			function;
		engine_shader::id	id;
		shader_type::type	type;
	};

	constexpr shader_file_info shader_files[]
	{
		{"FullScreenTriangle.hlsl", "FullScreenTriangleVS",engine_shader::fullscreen_triangle_vs,shader_type::vertex},
	};

	static_assert(_countof(shader_files) == engine_shader::count);

	constexpr const char* shaders_source_path{ "../../Engine/Graphics/Direct3D12/Shaders/" };

	// Get the path to the compiled shaders binary file
	decltype(auto)
		get_engine_shaders_path()
	{
		return std::filesystem::absolute(graphics::get_engine_shaders_path(graphics::graphics_platform::direct3d12));
	}

	bool
		compiled_shaders_are_up_to_date()
	{
		auto engine_shaders_path = get_engine_shaders_path();
		if (!std::filesystem::exists(engine_shaders_path)) return false;
		auto shaders_compilation_time = std::filesystem::last_write_time(engine_shaders_path);

		std::filesystem::path path{};
		std::filesystem::path full_path{};

		// Check if either of engine shader source file is newer than the compiled shader file
		// In that case, we need to recompile
		for (u32 i{ 0 }; i < engine_shader::count; ++i)
		{
			auto& info = shader_files[i];

			path = shaders_source_path;
			path += info.file;
			full_path = std::filesystem::absolute(path);
			if (!std::filesystem::exists(full_path)) return false;

			auto shader_file_time = std::filesystem::last_write_time(full_path);
			if (shader_file_time > shaders_compilation_time)
			{
				return false;
			}
		}

		return true;
	}

	bool
		save_compiled_shaders(utl::vector<ComPtr<IDxcBlob>>& shaders)
	{
		auto engine_shaders_path = get_engine_shaders_path();
		std::filesystem::create_directories(engine_shaders_path.parent_path());
		std::ofstream file(engine_shaders_path, std::ios::out | std::ios::binary);
		if (!file || !std::filesystem::exists(engine_shaders_path))
		{
			file.close();
			return false;
		}

		for (auto& shader : shaders)
		{
			const D3D12_SHADER_BYTECODE byte_code{ shader->GetBufferPointer(), shader->GetBufferSize() };
			file.write((char*)&byte_code.BytecodeLength, sizeof(byte_code.BytecodeLength));
			file.write((char*)byte_code.pShaderBytecode, byte_code.BytecodeLength);
		}

		file.close();
		return true;
	}

} // anonymous namespace

bool compile_shaders()
{
	if (compiled_shaders_are_up_to_date()) return true;
	utl::vector<ComPtr<IDxcBlob>> shaders;
	std::filesystem::path path{};
	std::filesystem::path full_path{};

	// compile shaders and put them together in a buffer in the same order of compilation
	for (u32 i{ 0 }; i < engine_shader::count; ++i)
	{
		auto& info = shader_files[i];

		path = shaders_source_path;
		path += info.file;
		full_path = std::filesystem::absolute(path);
		if (!std::filesystem::exists(full_path)) return false;
		ComPtr<IDxcBlob> compiled_shader{/* call compile shader function */ };
		if (compiled_shader->GetBufferPointer() && compiled_shader->GetBufferSize())
		{
			shaders.emplace_back(std::move(compiled_shader));
		}
		else
		{
			return false;
		}
	}
	return save_compiled_shaders(shaders);
}
