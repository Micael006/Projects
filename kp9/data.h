#ifndef DATA_H
#define DATA_H
#include <stdio.h>
#define cell struct cell
typedef struct
{
    float key;
    int index;
} t_table;

typedef cell
{
    cell *left;
    cell *right;
    t_table table; 
}
this_cell;

#endif
