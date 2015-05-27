#include "DialogTask.h"
#include "Constants.h"
#include "DateUtil.h"
#include "StringUtil.h"
#include "ParseTree.h"
#include "BindingFilters.h"
#include <string>
#include <sstream>
#include <map>
#include <ctime>
using namespace std;

////////////////////////////////////////////////////////////
// Interpreting basic units
////////////////////////////////////////////////////////////

string onesCardinal[] = {"ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",
                         "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"};
string tensCardinal[] = {"ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"};
string onesOrdinal[] = {"ZEROTH", "FIRST", "SECOND", "THIRD", "FOURTH", "FIFTH", "SIXTH", "SEVENTH", "EIGHTH", "NINTH",
                         "TENTH", "ELEVENTH", "TWELFTH", "THIRTEENTH", "FOURTEENTH", "FIFTEENTH", "SIXTEENTH", "SEVENTEENTH" ,"EIGHTEENTH", "NINETEENTH"};
string tensOrdinal[] = {"ZEROTH", "TENTH", "TWENTIETH", "THIRTIETH", "FORTIETH", "FIFTIETH", "SIXTIETH", "SEVENTIETH", "EIGHTIETH", "NINETIETH"};
string months[] = {"JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER"};
string daysOfWeek[] = {"SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"};

map<string, int> mapDigit() {
    map<string, int> m;
    for (int i = 0; i < 10; i++) {
        m[onesCardinal[i]] = i;
    }
    m["OH"] = 0;
    return m;
}

map<string, int> mapCardinal() {
    map<string, int> m;
    for (int i = 0; i < 20; i++) {
        m[onesCardinal[i]] = i;
    }
    for (int i = 2; i < 10; i++) {
        m[tensCardinal[i]] = i * 10;
        for (int j = 1; j < 10; j++) {
            m[tensCardinal[i] + " " + onesCardinal[j]] = i * 10 + j;
        }
    }
    return m;
}

map<string, int> mapOrdinal() {
    map<string, int> m;
    for (int i = 0; i < 20; i++) {
        m[onesOrdinal[i]] = i;
    }
    for (int i = 2; i < 10; i++) {
        m[tensOrdinal[i]] = i * 10;
        for (int j = 1; j < 10; j++) {
            m[tensCardinal[i] + " " + onesOrdinal[j]] = i * 10 + j;
        }
    }
    return m;
}

map<string, int> mapMonth() {
    map<string, int> m;
    for (int i = 1; i <= 12; i++) {
        m[months[i-1]] = i;
    }
    return m;
}

map<string, int> mapDayOfWeek() {
    map<string, int> m;
    for (int i = 0; i < 7; i++) {
        m[daysOfWeek[i]] = i;
    }
    return m;
}

int InterpretDigit(string s) {
    static map<string, int>& m = mapDigit();
    return m[s];
}

int InterpretCardinal(string s) {     // only deals with 0 to 99
    static map<string, int>& m = mapCardinal();
    return m[s];
}

int InterpretOrdinal(string s) {     // only deals with 0 to 99
    static map<string, int>& m = mapOrdinal();
    return m[s];
}

int InterpretMonth(string s) {
    static map<string, int>& m = mapMonth();
    return m[s];
}

int InterpretDayOfWeek(string s) {
    static map<string, int>& m = mapDayOfWeek();
    return m[s];
}

////////////////////////////////////////////////////////////
// Canonicalization for more complicated things
////////////////////////////////////////////////////////////

