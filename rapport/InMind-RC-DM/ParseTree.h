#ifndef __PARSE_TREE_H__
#define __PARSE_TREE_H__

#include <string>
#include <vector>
using namespace std;

class ParseTree {                       // Can be understood as either a parse tree or a node
public:
    ParseTree(string);                  // Construct a ParseTree from a string
    bool isLeaf() {return _isLeaf;}
    string getValue() {return value;}   // Return the non-terminal symbol or terminal word of the root of the ParseTree
    string getString();                 // Return the entire string spanned by this ParseTree
    ParseTree* find(string);            // Find specified node in the ParseTree
private:
	ParseTree(istringstream&);			// Construct a ParseTree from an istringstream
    void init(istringstream&);			// Private init function called by both constructors
private:
    bool _isLeaf;
    string value;
    vector<ParseTree> children;
};

#endif
