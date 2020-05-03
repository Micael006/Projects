#include <stdio.h>
#include <stdlib.h>
#include "extract.h"
#include "../data.h"
#include "../re_builder/re_builder.h"
info_pc *extract(info_pc *tmp)
{
    info_pc *new;
    int size = sizeof(info_pc); //размер нужной структуры
    FILE *stream = fopen(__NAME__, "r");
    if (!stream)
    {
        perror("Не удалось открыть файл");
        return NULL;
    }
    while (!feof(stream))
    { //пока ксть файл или элемент не обозначен как последний
        new = (info_pc *)malloc(size);
        fread(new, size, 1, stream);
        new->next = new->last = NULL;
        tmp=re_build(*new, tmp);
    }
    fclose(stream);
    return tmp;
}