int CanonicalizeDate(ParseTree& tree, int& year, int& month, int& day) {     // Returns status
    ParseTree* node;
    string s;

    bool ambiguous = false;
    bool inconsistent = false;
    bool past = false;

    if ((node = tree.find("Day")) != NULL) {         // Date specified by (month and) date
                                                     // Only date -- this month or next month
                                                     // Month and date -- if specified month is current month, it's in this year (may result in past date)
                                                     //                -- if specified month is not current month, it's the next coming one (may be next year)
        // Parse the day, don't save it yet
        int tempDay = InterpretOrdinal(node->getString());
        // Parse the month
        node = tree.find("Month");
        if (node != NULL) {     // Month specified
            string s = node->getString();
            if (s == "THIS MONTH") {
                past = (tempDay < day);
            }
            else if (s == "NEXT MONTH") {
                month++;
                if (month > 12) {year++; month = 1;}
            }
            else {
                int tempMonth = InterpretMonth(s);
                if (tempMonth == month) past = (tempDay < day);
                if (tempMonth < month) year++;
                month = tempMonth;
            }
        }
        // Save the day
        if (node == NULL && tempDay < day) {        // Month not specified, but day is earlier: go to next month
            month++;
            if (month > 12) {month = 1; year++;}
        }
        day = tempDay;
        // Check for inconsistency
        bool leap = isLeap(year);
        int maxDay = daysInMonth[month] + (month == 2 && leap);
        if (day > maxDay) {
            inconsistent = true;
        }
    }
    else if ((node = tree.find("DayOfWeek")) != NULL) {  // Date specified by day of week
        int dayOfWeekToday = dayOfWeek(year, month, day);
        int dayOfWeekGiven = InterpretDayOfWeek(node->getString());
        int inc;
        string s = tree.getString();
        if (startsWith(s, "THIS COMING")) {  // This coming ***day
            // Find the first ***day that's later than today. This is never ambiguous.
            inc = dayOfWeekGiven - dayOfWeekToday;
            if (inc <= 0) inc += 7;
        }
        else if (startsWith(s, "THIS")) {    // This ***day
            // Understand as "in this week" (may result in a past date)
            // If either today or the desired day is Sunday, treat as "this coming".
            // If both are Sundays, treat as ambiguous.
            if (dayOfWeekGiven != 0) {
                inc = dayOfWeekGiven - dayOfWeekToday;
            }
            else if (dayOfWeekToday != 0) {
                inc = 7 - dayOfWeekToday;
            }
            else {
                inc = 0;
                ambiguous = true;
            }
        }
        else if (startsWith(s, "NEXT")) {    // Next ***day
            // If one (not both) of today or the desired day is Sunday, treat as ambiguous
            // If today is Monday and we're talking about Saturday, treat as ambiguous, too
            inc = dayOfWeekGiven - dayOfWeekToday + 7;
            if (dayOfWeekToday == 0 && dayOfWeekGiven != 0 ||
                dayOfWeekToday == 1 && dayOfWeekGiven == 6) {inc -= 7; ambiguous = true;}
            if (dayOfWeekToday != 0 && dayOfWeekGiven == 0) {ambiguous = true;}
        }
        else {                                  // No qualifier
            // Same as "this coming", but if dayOfWeekToday == dayOfWeekGiven, treat as ambiguous
            inc = dayOfWeekGiven - dayOfWeekToday;
            if (inc == 0) {
                ambiguous = true;
            }
            else if (inc <= 0) {
                inc += 7;
            }
        }
        dateInc(year, month, day, inc);
        if (inc < 0) past = true;
    }
    else {      // Special words or phrases
        string s = tree.getString();
        if (s == "TODAY" || s == "TONIGHT" || startsWith(s, "THIS")) {}
            else if (s == "TOMORROW" || startsWith(s, "NEXT")) {dateInc(year, month, day, 1);}
            else if (s == "THE DAY AFTER TOMORROW") {dateInc(year, month, day, 2);}
    }

    if (inconsistent)
        return DT_INCONSISTENT;
    else if (ambiguous)
        return DT_AMBIGUOUS;
    else if (past)
        return DT_PAST;
    else
        return DT_OKAY;
}

