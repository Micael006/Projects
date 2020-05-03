#include <stdio.h>
#include "../data.h"
#include "re_builder.h"

info_pc *re_build(info_pc slct, info_pc *tmp)
{
    if (!tmp)
    {
        slct.last = NULL;
        slct.next = NULL;
        *tmp = slct;
        return tmp;
    }
    else
    {
        slct.last = tmp;
        *tmp->next = slct;
        return tmp->next;
    }
}
