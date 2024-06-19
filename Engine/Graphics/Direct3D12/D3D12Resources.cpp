#include "D3D12Resources.h"
#include "D3D12Core.h"

namespace primal::graphics::d3d12 {
	//// DESCRIPTOR HEAP //////////////////////////////////////////////////////////////////////////////
	bool descriptor_heap::initialize(u32 capacity, bool is_shader_visible)
	{
		std::lock_guard lock{ _mutex };
		assert(capacity && capacity < D3D12_MAX_SHADER_VISIBLE_DESCRIPTOR_HEAP_SIZE_TIER_2);
		assert(!(_type == D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER &&
				 capacity > D3D12_MAX_SHADER_VISIBLE_SAMPLER_HEAP_SIZE));
		if (_type == D3D12_DESCRIPTOR_HEAP_TYPE_DSV ||
			_type == D3D12_DESCRIPTOR_HEAP_TYPE_RTV)
		{
			is_shader_visible = false;
		}

		release();

		ID3D12Device *const device{ core::device() };
		assert(device);

		D3D12_DESCRIPTOR_HEAP_DESC desc{};
		desc.Flags = is_shader_visible
			? D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE
			: D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
		desc.NumDescriptors = capacity;
		desc.Type = _type;
		desc.NodeMask = 0;

		HRESULT hr{ S_OK };
		DXCall(hr = device->CreateDescriptorHeap(&desc, IID_PPV_ARGS(&_heap)));
		if (FAILED(hr)) return false;

		_free_handles = std::move(std::make_unique<u32[]>(capacity));
		_capacity = capacity;
		_size = 0;

		for (u32 i{ 0 }; i < capacity; ++i) _free_handles[i] = i;

		_descriptor_size = device->GetDescriptorHandleIncrementSize(_type);
		_cpu_start = _heap->GetCPUDescriptorHandleForHeapStart();
		_gpu_start = is_shader_visible ?
			_heap->GetGPUDescriptorHandleForHeapStart() : D3D12_GPU_DESCRIPTOR_HANDLE{ 0 };

		return true;
	}

	void
		descriptor_heap::release()
	{
	}

	descriptor_handle descriptor_heap::allocate()
	{
		std::lock_guard lock{ _mutex };
		assert(_heap);
		assert(_size < _capacity);

		const u32 index{ _free_handles[_size] };
		const u32 offset{ index * _descriptor_size };
		++_size;

		descriptor_handle handle;
		handle.cpu.ptr = _cpu_start.ptr + offset;
		if (is_shader_visible())
		{
			handle.gpu.ptr = _gpu_start.ptr + offset;
		}

		DEBUG_OP(handle.container = this);
		DEBUG_OP(handle.index = index);
		return handle;
	}

	void
		descriptor_heap::free(descriptor_handle & handle)
	{
		if (handle.is_valid()) return;
		std::lock_guard lock{ _mutex };
		assert(_heap && _size);
		assert(handle.container == this);
		assert(handle.cpu.ptr >= _cpu_start.ptr);
		assert((handle.cpu.ptr - _cpu_start.ptr) % _descriptor_size == 0);
		assert(handle.index < _capacity);
		const u32 index{ (u32)(handle.cpu.ptr - _cpu_start.ptr) / _descriptor_size };
		assert(handle.index == index);

		handle = {};
	}

}
