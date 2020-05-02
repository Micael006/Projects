#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>
#include "../data.h"
#include "../output/output.h"
#include "../re_builder/re_builder.h"
void student_pc(info_pc *tmp)
{
    int end = 0;
    info_pc *new;
    info_pc *new_tmp;
    while(!((tmp->HDD->count == 0) /* || (условие что компьютер мультимедийный) */))
    {
        if(tmp->next)
	    tmp = tmp->next;
	else
	{
	    end = 1;
	    break;
	}
    }
    if(!end)
    {
        new = (info_pc *)malloc(sizeof(info_pc);
        //чтение 
  	new->last_name = tmp->last_name;
  	new->proc->count = tmp->proc->count;
  	new->proc->type = tmp->proc->type;
  	new->memory = tmp->memory;
  	new->video->typ = tmp->video->typ;
  	new->video->memory = tmp->video->memory);
  	new->hdd->typ = tmp->hdd->typ;
  	new->hdd->memory = tmp.hdd.memory;
  	new->hdd->count = tmp->hdd->count;
  	new->ctrl->built_in = tmp->ctrl->built_in;
  	new->ctrl->discrete = tmp->ctrl->discrete;
 	new->OC = tmp->OC;
	//
        new->next = new->last = NULL;
        new_tmp=re_build(*new, new_tmp);
	new_tmp->next = student_pc(tmp);
    }
    return new_tmp;
}

