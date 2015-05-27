#include "StringUtil.h"
#include <string>
#include <cstdarg>
using namespace std;

////////////////////////////////////////////////////////////
// String utility functions
////////////////////////////////////////////////////////////

string mysprintf(const char* format, ...) {
    char buffer[256];
    va_list args;
    va_start(args, format);
    vsprintf(buffer, format, args);
    va_end(args);
    return buffer;
}

bool startsWith(string s, string pattern) {
    return s.substr(0, pattern.length()) == pattern;
}

bool endsWith(string s, string pattern) {
    return s.length() >= pattern.length() && s.substr(s.length() - pattern.length()) == pattern;
}

string toupper(string s) {
	for (size_t i = 0; i < s.length(); i++) {
		s[i] = toupper(s[i]);
	}
	return s;
}

string tolower(string s) {
	for (size_t i = 0; i < s.length(); i++) {
		s[i] = tolower(s[i]);
	}
	return s;
}

int stricmp(string a, string b) {
    return _stricmp(a.c_str(), b.c_str());
}
