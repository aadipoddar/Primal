#pragma once

class test
{
public:
	virtual bool initialize() = 0;
	virtual void run() = 0;
	virtual void shutdown() = 0;
};