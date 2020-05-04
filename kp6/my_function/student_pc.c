#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>
#include "../data.h"
#include "../output/output.h"
#include "../re_builder/re_builder.h"
void student_pc(info_pc *tmp)
{
    while(tmp)
    {   
        if((tmp->hdd.count == 0) || ((tmp->ctrl.discrete > 2) && (tmp->hdd.count > 0)))
        {
	    info_pc* saver = tmp->next;
	    tmp->next = NULL;
	    output_pc(tmp);
	    tmp->next = saver; 
        }
        tmp = tmp->next;	
    }
}

