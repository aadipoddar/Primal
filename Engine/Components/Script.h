#pragma once
#include "ComponentsCommon.h"

namespace primal::script {

	struct init_info
	{
		detail::script_creator scrpt_creator;
	};

	component create(init_info info, game_entity::entity entity);
	void remove(component c);
}