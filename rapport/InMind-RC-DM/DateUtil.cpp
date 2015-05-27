#include "DateUtil.h"

////////////////////////////////////////////////////////////
// Date utility functions
////////////////////////////////////////////////////////////

int daysInMonth[] = {0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
int daysInMonthCumSum[] = {0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};

bool isLeap(int year) {
    return (year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0);
}

int date2num(int year, int month, int day) {    // Jan 01, 0001 is converted to 1
    return (year - 1) * 365 + (year - 1) / 4 - (year - 1) / 100 + (year - 1) / 400
         + daysInMonthCumSum[month - 1] + (isLeap(year) && month > 2) + day;
}

void num2date(int num, int& year, int& month, int& day) {
    static int daysIn400Years = 146097;
    static int daysIn100Years = 36524;
    static int daysIn4Years = 1461;
    static int daysIn1Year = 365;

    num--; year = 1;
    year += num / daysIn400Years * 400; num %= daysIn400Years;
    year += num / daysIn100Years * 100; num %= daysIn100Years;
    year += num / daysIn4Years * 4; num %= daysIn4Years;
    year += num / daysIn1Year; num %= daysIn1Year;

    for (month = 1; month <= 12; month++) {
        int daysInThisMonth = daysInMonth[month] + (month == 2 && isLeap(year));
        if (num < daysInThisMonth) break;
        num -= daysInThisMonth;
    }
    day = num + 1;
}

int dayOfWeek(int year, int month, int day) {
    return date2num(year, month, day) % 7;   // Jan 01, 0001 is Monday
}

int dateDiff(int year1, int month1, int day1, int year2, int month2, int day2) {    // date2 - date1
    return date2num(year2, month2, day2) - date2num(year1, month1, day1);
}

void dateInc(int& year, int& month, int& day, int inc)  {   // date += inc
    int num = date2num(year, month, day) + inc;
    num2date(num, year, month, day);
}
