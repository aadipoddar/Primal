#pragma once
#include "Test.h"

class engine_test : public test
{
public:
	bool initialize() override;
	void run() override;
	void shutdown() override;
};