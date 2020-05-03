#include <stdio.h>
#include "../data.h"
#define EMPT(a)                            \
    if (!a)                                \
    {                                      \
        printf("%s", "Database is empty"); \
        return;                            \
    }
void output_pc(info_pc *tmp)
{
    EMPT(tmp);
    for (;;)
    {
        printf("%20s %9d %8s %6d %8s %10d", tmp->last_name, tmp->proc.count, tmp->proc.type, tmp->memory, (tmp->video.typ == built_in)?"built_in":(tmp->video.typ == discrete)?"discrete":(tmp->video.typ == AGP)?"AGP":"PCI",tmp->video.memory);
        printf("%10s %10d %9d",(tmp->hdd.typ == NONE)?"NONE":(tmp->hdd.typ == SCSI_IDE)?"SCSI_IDE":"ATA_SATA", tmp->hdd.memory, tmp->hdd.count);
        printf("%11d %11d %20s\n", tmp->ctrl.discrete, tmp->ctrl.built_in, tmp->OC);
        if (tmp->next)
        {
            tmp = tmp->next;
        }
        else
        {
            return;
        }
    }
    return;
}
