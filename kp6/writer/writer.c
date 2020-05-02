#include <stdio.h>
#include <stdlib.h>
#include <errno.h>
#include "../data.h"
void writer(info_pc pc)
{				 
    FILE *base = fopen(__NAME__, "w+");//Открываем файл 
    if (!base)
    {
        perror("Can't open file");//Если не удалось открыть файл выводим ошибку и ломаем программу
	return;
    }
	    fwrite(&pc, sizeof(pc), 1, base);//записываем 
    fclose(base);
    return;
}
