#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>
#include "../data.h"
#include "../output/output.h"
#include "../re_builder/re_builder.h"
#define N 20
info_pc* student_pc(info_pc *tmp)
{
    int end = 0;
    info_pc *new;
    info_pc *new_tmp;
    while(!((tmp->hdd.count == 0) /* || (условие что компьютер мультимедийный) */))//В условии тоже беды
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
        new = (info_pc *)malloc(sizeof(info_pc));
        //чтение(сплошные беды) 
	for(int i = 0; i < N; i++)
	{
  	    new->last_name[i] = tmp->last_name[i];
	    new->proc.type[i] = tmp->proc.type[i];
	    new->OC[i] = tmp->OC[i];
	}
  	new->proc.count = tmp->proc.count;
  	new->memory = tmp->memory;
  	new->video.typ = tmp->video.typ;
  	new->video.memory = tmp->video.memory;
  	new->hdd.typ = tmp->hdd.typ;
  	new->hdd.memory = tmp->hdd.memory;
  	new->hdd.count = tmp->hdd.count;
  	new->ctrl.built_in = tmp->ctrl.built_in;
  	new->ctrl.discrete = tmp->ctrl.discrete;
	//
        new->next = new->last = NULL;
        new_tmp=re_build(*new, new_tmp);
	new_tmp->next = student_pc(tmp);
    }
    return new_tmp;
}

