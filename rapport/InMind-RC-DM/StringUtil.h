#ifndef __STRING_UTIL_H__
#define __STRING_UTIL_H__

#include <string>
using namespace std;

////////////////////////////////////////////////////////////
// String utility functions
////////////////////////////////////////////////////////////

string mysprintf(const char* format, ...);

bool startsWith(string s, string pattern);
bool endsWith(string s, string pattern);

string toupper(string s);
string tolower(string s);
int stricmp(string a, string b);

#endif
