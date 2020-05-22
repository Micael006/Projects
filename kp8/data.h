#ifndef DATA_H
#define DATA_H
#include <stdio.h>
#define list struct list
enum Order
{
    FIRST,
    SECOND,
    THIRD,
    FOURTH,
    FIFTH,
    SIXTH,
    SEVENTH,
    EIGHTH,
    NINTH,
    TENTH
};
typedef list
{
    list *next;
    list *prev;
    enum Order field;
}
this_cell;

#endif