int CanonicalizeTime(ParseTree& tree, int& period, int& hour, int& minute) {     // Returns status
    ParseTree* node;
    string s;

    // Parse the hour
    node = tree.find("Hour");
    s = node->getString();
    if (endsWith(s, " HOUR")) s.erase(s.length() - 5);
    if (endsWith(s, " HOURS")) s.erase(s.length() - 6);
    hour = InterpretCardinal(s);
    bool is24hr = !(hour >= 1 && hour <= 12);

    // Parse the minute
    node = tree.find("Minute");
    s = (node == NULL) ? "" : node->getString();
    if (s.find("QUARTER") != string::npos)
        minute = 15;
    else if (s.find("HALF") != string::npos)
        minute = 30;
    else {
        if (startsWith(s, "OH ")) s.erase(0, 3);
        if (endsWith(s, " MINUTE")) s.erase(s.length() - 7);
        if (endsWith(s, " MINUTES")) s.erase(s.length() - 8);
        minute = InterpretCardinal(s);
    }
    node = tree.find("[1]");
    if (node != NULL && node->getString() == "TO") minute = -minute;    // minutes to hours

    // Check for ambiguity and consistency
    bool ambiguous = false;
    bool inconsistent = false;
    if (hour == 0 && minute < 0 || hour == 24 && minute > 0) {
        inconsistent = true;
        goto finish_canonicalize_time;
    }
    if (minute < 0) {
        hour--; minute += 60;
        if (hour == 11 && (period == PoD_NONE || period == PoD_AM || period == PoD_PM)) {       // xx to twelve (am / pm), can be ambiguous
            ambiguous = true;
            goto finish_canonicalize_time;
        }
    }
    if (is24hr) {   // Mainly check for consistency. Actually, when using 24hr notation, no period should be specified
        if (period == PoD_AM && hour >= 12) inconsistent = true;
        if (period == PoD_PM && hour < 12) inconsistent = true;
        if (period == PoD_MORNING && hour >= 12) inconsistent = true;
        if (period == PoD_NOON && (hour < 10 || hour >= 15)) inconsistent = true;
        if (period == PoD_AFTERNOON && hour < 12) inconsistent = true;
        if (period == PoD_EVENING && hour < 16) inconsistent = true;
        if (period == PoD_NIGHT && (hour >= 6 && hour < 18)) inconsistent = true;
    }
    else {          // Mainly check for ambiguity
        if (hour == 12) hour = 0;
        if (period == PoD_NONE) {ambiguous = true;}
        if (period == PoD_MORNING || period == PoD_AM) {}
        if (period == PoD_NOON) {if (hour < 3) hour += 12; else if (hour < 10) inconsistent = true;}
        if (period == PoD_AFTERNOON || period == PoD_PM) hour += 12;
        if (period == PoD_EVENING) {if (hour < 4) inconsistent = true; else hour += 12;}
        if (period == PoD_NIGHT) {if (hour >= 6) hour += 12;}
    }

finish_canonicalize_time:
    if (inconsistent)
        return DT_INCONSISTENT;
    else if (ambiguous)
        return DT_AMBIGUOUS;
    else
        return DT_OKAY;
}

int CanonicalizePeriodOfDay(ParseTree& period) {            // Returns an integer standing for the period of day
    string s = period.getString();
    if (s.find("MORNING") != string::npos) return PoD_MORNING;
    if (s.find("AFTERNOON") != string::npos) return PoD_AFTERNOON;
    if (s.find("NOON") != string::npos) return PoD_NOON;            // NOON must come after AFTERNOON because it's a substring
    if (s.find("EVENING") != string::npos) return PoD_EVENING;
    if (s.find("NIGHT") != string::npos) return PoD_NIGHT;
    if (s.find("A M") != string::npos) return PoD_AM;
    if (s.find("P M") != string::npos) return PoD_PM;
    return PoD_NONE;    // This shouldn't be reached
}

