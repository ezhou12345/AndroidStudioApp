#ifndef __BINDING_FILTERS_H__
#define __BIDNING_FILTERS_H__

#include <string>
using namespace std;

string DateTimeBindingFilter(string slotName, string slotValue);
string LocationBindingFilter(string slotName, string slotValue);
string DoorNumberBindingFilter(string slotName, string slotValue);

#endif
