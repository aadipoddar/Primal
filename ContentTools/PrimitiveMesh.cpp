#include "PrimitiveMesh.h"
#include "Geometry.h"

namespace primal::tools {
	namespace {

		using primitive_mesh_creator = void(*)(scene&, const primitive_init_info& info);

		void create_plane(scene& scene, const primitive_init_info& info);
		void create_cube(scene& scene, const primitive_init_info& info);
		void create_uv_sphere(scene& scene, const primitive_init_info& info);
		void create_ico_sphere(scene& scene, const primitive_init_info& info);
		void create_cylinder(scene& scene, const primitive_init_info& info);
		void create_capsule(scene& scene, const primitive_init_info& info);

		primitive_mesh_creator creators[primitive_mesh_type::count]
		{
			create_plane,
			create_cube,
			create_uv_sphere,
			create_ico_sphere,
			create_cylinder,
			create_capsule,
		};

		static_assert(_countof(creators) == primitive_mesh_type::count);

		void create_plane(scene& scene, const primitive_init_info& info)
		{
		}

		void create_cube(scene& scene, const primitive_init_info& info)
		{
		}

		void create_uv_sphere(scene& scene, const primitive_init_info& info)
		{
		}

		void create_ico_sphere(scene& scene, const primitive_init_info& info)
		{
		}

		void create_cylinder(scene& scene, const primitive_init_info& info)
		{
		}

		void create_capsule(scene& scene, const primitive_init_info& info)
		{
		}

	} // anonymous namespace

	EDITOR_INTERFACE void
		CreatePrimitiveMesh(scene_data* data, primitive_init_info* info)
	{
		assert(data && info);
		assert(info->type < primitive_mesh_type::count);
		scene scene{};
	}
}