string CanonicalizeDateTime(ParseTree& tree) {          // Returns DateTime struct in string form
    time_t rawtime;
    time(&rawtime);
    tm now;
    localtime_s(&now, &rawtime);                    // Get the current time
    int year = now.tm_year + 1900, month = now.tm_mon + 1, day = now.tm_mday,
        hour = now.tm_hour, minute = now.tm_min,    // Initialize to current time
        period_of_day = PoD_NONE, date_status = DT_OKAY, time_status = DT_OKAY;

    if (tree.getString() != "NOW") {                // Yes, people can say "now"!
        ParseTree* date = tree.find("Date");
        ParseTree* time = tree.find("Time");
        ParseTree* period = tree.find("PeriodOfDay");
        if (date == NULL) date = tree.find("DandP");
        if (period == NULL) period = tree.find("DandP");

        date_status = (date == NULL) ? DT_MISSING : CanonicalizeDate(*date, year, month, day);
        period_of_day = (period == NULL) ? PoD_NONE : CanonicalizePeriodOfDay(*period);
        time_status = (time == NULL) ? DT_MISSING : CanonicalizeTime(*time, period_of_day, hour, minute);
    }

    stringstream s;
    s << "{\n";
    s << "year\t" << year << "\n";
    s << "month\t" << month << "\n";
    s << "day\t" << day << "\n";
    s << "hour\t" << hour << "\n";
    s << "minute\t" << minute << "\n";
    s << "period_of_day\t" << period_of_day << "\n";
    s << "date_status\t" << date_status << "\n";
    s << "time_status\t" << time_status << "\n";
    s << "}\n";
    return s.str();
}

string CanonicalizeDoorNumber(ParseTree& tree) {
    if (tree.getValue() != "DoorNumber") return LOC_NONE;   // People can say "no"
    istringstream iss(tree.getString());
    stringstream oss;
    string token;
    while (iss >> token) {
        oss << InterpretDigit(token);
    }
    return oss.str();
}

string CanonicalizeLocation(ParseTree& tree) {
    string building = LOC_NONE;
    string door_number = LOC_NONE;
    int digits = 0;     // Number of digits of door number of this building

    if (tree.getValue() == "Location") {        // People can say "no"
        ParseTree* node;
        node = tree.find("Building");
        building = toupper(node->find("[0]")->getValue());

        if (building == "GHC") digits = 4;
            else if (building == "NSH") digits = 4;
            else if (building == "WEAN") digits = 4;
            else if (building == "UC") digits = 3;

        if (digits > 0) {       // Need door number
            door_number = LOC_MISSING;
            node = tree.find("DoorNumber");
            if (node != NULL) {
                string s = CanonicalizeDoorNumber(*node);
                if (s.length() == digits) door_number = s;
            }
        }
    }

    stringstream s;
    s << "{\n";
    s << "building\t" << building << "\n";
    s << "door_number\t" << door_number << "\n";
    s << "digits\t" << digits << "\n";
    s << "}\n";
    return s.str();
}

////////////////////////////////////////////////////////////
// Binding filters
////////////////////////////////////////////////////////////

string GeneralBindingFilter(string slotName, string (*filter)(ParseTree&)) {
    static string FAIL = "{\n}\n";
    // Get the last parse string
	CInteractionEvent *lastInputEvent = pInteractionEventManager->GetLastInput();
	if (lastInputEvent == NULL || pInteractionEventManager->GetLastEvent()->GetType() == IET_GUI) return FAIL;
    string parseString = lastInputEvent->GetStringProperty("[parse_str]");
    if (parseString == "") return FAIL;
    // Construct a parse tree
    ParseTree tree("[ROOT] ( " + parseString.substr(parseString.find(" ")) + " )");
    // Find the node corresponding to the slotName
    ParseTree* node = tree.find(slotName.substr(1, slotName.length() - 2)); // Strip the brackets from slotName
    // Pass that node to the binding filter
    if (node == NULL) return FAIL; else return filter(*node);
}

string DateTimeBindingFilter(string slotName, string slotValue) {
    return GeneralBindingFilter(slotName, CanonicalizeDateTime);
}

string LocationBindingFilter(string slotName, string slotValue) {
    return GeneralBindingFilter(slotName, CanonicalizeLocation);
}

string DoorNumberBindingFilter(string slotName, string slotValue) {
    return GeneralBindingFilter(slotName, CanonicalizeDoorNumber);
}
