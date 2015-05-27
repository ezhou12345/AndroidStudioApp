#include "ParseTree.h"
#include "StringUtil.h"
#include <string>
#include <sstream>
using namespace std;

ParseTree::ParseTree(string parseString) {
    init(istringstream(parseString));
}

ParseTree::ParseTree(istringstream& iss) {
	init(iss);
}

void ParseTree::init(istringstream& iss) {
    iss >> this->value;     // Read the value of the root node
    if (value[0] == '[') {
        _isLeaf = false;
        value = value.substr(1, value.length() - 2);
        string s;
        iss >> s;           // Skip the left parenthesis
        while (true) {
			while (iss.peek() == ' ') iss.get();
			if (iss.peek() == ')') break;
            ParseTree child(iss);
            this->children.push_back(child);
        }
        iss >> s;           // Skip the right parenthesis
    }
    else {
        _isLeaf = true;
    }
}

string ParseTree::getString() {
    if (_isLeaf) {
        return value;
    }
    else {
        string s;
        for (size_t i = 0; i < children.size(); i++) {
            if (i > 0) s.append(" ");
            s.append(children[i].getString());
        }
		return s;
    }
}

ParseTree* ParseTree::find(string path) {
// Find the node reached by following path from the root
// Path is a string of fields delimited by "."
// A field can be a slot name, or a "slotName[index]" which specifies the index-th (0-based) slot with that name,
//   or just "[index]" which matches all children
// If the first field has a slot name, the search is done recursively for the subtrees
    istringstream iss(path);
    ParseTree* node = this;
    char buffer[255];
    while (iss.getline(buffer, 255, '.')) {
        string field(buffer);
        int index;
        size_t p = field.find_first_of('[');
        if (p == string::npos) {    // no index
            index = 0;
        }
        else {
            index = atoi(field.substr(p + 1, field.length() - p - 2).c_str());
            field = field.substr(0, p);
        }
        bool found = false;
        for (size_t i = 0; i < node->children.size(); i++) {
            ParseTree* child = &node->children[i];
            if (field == "" || !child->isLeaf() && (stricmp(child->getValue(), field) == 0)) {
                    // Need to do a case-insensitive comparison, because path strings are all lower case
                if (index == 0) {
                    node = child;
                    found = true; break;
                }
                else {
                    index--;
                }
            }
        }
        if (!found) {
            node = NULL;
            break;
        }
    }
    if (node != NULL) return node;
    // Recursively check children if first field has a slot name
    if (path[0] != '[') {
        for (size_t i = 0; i < this->children.size(); i++) {
            node = this->children[i].find(path);
            if (node!= NULL) return node;
        }
    }
    return NULL;
}
