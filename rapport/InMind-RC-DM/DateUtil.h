#ifndef __DATE_UTIL_H__
#define __DATE_UTIL_H__

////////////////////////////////////////////////////////////
// Date utility functions
////////////////////////////////////////////////////////////

extern int daysInMonth[];
extern int daysInMonthCumSum[];

bool isLeap(int year);
int date2num(int year, int month, int day);
void num2date(int num, int& year, int& month, int& day);
int dayOfWeek(int year, int month, int day);
int dateDiff(int year1, int month1, int day1, int year2, int month2, int day2); // date2 - date1
void dateInc(int& year, int& month, int& day, int inc);                         // date += inc

#endif
