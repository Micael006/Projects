#ifndef FUNCTION_H
#define FUNCTION_H
#include "../data.h"

list * create(enum Order);
list * add(list *, enum Order);
list * destroy(list *);
list * expel(list *);
int listlength(list *);
list * removetail(list *, int);
void listprint(list *);
list * expelhead(list *);

#endif


