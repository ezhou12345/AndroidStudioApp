#ifndef __CONSTANTS_H__
#define __CONSTANTS_H__

// Task types
#define TASK_FIND_PERSON             1
#define TASK_FIND_PLACE              2
#define TASK_GOODBYE              3

// Query Types
#define QUERY_WHERE				1
#define QUERY_HOW				2
#define QUERY_WHICH				3

// Periods of Day
#define PoD_NONE                0
#define PoD_AM                  1
#define PoD_PM                  2
#define PoD_MORNING             3
#define PoD_NOON                4
#define PoD_AFTERNOON           5
#define PoD_EVENING             6
#define PoD_NIGHT               7

// Status of dates and times
#define DT_OKAY                 0
#define DT_MISSING              1
#define DT_AMBIGUOUS            2
#define DT_INCONSISTENT         3
#define DT_PAST                 4
#define DT_WRONG_ORDER          5

// Locations
#define LOC_NONE                "-"
#define LOC_MISSING             "?"

#endif
