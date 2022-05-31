#pragma comment(lib, "engine.lib")


#define TEST_ENTITY_COMPONENTS 1


#if TEST_ENTITY_COMPONENTS

#include "TestEntityComponent.h"

#else
#error One of the tests need to be enabled

#endif // TEST_ENTITY_COMPONENTS



int main()
{

#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

#endif // _DEBUG

	engine_test test{};

	if (test.initialize())
	{
		test.run();
	}


	test.shutdown();
}